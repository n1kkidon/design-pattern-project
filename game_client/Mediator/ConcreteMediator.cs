using System;
using System.Threading.Tasks;
using Avalonia.Threading;
using game_client.Models;
using game_client.Socket;
using game_client.Views;
using shared;

namespace game_client.Mediator;

public class ConcreteMediator : IMediator
{
    private readonly SocketService _socketService;
    private readonly Game _game;
    private readonly MainWindow _mainWindow;

    public ConcreteMediator(SocketService socketService, Game game, MainWindow mainWindow)
    {
        _socketService = socketService;
        _game = game;
        
        _mainWindow = mainWindow;
        _socketService.SetMediator(this);
        _game.SetMediator(this);
    }


    public async Task Notify(BaseComponent sender, string ev, object? args)
    {
        if (sender == _socketService)
        {
            switch (ev)
            {
                case "SubscribeToGame_OnTick":
                    if(args != null)
                        _game.OnTick += (Action)args;
                    break;
                case "UnsubscribeFromGame_OnTick":
                    if(args != null)
                        _game.OnTick -= (Action)args;
                    break;
                case "UpdateCoinCounter":
                    if (args != null)
                    {
                        Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            _mainWindow.coinCounter.Text = $"Coins: {(int)args}";
                        });
                    }
                    break;
            }
        }
        else if (sender == _game && ev.Equals("OnCurrentPlayerMove"))
        {
            if(args != null)
                await _socketService.OnCurrentPlayerMove((Vector2)args); 
        }
        else if (sender is ShootAlgorithm && ev.Equals("OnCurrentPlayerShoot"))
        {
            if (args != null)
                await _socketService.OnCurrentPlayerShoot(((ProjectileShootArgs)args).ClickPoint,
                    ((ProjectileShootArgs)args).WeaponType);
        }
    }
}