using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using game_client.Socket;
using shared;

namespace game_client.Views;

public partial class MainWindow : Window
{
    private static MainWindow? _instance;
    public static MainWindow GetInstance(){
        _instance ??= new();
        return _instance;
    }
    private MainWindow()
    {
        InitializeComponent();
        SocketService socketService = new();
        //var player = new PlayerPixel("Antanas", Color.FromRgb(255, 255, 0), 50, 50);
        //PlayerPixel player1 = new("Zack", Color.FromRgb(15, 255, 200), Width, Height);
        
        //canvas.Children.Add(player.PlayerObject);
        socketService.JoinGameLobby("forsen", Color.FromRgb(255, 255, 0)).Wait();
        KeyDown += async (sender, e) =>
        {
            switch (e.Key)
            {
                case Key.W: // Move up
                    await socketService.OnCurrentPlayerMove(Direction.UP);
                    break;
                case Key.S: // Move down
                    await socketService.OnCurrentPlayerMove(Direction.DOWN);
                    break;
                case Key.A: // Move left
                    await socketService.OnCurrentPlayerMove(Direction.LEFT);
                    break;
                case Key.D: // Move right
                    await socketService.OnCurrentPlayerMove(Direction.RIGHT);
                    break;
            }
        };
    }
}