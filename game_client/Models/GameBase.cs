using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using game_client.Mediator;

namespace game_client.Models;

public abstract class GameBase : BaseComponent
{
    public const int TicksPerSecond = 60;
    private readonly DispatcherTimer _timer = new() { Interval = new TimeSpan(0, 0, 0, 0, 1000 / TicksPerSecond) };

    protected GameBase()
    {
        _timer.Tick += async delegate { await DoTick(); };
    }

    public long CurrentTick { get; private set; }


    private async Task DoTick()
    {
        await Tick();
        CurrentTick++;
    }

    protected abstract Task Tick();

    public void Start()
    {
        _timer.IsEnabled = true;
    }

    public void Stop()
    {
        _timer.IsEnabled = false;
    }
}
