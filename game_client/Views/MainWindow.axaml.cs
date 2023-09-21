using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
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
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        if(name == null)
            return;
        canvas.Children.Remove(joinButton);
        canvas.Children.Remove(nameField);
        SocketService socketService = new();
        socketService.JoinGameLobby(name, Color.FromRgb(255, 255, 0)).Wait();
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