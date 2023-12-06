using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using game_client.Observer;
using shared;
using System.Collections.Generic;
using game_client.Socket;
using game_client.Models.PlayerState;

namespace game_client.Models;

public class PlayerPixel : GameObject, ISubject
{
    private IPlayerState _currentState;
    private TextBlock NameTag;
    private Rectangle Pixel;
    private Rectangle HealthBar;
    private ShootAlgorithm _shootAlgorithm;
    public ShootAlgorithm ShootAlgorithm => _shootAlgorithm;
    SocketService _socketService = SocketService.GetInstance();

    private int health = 10;

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

        // Set initial state
        _currentState = new NormalState(this);
    }

    private List<IObserver> observers = new();
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

    public void DecreaseHealth()
    {
        if (health > 0)
        {
            health--;
            NotifyObservers();
        }
        CheckForStateTransition();
    }

    private void CheckForStateTransition()
    {
        if (health <= 0 && !(_currentState is DeadState))
        {
            SetState(new DeadState(this));
        }
        else if (health < 7 && _currentState is NormalState)
        {
            SetState(new InjuredState(this));
        }
    }

    public void SetShootingAlgorithm(ShootAlgorithm shootAlgo)
    {
        _shootAlgorithm = shootAlgo;
    }

    public async void Shoot(IVector2 position)
    {
        _currentState.Shoot(position);
    }

    public void SetState(IPlayerState newState)
    {
        _currentState = newState;
    }
}
