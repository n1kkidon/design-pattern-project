using System;
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

        Random rnd = new();
        var socketService = SocketService.GetInstance();
        socketService.JoinGameLobby(name, Color.FromRgb((byte)rnd.Next(255), 
                                                        (byte)rnd.Next(255), 
                                                        (byte)rnd.Next(255))).Wait();


        //keymaps
        InputHandler.SetCommand(Key.W, new MoveUpCommand());
        InputHandler.SetCommand(Key.A, new MoveLeftCommand());
        InputHandler.SetCommand(Key.S, new MoveDownCommand());
        InputHandler.SetCommand(Key.D, new MoveRightCommand());

        var game = new Game(); //tickrate
        game.Start();
        //KeyDown += async (sender, e) => await InputHandler.HandleInput(e.Key);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        Keyboard.Keys.Add(e.Key);
        base.OnKeyUp(e);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        Keyboard.Keys.Remove(e.Key);
        base.OnKeyUp(e);
    }
}