using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using shared;
using System.Threading;
using System.Threading.Tasks;

public class CoinBackgroundService : BackgroundService
{
    private readonly IHubContext<PlayerHub> _hubContext;
    private readonly Random _random = new Random();

    public CoinBackgroundService(IHubContext<PlayerHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var coin = new Coin
            {
                X = _random.Next((int)(Constants.MapWidth * 0.9)),
                Y = _random.Next((int)(Constants.MapHeight * 0.9))
            };

            var coinId = Guid.NewGuid().ToString();
            await _hubContext.Clients.All.SendAsync("AddCoinToMap", coin, coinId);
            await Task.Delay(TimeSpan.FromSeconds(_random.Next(10, 21)), stoppingToken);
        }
    }
}
