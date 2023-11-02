using System.Drawing;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using game_client.Models;
using shared;

namespace game_client.Views;

public partial class MainWindow : Window
{
    private static MainWindow? _instance;
    public static MainWindow GetInstance()
    {
        _instance ??= new();
        return _instance;
    }
    private readonly GameFacade _gameFacade;
    private bool gameStarted;

    public MainWindow()
    {
        InitializeComponent();
        _gameFacade = new GameFacade(this);
        gameStarted = false;
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        if (string.IsNullOrEmpty(name))
            return;
        _gameFacade.JoinAndStartGame(name);
        canvas.Children.Remove(joinButton);
        canvas.Children.Remove(nameField);
        gameStarted = true;
    }

    private void OnMouseClick(object sender, PointerPressedEventArgs e)
    {
        if(!gameStarted)
            return;
        var clickPos = e.GetPosition(canvas);
        var pnt = new Point((int)clickPos.X, (int)clickPos.Y);
        var pointAdapter = new PointAdapter(pnt);
        //var position = new Vector2((float)clickPos.X, Constants.MapHeight-(float)clickPos.Y);
        
        _gameFacade.SendShootingCords(pointAdapter);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        _gameFacade.HandleKeyDown(e.Key);
        base.OnKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _gameFacade.HandleKeyUp(e.Key);
        base.OnKeyUp(e);
    }
}
