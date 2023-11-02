using game_client.Adapters;
using game_client.Socket;
using game_client.Views;
using System;
using Avalonia.Input;
using shared;

namespace game_client.Models
{
    public class GameFacade
    {
        private readonly MainWindow _mainWindow;
        private WeaponType currentWeapon;
        private PlayerPixel currentPlayer;
        public GameFacade(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void JoinAndStartGame(string name, WeaponType selectedWeapon)
        {
            if (string.IsNullOrEmpty(name))
                return;
            currentWeapon = selectedWeapon;
            var _socketService = SocketService.GetInstance();
            var _game = Game.GetInstance();
            
            // Reset the coin count and update the coin counter
            _socketService.ResetCoinCount();
            _socketService.UpdateCoinCounter();

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
        public void SetCurrentPlayer(PlayerPixel player)
        {
            currentPlayer = player;
        }
        public void setCurrentWeapon(WeaponType weapon)
        {
            currentWeapon = weapon;
        }
        public void HandleKeyDown(Key key)
        {
            InputHandler.AddKey(key);
        }

        public void HandleKeyUp(Key key)
        {
            InputHandler.RemoveKey(key);
        }

        public async void SendShootingCords(Vector2 position)
        {
            currentPlayer.Shoot(position);
        }
    }
}
