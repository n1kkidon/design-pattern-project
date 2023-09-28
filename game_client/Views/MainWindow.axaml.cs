using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using game_client.Socket;
using shared;

namespace game_client.Views;

public partial class MainWindow : Window, INotifyPropertyChanged
{
    private int playerCoins = 0;
    private TextBlock? coinCounter;
    private static MainWindow? _instance;
    private SocketService? socketService;
    private readonly Dictionary<string, CoinView> coinViews = new();
    private readonly Dictionary<string, PlayerPixel> ConnectedPlayers = new();


    public static MainWindow GetInstance(){
        _instance ??= new();
        return _instance;
    }
    public MainWindow()
    {
        InitializeComponent();
        this.coinCounter = this.FindControl<TextBlock>("coinCounter");
        if (coinCounter != null)
        {
            coinCounter.Text = playerCoins.ToString();
        }
    }

    private void OnJoinButtonClick(object sender, RoutedEventArgs e)
    {
        var name = nameField.Text;
        if(name == null)
            return;
        canvas.Children.Remove(joinButton);
        canvas.Children.Remove(nameField);

        Random rnd = new();
        socketService = new("http://localhost:5000"); //TODO: maybe make a config.json
        socketService.OnAddCoinToMap((coin, coinId) =>
        {
            Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
            {
                var coinView = new CoinView(coin.X, coin.Y);
                canvas.Children.Add(coinView.CoinObject);
                coinViews[coinId] = coinView;  // Store the coinView with its ID
            });
        });


        socketService.JoinGameLobby(name, Color.FromRgb((byte)rnd.Next(255), 
                                                        (byte)rnd.Next(255), 
                                                        (byte)rnd.Next(255))).Wait();
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