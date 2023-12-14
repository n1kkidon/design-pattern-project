using System.Collections.Concurrent;
using game_server.Iterator;
using game_server.Memento;
using game_server.Services;
using Microsoft.AspNetCore.SignalR;
using shared;
namespace game_server.Sockets;
public class PlayerHub : Hub
{
    public static readonly ConcurrentDictionary<string, CanvasObjectInfo> CurrentCanvasItems = new();
    public static readonly GameObjectCollection GameObjectsCollection = new GameObjectCollection();
    private static readonly Random Rnd = new();
    private readonly CoinBackgroundService _coinService;
    private static readonly ConcurrentDictionary<string, PlayerMemento> PlayerStates = new();

    public PlayerHub(CoinBackgroundService coinService)
    {
        _coinService = coinService;
    }

    
    public static void RemoveIteratorObject(string uuid)
    {
        var iterator = GameObjectsCollection.CreateIterator();
        while (iterator.HasNext())
        {
            var current = iterator.Next();
            if (current.Key == uuid)
            {
                iterator.Remove();
                break;
            }
        }
    }
    public IGameObjectIterator GetGameObjectIterator()
    {
        return GameObjectsCollection.CreateIterator();
    }

    public async Task AddObstacleToGame()
    {
        var info = new CanvasObjectInfo
        {
            EntityType = EntityType.OBSTACLE,
            
            Name = "Obstacle",
            Color = new(128, 128, 128), 
            Uuid = Guid.NewGuid().ToString(),
            Location = new(Rnd.Next((int)(Constants.MapWidth * 0.9)),
                                   Rnd.Next((int)(Constants.MapHeight * 0.9)))
        };

        CurrentCanvasItems.TryAdd(info.Uuid, info);
        GameObjectsCollection.Add(info.Uuid, info);
        await Clients.All.SendAsync("AddEntityToLobbyClient", info);
    }
    
    public async Task AddEntityToLobby(string name, RGB color, EntityType entityType, WeaponType weaponType)
    {
        var uuid = entityType == EntityType.PLAYER ? Context.ConnectionId : Guid.NewGuid().ToString();
        Vector2 location;
        int health = Constants.PlayerHealth;

        if (entityType == EntityType.PLAYER && PlayerStates.TryGetValue(name, out var savedState))
        {
            // Restore state from memento
            var originator = new PlayerOriginator();
            originator.RestoreFromMemento(savedState);
            location = originator.Location;
            health = originator.Health;
            // Restore other properties if needed
        }
        else
        {
            location = new Vector2(Rnd.Next((int)(Constants.MapWidth * 0.9)),
                                   Rnd.Next((int)(Constants.MapHeight * 0.9)));
        }

        var info = new CanvasObjectInfo
        {
            EntityType = entityType,
            Color = color,
            Name = name,
            Uuid = uuid,
            Location = location,
            WeaponType = weaponType,
            Health = health
        };

        var freshJoined = CurrentCanvasItems.TryAdd(info.Uuid, info);
        GameObjectsCollection.Add(info.Uuid, info); 
        var existingEntities = CurrentCanvasItems.Values.ToList();
        await Clients.Others.SendAsync("AddEntityToLobbyClient", info); // Player is displayed for other online clients

        if (freshJoined && entityType == EntityType.PLAYER)
        {
            foreach (var entity in existingEntities)
                await Clients.Caller.SendAsync("AddEntityToLobbyClient", entity); // All items are displayed for the new player
        }
        else
            await Clients.Caller.SendAsync("AddEntityToLobbyClient", info);
    }

