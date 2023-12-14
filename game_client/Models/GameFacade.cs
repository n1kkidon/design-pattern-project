using game_client.Adapters;
using game_client.Socket;
using game_client.Views;
using System;
using Avalonia.Input;
using shared;
using game_client.ChainOfResponsability;

namespace game_client.Models
{
    public class GameFacade
    {
        private readonly MainWindow _mainWindow;
        private WeaponType currentWeapon;
        private PlayerPixel currentPlayer;
        private IPlayerJoinHandler _joinChain;

        public GameFacade(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
            SetupJoinChain();
        }

        private void SetupJoinChain()
        {
            _joinChain = new NameValidationHandler();
            var weaponHandler = new WeaponValidationHandler();
            _joinChain.SetNext(weaponHandler);
        }

        public void JoinAndStartGame(string name, WeaponType selectedWeapon)
        {
            var request = new PlayerJoinRequest { Name = name, SelectedWeapon = selectedWeapon };
            _joinChain.Handle(request);

            if (!request.IsValid)
            {
                return;
            }

            var _socketService = SocketService.GetInstance();
            var _game = Game.GetInstance();
            
            // Reset the coin count and update the coin counter
            _socketService.ResetCoinCount();
            //_socketService.UpdateCoinCounter();

            // Generate a random color
            Random rnd = new Random();
            System.Drawing.Color randomDrawingColor = System.Drawing.Color.FromArgb(
                rnd.Next(256), rnd.Next(256), rnd.Next(256));
            Avalonia.Media.Color randomAvaloniaColor = ColorAdapter.ToAvaloniaColor(randomDrawingColor);

            _socketService.JoinGameLobby(name, randomAvaloniaColor, currentWeapon).Wait();
            _socketService.AddOpponentToGame().Wait();

            //keymaps
            InputHandler.SetCommand(Key.W, new MoveUpCommand(false, _game));
            InputHandler.SetCommand(Key.A, new MoveLeftCommand(false, _game));
            InputHandler.SetCommand(Key.S, new MoveDownCommand(false, _game));
            InputHandler.SetCommand(Key.D, new MoveRightCommand(false, _game));

            _game.Start();
        }
        public void SetCurrentPlayer(PlayerPixel player, WeaponType weaponType)
        {
            var _socketService = SocketService.GetInstance();
            currentPlayer = player;
            switch (weaponType)
            {
                case WeaponType.PISTOL:
                    {
                        currentPlayer.SetShootingAlgorithm(new Pistol(_socketService));
                        break;
                    }
                case WeaponType.ROCKET:
                    {
                        currentPlayer.SetShootingAlgorithm(new Rocket(_socketService));
                        break;
                    }
                case WeaponType.SNIPER:
                    {
                        currentPlayer.SetShootingAlgorithm(new Sniper(_socketService));
                        break;
                    }
                case WeaponType.CANNON:
                    {
                        currentPlayer.SetShootingAlgorithm(new Cannon(_socketService));
                        break;
                    }
                default:
                    {
                        currentPlayer.SetShootingAlgorithm(new Pistol(_socketService));
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
    }
}
