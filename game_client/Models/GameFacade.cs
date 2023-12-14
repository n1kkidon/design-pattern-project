using game_client.Adapters;
using game_client.Socket;
using game_client.Views;
using System;
using Avalonia.Input;
using Avalonia.Threading;
using shared;
using game_client.ChainOfResponsibility;
using game_client.Mediator;
using game_client.Models.CanvasItems;

namespace game_client.Models
{
    public class GameFacade
    {
        private readonly MainWindow _mainWindow;
        private PlayerPixel currentPlayer;
        private IPlayerJoinHandler _joinChain;
        public WeaponType WeaponType { get; set; }

        public GameFacade(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            SetupJoinChain();
        }

        private void HandlePlayerCreated(PlayerPixel player, IMediator mediator, int coinCount)
        {
            SetCurrentPlayer(player, WeaponType, mediator);
            UpdateCoinCounter(coinCount);
        }
        
        private void SetupJoinChain()
        {
            _joinChain = new NameValidationHandler();
            
            _joinChain.SetNext(new WeaponValidationHandler())
                .SetNext(new UiChangeHandler(_mainWindow));
        }

        public void JoinAndStartGame(string? name)
        {
            var request = new PlayerJoinRequest { Name = name, SelectedWeapon = WeaponType };
            _joinChain.Handle(request);

            if (!request.IsValid)
            {
                return;
            }
            
            
            var socketService = new SocketService();
            var game = new Game();
            var mediator = new ConcreteMediator(socketService, game, _mainWindow);
                
            socketService.OnPlayerCreated += HandlePlayerCreated;

            // Generate a random color
            Random rnd = new Random();
            System.Drawing.Color randomDrawingColor = System.Drawing.Color.FromArgb(
                rnd.Next(256), rnd.Next(256), rnd.Next(256));
            Avalonia.Media.Color randomAvaloniaColor = ColorAdapter.ToAvaloniaColor(randomDrawingColor);

            socketService.JoinGameLobby(name!, randomAvaloniaColor, WeaponType).Wait();
            socketService.AddOpponentToGame().Wait();

            //keymaps
            InputHandler.SetCommand(Key.W, new MoveUpCommand(false, game));
            InputHandler.SetCommand(Key.A, new MoveLeftCommand(false, game));
            InputHandler.SetCommand(Key.S, new MoveDownCommand(false, game));
            InputHandler.SetCommand(Key.D, new MoveRightCommand(false, game));

            game.Start();
        }
        private void SetCurrentPlayer(PlayerPixel player, WeaponType weaponType, IMediator mediator)
        {
            currentPlayer = player;
            switch (weaponType)
            {
                case WeaponType.PISTOL:
                    {
                        currentPlayer.SetShootingAlgorithm(new Pistol(mediator));
                        break;
                    }
                case WeaponType.ROCKET:
                    {
                        currentPlayer.SetShootingAlgorithm(new Rocket(mediator));
                        break;
                    }
                case WeaponType.SNIPER:
                    {
                        currentPlayer.SetShootingAlgorithm(new Sniper(mediator));
                        break;
                    }
                case WeaponType.CANNON:
                    {
                        currentPlayer.SetShootingAlgorithm(new Cannon(mediator));
                        break;
                    }
                default:
                    {
                        currentPlayer.SetShootingAlgorithm(new Pistol(mediator));
                        break;
                    }
            }
        }
        public void HandleKeyDown(Key key)
        {
            InputHandler.AddKey(key);
        }

        public void HandleKeyUp(Key key)
        {
            InputHandler.RemoveKey(key);
        }

        public void SendShootingCords(IVector2 position)
        {
            currentPlayer.Shoot(position);
        }
        
        public void UpdateCoinCounter(int count)
        {
            // Update the UI with the current coin count
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                _mainWindow.coinCounter.Text = $"Coins: {count}";
            });
        }
    }
}
