using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using game_client.Models;
using game_client.Socket;
using game_client.Adapters;

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

    public MainWindow()
    {
        InitializeComponent();
        _gameFacade = new GameFacade(this);
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        _gameFacade.JoinAndStartGame(name);
        canvas.Children.Remove(joinButton);
        canvas.Children.Remove(nameField);
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
