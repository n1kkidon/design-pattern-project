using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Models;
using game_client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using shared;

namespace game_client.Socket;

public class SocketService
{
    private readonly ConcurrentDictionary<string, GameObject> CurrentCanvasObjects = new();
    private readonly HubConnection socket;
    private readonly CanvasObjectFactory factory;

    private int coinCount;

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

        coinCount = 0;
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
            await CheckForObjectCollision((PlayerPixel)playerpxl);
        });
    }

    public async Task CheckForObjectCollision(PlayerPixel currentPlayer)
    {
        foreach (var coin in CurrentCanvasObjects)
        {
            if (coin.Value is CoinView view && CheckCollision(currentPlayer, view)) //TODO: this entire method should happen in the server
            {
                await socket.InvokeAsync("PickupCoin", coin.Key);
                RemoveObjectFromCanvas(coin.Key);

                coinCount++;
                UpdateCoinCounter(); // Update the coin counter

                break;
            }
            else if (coin.Value is EnemyPixel enemy && CheckCollision(currentPlayer, enemy))
            {
                currentPlayer.DecreaseHealth(); // Decrease player health
                currentPlayer.UpdateHealthBar(); // Update the health bar
            }
        }
    }

    private bool CheckCollision(PlayerPixel player, GameObject coin)
    {
        float extraPadding = 0;  // the extra area for detection

        float halfWidthPlayer = player.GetWidth() / 2f + extraPadding;
        float halfHeightPlayer = player.GetHeight() / 2f + extraPadding;
        float halfWidthObject = coin.GetWidth() / 2f + extraPadding;
        float halfHeightObject = coin.GetHeight() / 2f + extraPadding;

        return Math.Abs(player.Location.X - coin.Location.X) < (halfWidthPlayer + halfWidthObject) &&
               Math.Abs(player.Location.Y - coin.Location.Y) < (halfHeightPlayer + halfHeightObject);
    }

    public void UpdateCoinCounter()
    {
        // Update the UI with the current coin count
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var mainWindow = MainWindow.GetInstance();
            mainWindow.coinCounter.Text = $"Coins: {coinCount}";
        });
    }

    public void ResetCoinCount()
    {
        coinCount = 0; // Reset the coin count
    }
}
