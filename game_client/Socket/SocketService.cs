using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.AbstractFactory;
using game_client.Models;
using game_client.Views;
using shared;
using game_client.Builder;
using game_client.Template;
using game_client.Composite;
using game_client.Mediator;
using game_client.Models.CanvasItems;

namespace game_client.Socket;

public class SocketService : BaseComponent
{
    //private GameObjectCollection _gameObjectsCollection = new GameObjectCollection();
    public event Action<PlayerPixel, IMediator, int> OnPlayerCreated;
    private readonly ConcurrentDictionary<string, GameObject> CurrentCanvasObjects = new();
    private readonly HubConnection socket;
    private readonly CanvasObjectFactory factory;
    Team giants = new Team("Giants", true);
    Team gnomes = new Team("Gnomes", false);
    //private int coinCount;

    public SocketService()
    {
        var url = Constants.ServerIP;
        factory = new();
        socket = new HubConnectionBuilder()
        .WithUrl(url + "/playerHub")
        .Build();   
        socket.On("AddEntityToLobbyClient", (CanvasObjectInfo p) => AddEntityToLobbyClient(p));
        socket.On("UpdateEntityPositionInClient", (Vector2 d, string uuid) => UpdateEntityPositionInClient(d, uuid));
        socket.On("UpdateEntityHealthInClient", (int hpAmount, string uuid) => UpdateEntityHealthInClient(hpAmount, uuid));
        socket.On("RemoveObjectFromCanvas", (string uuid) => RemoveObjectFromCanvas(uuid));
        socket.On("UpdateOnProjectileInClient", (Vector2 direction, Vector2 initialPosition, string weaponType) => UpdateOnProjectileInClient(direction, initialPosition, weaponType));
        socket.On("UpdateCoinCounter", (int count) => Mediator.Notify(this, "UpdateCoinCounter", count));
        socket.On("MobOperationsTemp", MobOperationsTemp);

        socket.StartAsync().Wait();
        Console.WriteLine("connected to server.");
    }

    private void MobOperationsTemp()
    {
        giants.Operation();
        gnomes.Operation();
    }

    public string GetCurrentConnectionId()
    {
        return socket.ConnectionId ?? "";
    }

    public async Task OnCurrentPlayerShoot(IVector2 direction, WeaponType weaponType)
    {
        await socket.InvokeAsync("ProjectileShot", direction.ToVector2(), weaponType.ToString());
    }

    public async Task OnCurrentPlayerMove(Vector2 direction)
    {
        await socket.InvokeAsync("EntityMoved", direction);
    }

    public async Task JoinGameLobby(string name, Color color, WeaponType weaponType) //this goes through backend, which calls the AddPlayerToLobby() below
    {
       await socket.SendAsync("AddObstacleToGame");
       await socket.SendAsync("AddEntityToLobby", name, new RGB(color.R, color.G, color.B), EntityType.PLAYER, weaponType);
       await socket.SendAsync("AddEntityToLobby", $"Giant", ConvertRgbToAvaloniaColor(new RGB(255, 0, 0)), EntityType.ENEMY, WeaponType.HANDS);
       await socket.SendAsync("AddEntityToLobby", $"Gnome", ConvertRgbToAvaloniaColor(new RGB(255, 0, 0)), EntityType.ENEMY, WeaponType.HANDS);
    }
    public async Task AddOpponentToGame()
    {
        var enemyCombinations = new List<(string difficulty, string enemyType)>{
            ("Easy", "Soldier"),
            ("Hard", "Soldier"),
            ("Easy", "Knight"),
            ("Hard", "Knight")
        };

        foreach (var combo in enemyCombinations)
        {
            AbstractEnemyFactory enemyFactory = combo.difficulty switch
            {
                "Easy" => new EasyEnemyFactory(),
                "Hard" => new HardEnemyFactory()
            };
            var enemyPixel = enemyFactory.CreateEnemyPixel(combo.enemyType, ConvertRgbToAvaloniaColor(new RGB(255, 0, 0)), new Vector2());
            var enemyStats = enemyFactory.CreateEnemyStats(combo.enemyType);

            await socket.SendAsync("AddEntityToLobby", $"{combo.difficulty}{combo.enemyType}", ConvertRgbToAvaloniaColor(new RGB(255, 0, 0)), EntityType.ENEMY, WeaponType.HANDS);

            // We can send stats, pixel to anywhere else, in order to interact. Maybe adjust CanvasObjectInfo.cs, to take in health/damage.

            if (enemyStats.Health > 150)
            {
                Console.WriteLine($"Spawned insane monster - {combo.difficulty}{combo.enemyType} - Health: {enemyStats.Health}, Damage: {enemyStats.Damage}");
            }
        }
    }

