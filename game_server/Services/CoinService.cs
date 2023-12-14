using game_server.Sockets;
using Microsoft.AspNetCore.SignalR;
using shared;

namespace game_server.Services;

public class CoinBackgroundService : BackgroundService
{
    private readonly IHubContext<PlayerHub> _hubContext;
    private readonly Random _random = new();

    public CoinBackgroundService(IHubContext<PlayerHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var info = new CanvasObjectInfo
            {
                EntityType = EntityType.COIN,
                Location = new Vector2
                {
                    X = _random.Next((int)(Constants.MapWidth * 0.9)),
                    Y = _random.Next((int)(Constants.MapHeight * 0.9))
                },
                Uuid = Guid.NewGuid().ToString(),
            };
            PlayerHub.CurrentCanvasItems[info.Uuid] = info;
            PlayerHub.GameObjectsCollection.Add(info.Uuid, info);
            await _hubContext.Clients.All.SendAsync("AddEntityToLobbyClient", info);
            await Task.Delay(TimeSpan.FromSeconds(_random.Next(10, 21)), stoppingToken);
        }
    }

    public async Task PickupCoin(string coinId)
    {
        if (PlayerHub.CurrentCanvasItems.TryRemove(coinId, out _))
        {
            PlayerHub.RemoveIteratorObject(coinId);
            await _hubContext.Clients.All.SendAsync("RemoveObjectFromCanvas", coinId);
        }
    }
}
