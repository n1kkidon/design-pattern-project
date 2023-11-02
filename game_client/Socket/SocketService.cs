using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Models;
using game_client.Views;
using Microsoft.AspNetCore.SignalR.Client;
using shared;

namespace game_client.Socket;

public class SocketService
{
    public event Action<PlayerPixel> OnPlayerCreated;
    private readonly ConcurrentDictionary<string, GameObject> CurrentCanvasObjects = new();
    private AbstractEnemyFactory enemyFactory = new BasicEnemyFactory();
    private readonly HubConnection socket;
    private readonly CanvasObjectFactory factory;

    Projectile originalProjectile = new Projectile(new Vector2(0, 0), Colors.White);

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
        socket.On("UpdateOnProjectileInClient", (Vector2 direction, Vector2 initialPosition) => UpdateOnProjectileInClient(direction, initialPosition));


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
    public async Task OnCurrentPlayerShoot(Vector2 direction)
    {
        await socket.InvokeAsync("ProjectileShot", direction);
    }

    public async Task OnCurrentPlayerMove(Vector2 direction)
    {
        await socket.InvokeAsync("EntityMoved", direction);
    }

    public async Task JoinGameLobby(string name, Color color, WeaponType weaponType) //this goes through backend, which calls the AddPlayerToLobby() below
    {
       await socket.SendAsync("AddEntityToLobby", name, new RGB(color.R, color.G, color.B), EntityType.PLAYER, weaponType);
    }
    public async Task AddOpponentToGame()
    {
        string[] difficulties = new[] { "EasySoldier", "HardSoldier", "EasyKnight", "HardKnight" };
        foreach (var difficulty in difficulties)
        {
            await socket.SendAsync("AddEntityToLobby", $"{difficulty}", new RGB(255, 0, 0), EntityType.ENEMY, WeaponType.HANDS);
        }
    }
    private void AddEntityToLobbyClient(CanvasObjectInfo entityInfo)
    {
        Dispatcher.UIThread.Invoke(() => {
            var entity = factory.CreateCanvasObject(entityInfo);
            entity.AddObjectToCanvas();
            CurrentCanvasObjects.TryAdd(entityInfo.Uuid, entity);
            if (entityInfo.EntityType == EntityType.PLAYER && entity is PlayerPixel playerPixel)
            {
                OnPlayerCreated?.Invoke(playerPixel);
            }
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

    private async void UpdateOnProjectileInClient(Vector2 direction, Vector2 initialPosition) //TODO: change direction from click location to final (colide) location
    {
        var game = Game.GetInstance();
        Projectile projectile;

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            projectile = originalProjectile.Clone() as Projectile;
            Projectile shallowProjectile = originalProjectile.ShallowClone() as Projectile; // cia laikina, nes reikės ataskaitai
            projectile.AddObjectToCanvas();

            Console.WriteLine($"Original Hash Code: {originalProjectile.GetHashCode()}"); // cia laikina, nes reikės ataskaitai 
            Console.WriteLine($"Deep copy Hash Code: {projectile.GetHashCode()}"); // cia laikina, nes reikės ataskaitai
            Console.WriteLine($"Shallow Hash Code: {shallowProjectile.GetHashCode()}"); // cia laikina, nes reikės ataskaitai

            game.OnTick += logic;
        });
        async void logic()
        {
            var current = CalculateCurrentProjectilePosition(direction, initialPosition);
            // Update the object's position
            await Dispatcher.UIThread.InvokeAsync(() =>
            {
                projectile.TeleportTo(current);
            });

            var distX = direction.X - current.X;
            var distY = direction.Y - current.Y;
            // Check if the object has reached the end position
            float distanceRemaining = (float)Math.Sqrt(distX * distX + distY * distY);

            if (distanceRemaining < Constants.ProjectileDistPerTick)
            {
                game.OnTick -= logic; //TODO: destroy projectile when it hits something
            }
            else
            {
                // Update the current position as the new start position
                initialPosition = new Vector2(current.X, current.Y);
            }
        };
        
    }

    private Vector2 CalculateCurrentProjectilePosition(Vector2 endPosition, Vector2 startPosition)
    {       
        // Calculate the direction vector from start to end
        float dx = endPosition.X - startPosition.X;
        float dy = endPosition.Y - startPosition.Y;
        // Calculate the length of the direction vector
        float length = (float)Math.Sqrt(dx * dx + dy * dy);
        // Normalize the direction vector
        if (length > 0)
        {
            dx /= length;
            dy /= length;
        }
        // Calculate the new position
        float currentX = startPosition.X + dx * Constants.ProjectileDistPerTick;
        float currentY = startPosition.Y + dy * Constants.ProjectileDistPerTick;
        return new Vector2(currentX, currentY);
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