    private void AddEntityToLobbyClient(CanvasObjectInfo entityInfo)
    {
        Dispatcher.UIThread.Invoke(() => {

            GameObject entity;
        
            if (entityInfo.EntityType == EntityType.OBSTACLE)
            {
                string imagePath = "./Assets/obstacle.png";
                
                ObstacleProxy obs = new ObstacleProxy(new Obstacle(entityInfo.Location, imagePath));
                entity = obs;
            
                obs.LoadImage();
            }
            else
            {
                entity = factory.CreateCanvasObject(entityInfo);
            }
            if (entityInfo.EntityType == EntityType.PLAYER && entity is PlayerPixel playerPixel)
            {
                OnPlayerCreated?.Invoke(playerPixel, Mediator, entityInfo.CoinCount);
            }
            switch (entityInfo.Name)
            {
                case "Giant" when entity is EnemyPixel enemyGiant:
                    giants.Add(enemyGiant);
                    break;
                case "Gnome" when entity is EnemyPixel enemyGnome:
                    gnomes.Add(enemyGnome);
                    break;
            }

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
        });
    }
    
    private async void UpdateEntityHealthInClient(int amount, string uuid)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var playerpxl = CurrentCanvasObjects[uuid];
            ((PlayerPixel)playerpxl).DecreaseHealth(amount); //TODO: move DecreaseHealth() to a shared object between animate objects
        });
    }

    private async void UpdateOnProjectileInClient(Vector2 direction, Vector2 initialPosition, string weaponType) //TODO: change direction from click location to final (colide) location
    {
        Projectile projectile;
        Projectile projectileForAll;
        IProjectileBuilder builder = new ProjectileBuilder();
        ProjectileDirector director = new ProjectileDirector();

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            switch (weaponType)
            {
                case "PISTOL":
                    {
                        projectileForAll = director.Construct(builder, new Vector2(0, 0), Colors.AliceBlue, 10, 10);
                        break;
                    }
                case "SNIPER":
                    {
                        var sniperCreator = new SniperProjectileCreation();
                        projectileForAll = sniperCreator.CreateProjectile(new Vector2(0, 0));
                        break;
                    }
                case "ROCKET":
                    {
                        projectileForAll = director.Construct(builder, new Vector2(0, 0), Colors.OrangeRed, 15, 15);
                        break;
                    }
                case "CANNON":
                    {
                        var cannonCreator = new CannonProjectileCreation();
                        projectileForAll = cannonCreator.CreateProjectile(new Vector2(0, 0));
                        break;
                    }
                default:
                    {
                        projectileForAll = director.Construct(builder, new Vector2(0, 0), Colors.AliceBlue, 10, 10);
                        break;
                    }
            }
            projectile = projectileForAll.Clone() as Projectile;
            //Projectile shallowProjectile = projectileForAll.ShallowClone() as Projectile; 
            projectile.AddObjectToCanvas();
            
            Mediator.Notify(this, "SubscribeToGame_OnTick", logic);
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
                //game.OnTick -= logic; //TODO: destroy projectile when it hits something
                await Mediator.Notify(this, "UnsubscribeFromGame_OnTick", logic);
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
    

    // public void UpdateCoinCounter(int count)
    // {
    //     // Update the UI with the current coin count
    //     Dispatcher.UIThread.InvokeAsync(() =>
    //     {
    //         var mainWindow = MainWindow.GetInstance();
    //         mainWindow.coinCounter.Text = $"Coins: {count}";
    //     });
    // }

    private static Color ConvertRgbToAvaloniaColor(RGB rgb)
    {
        return Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
    }

}
