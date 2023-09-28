using System.Collections.Concurrent;
using System.Drawing;
using Microsoft.AspNetCore.SignalR;
using shared;
using Microsoft.Extensions.DependencyInjection;

public class PlayerHub : Hub
{
    private static readonly ConcurrentDictionary<string, PlayerInfo> ConnectedUsers = new();
    private static readonly Random rnd = new();
    private static readonly ConcurrentDictionary<string, OpponentInfo> ConnectedOpponents = new();

    public async Task AddPlayerToLobby(string name, RGB color)
    {   
        var info = new PlayerInfo{
            Color = color,
            Name = name,
            Uuid = Context.ConnectionId,
            X = rnd.Next((int)(Constants.MapWidth*0.9)),
            Y = rnd.Next((int)(Constants.MapHeight*0.9))
        };
        ConnectedUsers.TryAdd(Context.ConnectionId, info);
        var existingplrs = ConnectedUsers.Values.ToList();
        await Clients.Others.SendAsync("AddPlayerToLobbyClient", info);
        foreach(var plr in existingplrs)
            await Clients.Caller.SendAsync("AddPlayerToLobbyClient", plr);
    }
    public async Task AddOpponentToGame()
    {
        var opponentInfo = new OpponentInfo
        {
            Name = "Opponent1",
            Color = new RGB(255, 0, 0),
            Uuid = Context.ConnectionId,
            X = 100,
            Y = 100
        };
        ConnectedOpponents.TryAdd(opponentInfo.Uuid, opponentInfo);
        await Clients.All.SendAsync("AddOpponentToGameClient", opponentInfo);
    }


    public async Task ClientMoved(Direction direction)
    {
        await Clients.All.SendAsync("UpdateClientPosition", direction, Context.ConnectionId);
        //syncing location here for when a new player joins, 
        //we know the correct coords of others to send it to him.
        var playerInfo = ConnectedUsers[Context.ConnectionId];
        switch(direction){
            case Direction.UP: 
                playerInfo.Y -= Constants.MoveStep;
                break;
            case Direction.DOWN:
                playerInfo.Y += Constants.MoveStep;
                break;
            case Direction.LEFT:
                playerInfo.X -= Constants.MoveStep;
                break;
            case Direction.RIGHT:
                playerInfo.X += Constants.MoveStep;
                break;
        }
    }


    public Task ServerMessageReceiver(string message)
    {
        var ctx = Context.GetHttpContext();
        Console.WriteLine($"Message received from {ctx?.Connection.RemoteIpAddress}: {message}");
        Clients.Others.SendAsync("SendMessageToClient", $"{ctx?.Connection.RemoteIpAddress}: {message}");
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedUsers.TryRemove(Context.ConnectionId, out _);
        Clients.Others.SendAsync("RemoveDisconnectedPlayer", Context.ConnectionId);
        Console.WriteLine($"Client from {Context.GetHttpContext()?.Connection.RemoteIpAddress} disconnected from gamer server chat socket.");
        return base.OnDisconnectedAsync(exception);
    }
    public override Task OnConnectedAsync()
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

    private readonly CoinBackgroundService _coinService;

    public PlayerHub(CoinBackgroundService coinService)
    {
        _coinService = coinService;
    }
    public async Task PickupCoin(string coinId)
    {
        Console.WriteLine("Picked up coinas");
        await _coinService.PickupCoin(coinId);
    }
}