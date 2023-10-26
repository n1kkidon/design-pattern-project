using System.Collections.Concurrent;
using game_server.Services;
using Microsoft.AspNetCore.SignalR;
using shared;

namespace game_server.Sockets;
public class PlayerHub : Hub
{
    public static readonly ConcurrentDictionary<string, CanvasObjectInfo> CurrentCanvasItems = new();
    private static readonly Random rnd = new();
    private readonly CoinBackgroundService _coinService;

    public PlayerHub(CoinBackgroundService coinService)
    {
        _coinService = coinService;
    }

    public async Task AddEntityToLobby(string name, RGB color, EntityType entityType)
    {   
        var info = new CanvasObjectInfo{
            EntityType = entityType,
            Color = color,
            Name = name,
            Uuid = entityType == EntityType.PLAYER ? Context.ConnectionId : Guid.NewGuid().ToString(),
            Location = new(rnd.Next((int)(Constants.MapWidth*0.9)),
                rnd.Next((int)(Constants.MapHeight*0.9)))
        };
        var freshJoined = CurrentCanvasItems.TryAdd(info.Uuid, info);
        var existingEntities = CurrentCanvasItems.Values.ToList();
        await Clients.Others.SendAsync("AddEntityToLobbyClient", info); //player is displayed for other online clients
        if(freshJoined && entityType == EntityType.PLAYER)
        {
            foreach(var entity in existingEntities) 
                await Clients.Caller.SendAsync("AddEntityToLobbyClient", entity); //all items are displayed for the new player
        }
        else await Clients.Caller.SendAsync("AddEntityToLobbyClient", info);
    }

    public async Task ProjectileShot(Vector2 direction)
    {
        var playerInfo = CurrentCanvasItems[Context.ConnectionId];
        await Clients.All.SendAsync("UpdateOnProjectileInClient", direction, playerInfo.Location);
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
        playerInfo.Location = new(newX, newY);

        await Clients.All.SendAsync("UpdateEntityPositionInClient", playerInfo.Location, Context.ConnectionId);
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        CurrentCanvasItems.TryRemove(Context.ConnectionId, out _);
        Clients.Others.SendAsync("RemoveObjectFromCanvas", Context.ConnectionId);
        Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} disconnected from gamer server chat socket.");
        return base.OnDisconnectedAsync(exception);
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