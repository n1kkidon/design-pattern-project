using System.Drawing;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using game_client.Models;
using shared;
using System.Security.Cryptography.X509Certificates;
using System.Net.Sockets;
using System.IO;
using System;
using game_client.Socket;

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
    private WeaponType _weaponType;
    private bool gameStarted;

    public MainWindow()
    {
        InitializeComponent();
        _gameFacade = new GameFacade(this);
        var socketService = SocketService.GetInstance();
        socketService.OnPlayerCreated += HandlePlayerCreated;
        gameStarted = false;
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        _gameFacade.JoinAndStartGame(name, _weaponType);
        weaponSelectionPanel.IsVisible = false;
        if (string.IsNullOrEmpty(name))
            return;
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
    private void HandlePlayerCreated(PlayerPixel player)
    {
        _gameFacade.SetCurrentPlayer(player, _weaponType);
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
            _weaponType = Enum.Parse<WeaponType>(radioButton.Content.ToString(), true);
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        _gameFacade.HandleKeyUp(e.Key);
        base.OnKeyUp(e);
    }

}
