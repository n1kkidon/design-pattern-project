using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using shared;

namespace game_client.Socket;

public class SocketService
{
    private readonly ConcurrentDictionary<string, PlayerPixel> ConnectedPlayers = new();
    private HubConnection socket {get; set;}
    public SocketService()
    {
        socket = new HubConnectionBuilder()
        .WithUrl("http://localhost:5000/exampleChat") //TODO: rename this, move it to settings
        .Build();
        socket.On("SendMessageToClient", (string message) => OnMessageReceived(message));     
        socket.On("AddPlayerToLobbyClient", (PlayerInfo p) => AddPlayerToLobbyClient(p));
        socket.On("UpdateClientPosition", (Direction d, string uuid) => UpdateClientPosition(d, uuid));
        socket.On("RemoveDisconnectedPlayer", (string uuid) => RemoveDisconnectedPlayer(uuid));

        socket.StartAsync().Wait();
        Console.WriteLine("connected to server.");
    }

    private async Task OnMessageReceived(string message) //this does nothing now
    {
        Console.WriteLine(message);
    }

    public async Task JoinGameLobby(string name, Color color) //this goes through backend, which calls the AddPlayerToLobby() below
    {
       await socket.SendAsync("AddPlayerToLobby", name, new RGB(color.R, color.G, color.B));
    }

    //TODO: current movement assumes that there is no packet loss/desync, we are not actually sending precise location of clients. Might need to fix this in the future.
    public async Task OnCurrentPlayerMove(Direction direction)
    {
        await socket.InvokeAsync("ClientMoved", direction);
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

}
