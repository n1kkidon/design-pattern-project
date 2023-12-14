using System.Drawing;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using game_client.Models;
using shared;
using System;


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
        _gameFacade.JoinAndStartGame(name);
        gameStarted = true;
    }

    private void OnMouseClick(object sender, PointerPressedEventArgs e)
    {
        if(!gameStarted)
            return;
        
        var clickPos = e.GetPosition(canvas);
        var pnt = new Point((int)clickPos.X, (int)clickPos.Y);
        var pointAdapter = new PointAdapter(pnt);
        
        _gameFacade.SendShootingCords(pointAdapter);
    }
    protected override void OnKeyDown(KeyEventArgs e)
    {
        _gameFacade.HandleKeyDown(e.Key);
        base.OnKeyUp(e);
    }
    public void OnWeaponChanged(object? sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true)
        {
            _gameFacade.WeaponType = Enum.Parse<WeaponType>(radioButton.Content.ToString(), true);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _gameFacade.HandleKeyUp(e.Key);
        base.OnKeyUp(e);
    }

}
