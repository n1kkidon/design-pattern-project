using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Models;
using game_client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using shared;
using Tmds.DBus.Protocol;
namespace game_client.Socket;

public class SocketService
{
    private readonly ConcurrentDictionary<string, PlayerPixel> ConnectedPlayers = new();
    private readonly ConcurrentDictionary<string, PlayerPixel> ConnectedOpponents = new();
    private HubConnection socket {get; set;}
    private SocketService()
    {
        var url = Constants.ServerIP;
        
        socket = new HubConnectionBuilder()
        .WithUrl(url + "/playerHub")
        .Build();   
        socket.On("AddPlayerToLobbyClient", (PlayerInfo p) => AddPlayerToLobbyClient(p));
        socket.On("UpdateClientPosition", (Direction d, string uuid) => UpdateClientPosition(d, uuid));
        socket.On("RemoveDisconnectedPlayer", (string uuid) => RemoveDisconnectedPlayer(uuid));
        socket.On("AddOpponentToGameClient", (OpponentInfo o) => AddOpponentToGameClient(o));
        socket.On("AddCoinToMap", (Coin coin, string coinId) => AddCoinToGameClient(coin, coinId));

        socket.StartAsync().Wait();
        Console.WriteLine("connected to server.");
    }
    public string GetCurrentConnectionId()
    {
        return socket.ConnectionId ?? "";
    }
    private static SocketService? instance;
    public static SocketService GetInstance()
    {
        instance ??= new();
        return instance;
    }

    public async Task OnCurrentPlayerMove(Direction direction)
    {
        await socket.InvokeAsync("ClientMoved", direction);
    }

    public async Task JoinGameLobby(string name, Color color) //this goes through backend, which calls the AddPlayerToLobby() below
    {
       await socket.SendAsync("AddPlayerToLobby", name, new RGB(color.R, color.G, color.B));
    }
    public async Task AddOpponentToGame()
    {
        await socket.SendAsync("AddOpponentToGame");
    }

    private void AddPlayerToLobbyClient(PlayerInfo player)
    {
        Dispatcher.UIThread.Invoke(() => {
            var playerpxl = new PlayerPixel(player.Name, Color.FromRgb(player.Color.R, player.Color.G, player.Color.B), player.X, player.Y);
            MainWindow.GetInstance().canvas.Children.Add(playerpxl.PlayerObject);
            ConnectedPlayers.TryAdd(player.Uuid, playerpxl);
        });
    }


    private void RemoveDisconnectedPlayer(string uuid)
    {
        ConnectedPlayers.TryRemove(uuid, out var playerpxl);
        if(playerpxl != null)
            Dispatcher.UIThread.Invoke(() => {
                MainWindow.GetInstance().canvas.Children.Remove(playerpxl.PlayerObject);
            });
    }

    private void UpdateClientPosition(Direction direction, string uuid)
    {
        Dispatcher.UIThread.Invoke(() => {
            var playerpxl = ConnectedPlayers[uuid];
            playerpxl.Move(direction);
        });
    }
    private void AddOpponentToGameClient(OpponentInfo opponent)
    {
        Dispatcher.UIThread.Invoke(() => {
            var opponentPxl = new PlayerPixel(opponent.Name, Color.FromRgb(opponent.Color.R, opponent.Color.G, opponent.Color.B), opponent.X, opponent.Y);
            MainWindow.GetInstance().canvas.Children.Add(opponentPxl.PlayerObject);
            ConnectedOpponents.TryAdd(opponent.Uuid, opponentPxl);
        });
    }

    private void AddCoinToGameClient(Coin coin, string coinId)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var coinView = new CoinView(coin.X, coin.Y);
            MainWindow.GetInstance().canvas.Children.Add(coinView.CoinObject);
            //coinViews[coinId] = coinView;  // Store the coinView with its ID
        });

    }


    public void OnAddCoinToMap(Action<Coin, string> action)
    {
        socket.On<Coin, string>("AddCoinToMap", action);
    }
}
