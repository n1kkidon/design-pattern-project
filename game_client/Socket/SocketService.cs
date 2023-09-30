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
    private readonly ConcurrentDictionary<string, CoinView> coinViews = new();
    private HubConnection socket {get; set;}
    private SocketService()
    {
        var url = Constants.ServerIP;
        
        socket = new HubConnectionBuilder()
        .WithUrl(url + "/playerHub")
        .Build();   
        socket.On("AddPlayerToLobbyClient", (PlayerInfo p) => AddPlayerToLobbyClient(p));
        socket.On("UpdateClientPosition", (Vector2 d, string uuid) => UpdateClientPosition(d, uuid));
        socket.On("RemoveDisconnectedPlayer", (string uuid) => RemoveDisconnectedPlayer(uuid));
        socket.On("AddOpponentToGameClient", (OpponentInfo o) => AddOpponentToGameClient(o));
        socket.On("AddCoinToMap", (Coin coin, string coinId) => AddCoinToGameClient(coin, coinId));
        socket.On("CoinPickedUp", (string coinId) => RemoveCoinFromUI(coinId));

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

    public async Task OnCurrentPlayerMove(Vector2 direction)
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
            var playerpxl = new PlayerPixel(player.Name, Color.FromRgb(player.Color.R, player.Color.G, player.Color.B), player.Location);
            playerpxl.AddObjectToCanvas();
            System.Console.WriteLine($"Width: {playerpxl.GetWidth()} Height: {playerpxl.GetHeight()}");
            ConnectedPlayers.TryAdd(player.Uuid, playerpxl);
        });
    }


    private void RemoveDisconnectedPlayer(string uuid)
    {
        ConnectedPlayers.TryRemove(uuid, out var playerpxl);
        if(playerpxl != null)
            Dispatcher.UIThread.Invoke(() => {
                playerpxl.RemoveObjectFromCanvas();
            });
    }

    private async void UpdateClientPosition(Vector2 direction, string uuid)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var playerpxl = ConnectedPlayers[uuid];
            playerpxl.Move(direction);
            await CheckForCoinPickup(playerpxl);
        });
    }

    private void AddOpponentToGameClient(OpponentInfo opponent)
    {
        Dispatcher.UIThread.Invoke(() => {
            var opponentPxl = new PlayerPixel(opponent.Name, Color.FromRgb(opponent.Color.R, opponent.Color.G, opponent.Color.B), opponent.Location);
            opponentPxl.RemoveObjectFromCanvas();
            ConnectedOpponents.TryAdd(opponent.Uuid, opponentPxl);
        });
    }

    private void AddCoinToGameClient(Coin coin, string coinId)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var coinView = new CoinView(coin.X, coin.Y);
            MainWindow.GetInstance().canvas.Children.Add(coinView.CoinObject);
            coinViews[coinId] = coinView;  // Store the coinView with its ID
        });
    }

    public async Task CheckForCoinPickup(PlayerPixel currentPlayer)
    {
        foreach (var coin in coinViews)
        {
            if (CheckCollision(currentPlayer, coin.Value))
            {
                await socket.InvokeAsync("PickupCoin", coin.Key);
                RemoveCoinFromUI(coin.Key);
                break;
            }
        }
    }
    private bool CheckCollision(PlayerPixel player, CoinView coin)
    {
        double sizePlayer = 15;
        double sizeCoin = 15;
        double extraPadding = 5;  // the extra area for detection

        double halfSizePlayer = sizePlayer / 2.0 + extraPadding;
        double halfSizeCoin = sizeCoin / 2.0 + extraPadding;

        double playerCenterX = player.Location.X;
        double playerCenterY = player.Location.Y;

        double coinCenterX = Canvas.GetLeft(coin.CoinObject) + sizeCoin / 2.0;
        double coinCenterY = Canvas.GetTop(coin.CoinObject) + sizeCoin / 2.0;

        return Math.Abs(playerCenterX - coinCenterX) < (halfSizePlayer + halfSizeCoin) &&
               Math.Abs(playerCenterY - coinCenterY) < (halfSizePlayer + halfSizeCoin);
    }

    private void RemoveCoinFromUI(string coinId)
    {
        if (coinViews.TryRemove(coinId, out var coin))
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                MainWindow.GetInstance().canvas.Children.Remove(coin.CoinObject);
            });
        }
    }
}
