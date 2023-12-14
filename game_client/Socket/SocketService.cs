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
namespace game_client.Socket;

public class SocketService
{
    //private GameObjectCollection _gameObjectsCollection = new GameObjectCollection();
    public event Action<PlayerPixel> OnPlayerCreated;
    private readonly ConcurrentDictionary<string, GameObject> CurrentCanvasObjects = new();
    private readonly HubConnection socket;
    private readonly CanvasObjectFactory factory;
    private WeaponType _weaponType = WeaponType.HANDS;
    IProjectileBuilder builder = new ProjectileBuilder();
    ProjectileDirector director = new ProjectileDirector();
    Projectile originalProjectile;
    Team giants = new Team("Giants", true);
    Team gnomes = new Team("Gnomes", false);
    //private int coinCount;

    private SocketService()
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
        socket.On("UpdateCoinCounter", (int count) => UpdateCoinCounter(count));
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
    private static SocketService? instance;
    public static SocketService GetInstance()
    {
        instance ??= new();
        return instance;
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
                OnPlayerCreated?.Invoke(playerPixel);
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
            if (GetCurrentConnectionId().Equals(entityInfo.Uuid))
            {
                var dimensions = new Vector2(entity.GetWidth(), entity.GetHeight());
                socket.SendAsync("DimensionsCallback", dimensions);
            }

            CurrentCanvasObjects.TryAdd(entityInfo.Uuid, entity);
            //_gameObjectsCollection.Add(entityInfo.Uuid, entity);
        });
    }
    private void RemoveObjectFromCanvas(string uuid)
    {
        //RemoveIteratorObject(uuid);
        CurrentCanvasObjects.TryRemove(uuid, out var entity);
        if(entity != null)
            Dispatcher.UIThread.Invoke(entity.RemoveObjectFromCanvas);
    }

    // private void RemoveIteratorObject(string uuid)
    // {
    //     var iterator = _gameObjectsCollection.CreateIterator();
    //     while (iterator.HasNext())
    //     {
    //         var current = iterator.Next();
    //         if (current.Key == uuid)
    //         {
    //             iterator.Remove();
    //             break;
    //         }
    //     }
    // }

    private async void UpdateEntityPositionInClient(Vector2 direction, string uuid)
    {
        await Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var playerpxl = CurrentCanvasObjects[uuid];
            playerpxl.TeleportTo(direction);
            //await CheckForObjectCollision((PlayerPixel)playerpxl);
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
        var game = Game.GetInstance();
        Projectile projectile;
        Projectile projectileForAll;

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
                        projectileForAll = director.Construct(builder, new Vector2(0, 0), Colors.Red, 6, 6);
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
            Projectile shallowProjectile = projectileForAll.ShallowClone() as Projectile; 
            projectile.AddObjectToCanvas();
            

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

    // public async Task CheckForObjectCollision(PlayerPixel currentPlayer)
    // {
    //     var iterator = _gameObjectsCollection.CreateIterator();
    //
    //     while (iterator.HasNext())
    //     {
    //         try
    //         {
    //             var iteratorObject = iterator.Next();
    //             if (iteratorObject.GameObject is CoinView view && CheckCollision(currentPlayer, view)) //TODO: this entire method should happen in the server
    //             {
    //                 await socket.InvokeAsync("PickupCoin", iteratorObject.Key);
    //                 RemoveObjectFromCanvas(iteratorObject.Key);
    //                 giants.Operation();
    //                 gnomes.Operation();
    //                 coinCount++;
    //                 UpdateCoinCounter(coinCount); // Update the coin counter
    //
    //                 break;
    //             }
    //             else if (iteratorObject.GameObject is EnemyPixel enemy && CheckCollision(currentPlayer, enemy))
    //             {
    //                 currentPlayer.DecreaseHealth(); // Decrease player health
    //             }
    //         }
    //         catch (Exception e)
    //         {
    //             Console.WriteLine($"[ERROR] {e.Message} {e.StackTrace}");
    //         }
    //     }
    // }
    //
    // private bool CheckCollision(PlayerPixel player, GameObject coin)
    // {
    //     float extraPadding = 0;  // the extra area for detection
    //
    //     float halfWidthPlayer = player.GetWidth() / 2f + extraPadding;
    //     float halfHeightPlayer = player.GetHeight() / 2f + extraPadding;
    //     float halfWidthObject = coin.GetWidth() / 2f + extraPadding;
    //     float halfHeightObject = coin.GetHeight() / 2f + extraPadding;
    //
    //     return Math.Abs(player.Location.X - coin.Location.X) < (halfWidthPlayer + halfWidthObject) &&
    //            Math.Abs(player.Location.Y - coin.Location.Y) < (halfHeightPlayer + halfHeightObject);
    // }

    private void UpdateCoinCounter(int count)
    {
        // Update the UI with the current coin count
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            var mainWindow = MainWindow.GetInstance();
            mainWindow.coinCounter.Text = $"Coins: {count}";
        });
    }
    public void setWeaponProjectiles(WeaponType weapon)
    {
        if (weapon.ToString() == _weaponType.ToString()) return;
        Console.WriteLine(weapon.ToString());
        switch (weapon)
        {
            case WeaponType.PISTOL:
                {
                    originalProjectile = director.Construct(builder, new Vector2(0, 0), Colors.AliceBlue, 10, 10);
                    _weaponType = WeaponType.PISTOL;
                    break;
                }
            case WeaponType.SNIPER:
                {
                    originalProjectile = director.Construct(builder, new Vector2(0, 0), Colors.Red, 6, 6);
                    _weaponType = WeaponType.SNIPER;
                    break;
                }
            case WeaponType.ROCKET:
                {
                    originalProjectile = director.Construct(builder, new Vector2(0, 0), Colors.OrangeRed, 15, 15);
                    _weaponType = WeaponType.ROCKET;
                    break;
                }
            case WeaponType.CANNON:
                {
                    var cannonCreator = new CannonProjectileCreation();
                    originalProjectile = cannonCreator.CreateProjectile(new Vector2(0, 0));
                    _weaponType = WeaponType.CANNON;
                    break;
                }
            default:
                {
                    originalProjectile = director.Construct(builder, new Vector2(0, 0), Colors.AliceBlue, 10, 10);
                    break;
                }
        }
        
    }
    // public IGameObjectIterator GetGameObjectIterator()
    // {
    //     return _gameObjectsCollection.CreateIterator();
    // }

    public void ResetCoinCount() => UpdateCoinCounter(0);

    private static Avalonia.Media.Color ConvertRgbToAvaloniaColor(RGB rgb)
    {
        return Avalonia.Media.Color.FromArgb(255, rgb.R, rgb.G, rgb.B);
    }

}
