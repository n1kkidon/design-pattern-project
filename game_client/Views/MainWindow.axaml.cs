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

        // Generate a random System.Drawing.Color
        Random rnd = new Random();
        System.Drawing.Color randomDrawingColor = System.Drawing.Color.FromArgb(
            rnd.Next(256), rnd.Next(256), rnd.Next(256));

        Avalonia.Media.Color randomAvaloniaColor = ColorAdapter.ToAvaloniaColor(randomDrawingColor);

        var socketService = SocketService.GetInstance();
        socketService.JoinGameLobby(name, randomAvaloniaColor).Wait();

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
