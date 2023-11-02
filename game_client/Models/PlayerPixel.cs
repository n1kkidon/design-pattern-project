using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using game_client.Observer;
using shared;
using System.Collections.Generic;
using game_client.Socket;

namespace game_client.Models;

public class PlayerPixel : GameObject, ISubject
{
    private TextBlock NameTag;
    private Rectangle Pixel;
    private Rectangle HealthBar;
    private ShootAlgorithm _shootAlgorithm;
    SocketService _socketService = SocketService.GetInstance();

    private int health = 10;

    private List<IObserver> observers = new();

    public PlayerPixel(string name, Color color, Vector2 location) : base(location)
    {
        _shootAlgorithm = new Pistol(_socketService); 
        NameTag = new()
        {
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Pixel = new()
        {
            Fill = new SolidColorBrush(color),
            Width = 16,
            Height = 16
        };
        AddToStackPanel(NameTag, true);
        AddToStackPanel(Pixel);

        HealthBar = new Rectangle
        {
            Width = 30,
            Height = 10,
            Fill = Brushes.Green,
        };
        AddToStackPanel(HealthBar, true);

        new HealthBarObserver(this, HealthBar);
    }

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health--;
            NotifyObservers();
        }
    }

    public void SetShootingAlgorithm(ShootAlgorithm shootAlgo)
    {
        _shootAlgorithm = shootAlgo;
    }

    // Method to get the current shooting algorithm
    public ShootAlgorithm GetShootAlgorithm()
    {
        return _shootAlgorithm;
    }

    public async void Shoot(Vector2 position)
    {
        await _shootAlgorithm.Shoot(position);
    }

    public void RegisterObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void NotifyObservers()
    {
        foreach (var observer in observers)
        {
            observer.Update(health);
        }
    }
}