    public async Task ProjectileShot(Vector2 direction, string weaponType)
    {
        var playerInfo = CurrentCanvasItems[Context.ConnectionId];
        await Clients.All.SendAsync("UpdateOnProjectileInClient", direction, playerInfo.Location, weaponType);
    }
    public async Task EntityMoved(Vector2 moveDirection)
    {
        var playerInfo = CurrentCanvasItems[Context.ConnectionId];
        
        var x = 0f;
        var y = 0f;
        if(moveDirection.X > 0)
            x = 1f;
        else if(moveDirection.X < 0)
            x = -1f;    
        if(moveDirection.Y > 0)
            y = 1f;
        else if(moveDirection.Y < 0)
            y = -1f;
        var magn = (float)Math.Sqrt(x*x + y*y);
        if(magn == 0)
            return;
        var newX = playerInfo.Location.X + x * Constants.MoveStep / magn;
        var newY = playerInfo.Location.Y + y * Constants.MoveStep / magn;
        var oldLocation = playerInfo.Location;
        playerInfo.Location = new(newX, newY);
        
        var iterator = GameObjectsCollection.CreateIterator();

        while (iterator.HasNext())
        {
            try
            {
                var iteratorObject = iterator.Next();
                var collides = CheckCollision(playerInfo, iteratorObject.ObjectInfo);
                if (!collides) continue;

                switch (iteratorObject.ObjectInfo.EntityType)
                {
                    case EntityType.COIN:
                        await PickupCoin(iteratorObject.Key);
                        await Clients.All.SendAsync("RemoveObjectFromCanvas", iteratorObject.Key);
                        await Clients.All.SendAsync("MobOperationsTemp");
                        playerInfo.CoinCount++;
                        await Clients.Caller.SendAsync("UpdateCoinCounter", playerInfo.CoinCount);
                        break;
                    case EntityType.ENEMY:
                        playerInfo.Health -= Constants.EnemyDamage; //TODO: Make this different for different enemies
                        await Clients.All.SendAsync("UpdateEntityHealthInClient", Constants.EnemyDamage, Context.ConnectionId);
                        break;
                    case EntityType.OBSTACLE:
                        playerInfo.Location = oldLocation;
                        return;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"[ERROR] {e.Message} {e.StackTrace}");
            }
        }


        await Clients.All.SendAsync("UpdateEntityPositionInClient", playerInfo.Location, Context.ConnectionId);
    }
    
    private static bool CheckCollision(CanvasObjectInfo player, CanvasObjectInfo obj)
    {
        float extraPadding = 0;  // the extra area for detection
        

        float halfWidthPlayer = player.Width / 2f + extraPadding;
        float halfHeightPlayer = player.Height / 2f + extraPadding;
        float halfWidthObject = obj.Width / 2f + extraPadding;
        float halfHeightObject = obj.Height / 2f + extraPadding;

        return Math.Abs(player.Location.X - obj.Location.X) < (halfWidthPlayer + halfWidthObject) &&
               Math.Abs(player.Location.Y - obj.Location.Y) < (halfHeightPlayer + halfHeightObject);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (CurrentCanvasItems.TryGetValue(Context.ConnectionId, out var playerInfo) && playerInfo.EntityType == EntityType.PLAYER)
        {
            // Save player state to memento
            var originator = new PlayerOriginator
            {
                Location = playerInfo.Location,
                Health = playerInfo.Health
                // Add other properties to save if needed
            };
            PlayerStates[playerInfo.Name] = originator.SaveToMemento();
        }

        CurrentCanvasItems.TryRemove(Context.ConnectionId, out _);
        RemoveIteratorObject(Context.ConnectionId);
        await Clients.Others.SendAsync("RemoveObjectFromCanvas", Context.ConnectionId);
        Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} disconnected from game server chat socket.");
        await base.OnDisconnectedAsync(exception);
    }


    public override Task OnConnectedAsync() //this really does nothing except display in server console that connection happened
    {
        try
        {
            Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} connected to game server chat socket.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error on connecting client: {ex.Message}");
        }
        return base.OnConnectedAsync();
    }

    public async Task PickupCoin(string coinId)
    {
        Console.WriteLine("Picked up coinas");
        await _coinService.PickupCoin(coinId);
    }
}