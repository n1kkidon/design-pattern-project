using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Models;
using Microsoft.AspNetCore.SignalR.Client;
using shared;
using Tmds.DBus.Protocol;
namespace game_client.Socket;

public class SocketService
{
    private readonly ConcurrentDictionary<string, GameObject> CurrentCanvasObjects = new();
    private readonly HubConnection socket;
    private readonly CanvasObjectFactory factory;
    private SocketService()
    {
        var url = Constants.ServerIP;
        factory = new();
        socket = new HubConnectionBuilder()
        .WithUrl(url + "/playerHub")
        .Build();   
        socket.On("AddEntityToLobbyClient", (CanvasObjectInfo p) => AddEntityToLobbyClient(p));
        socket.On("UpdateEntityPositionInClient", (Vector2 d, string uuid) => UpdateEntityPositionInClient(d, uuid));
        socket.On("RemoveObjectFromCanvas", (string uuid) => RemoveObjectFromCanvas(uuid));

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
        await socket.InvokeAsync("EntityMoved", direction);
    }

    public async Task JoinGameLobby(string name, Color color) //this goes through backend, which calls the AddPlayerToLobby() below
    {
       await socket.SendAsync("AddEntityToLobby", name, new RGB(color.R, color.G, color.B), EntityType.PLAYER);
    }
    public async Task AddOpponentToGame()
    {
        await socket.SendAsync("AddEntityToLobby", "Opponent1", new RGB(255, 0, 0), EntityType.ENEMY);
    }

    private void AddEntityToLobbyClient(CanvasObjectInfo entityInfo)
    {
        Dispatcher.UIThread.Invoke(() => {
            var entity = factory.CreateCanvasObject(entityInfo);
            entity.AddObjectToCanvas();
            CurrentCanvasObjects.TryAdd(entityInfo.Uuid, entity);
        });
    }

    private void RemoveObjectFromCanvas(string uuid)
    {
        CurrentCanvasObjects.TryRemove(uuid, out var entity);
        if(entity != null)
            Dispatcher.UIThread.Invoke(entity.RemoveObjectFromCanvas);
    }

    private async void UpdateEntityPositionInClient(Vector2 direction, string uuid)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var playerpxl = CurrentCanvasObjects[uuid];
            playerpxl.TeleportTo(direction);
            await CheckForCoinPickup((PlayerPixel)playerpxl);
        });
    }

    public async Task CheckForCoinPickup(PlayerPixel currentPlayer)
    {
        foreach (var coin in CurrentCanvasObjects)
        {
            if (coin.Value is CoinView view && CheckCollision(currentPlayer, view)) //TODO: call server here
            {
                await socket.InvokeAsync("PickupCoin", coin.Key);
                RemoveObjectFromCanvas(coin.Key);
                break;
            }
        }
    }
    private bool CheckCollision(PlayerPixel player, CoinView coin)
    {
        double extraPadding = 1;  // the extra area for detection

        double halfSizePlayer = player.GetWidth() / 2.0 + extraPadding;
        double halfSizeCoin = coin.GetWidth() / 2.0 + extraPadding;

        return Math.Abs(player.Location.X - coin.Location.X) < (halfSizePlayer + halfSizeCoin) &&
               Math.Abs(player.Location.Y - coin.Location.Y) < (halfSizePlayer + halfSizeCoin);
    }
}
