using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using game_client.Models;
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
    public MainWindow() //this will be private when the IDE preview window is no longer needed
    {
        InitializeComponent();
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        if (name == null)
            return;

        // Reset the coin count and update the coin counter
        SocketService.GetInstance().ResetCoinCount();
        SocketService.GetInstance().UpdateCoinCounter();

        canvas.Children.Remove(joinButton);
        canvas.Children.Remove(nameField);

        Random rnd = new();
        var socketService = SocketService.GetInstance();
        socketService.JoinGameLobby(name, Color.FromRgb((byte)rnd.Next(255),
                                                        (byte)rnd.Next(255),
                                                        (byte)rnd.Next(255))).Wait();

        socketService.AddOpponentToGame().Wait();

        var game = new Game(); //tickrate
        //keymaps
        InputHandler.SetCommand(Key.W, new MoveUpCommand(false, game));
        InputHandler.SetCommand(Key.A, new MoveLeftCommand(false, game));
        InputHandler.SetCommand(Key.S, new MoveDownCommand(false, game));
        InputHandler.SetCommand(Key.D, new MoveRightCommand(false, game));

        
        game.Start();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        InputHandler.AddKey(e.Key);
        base.OnKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        InputHandler.RemoveKey(e.Key);
        base.OnKeyUp(e);
    }
}
