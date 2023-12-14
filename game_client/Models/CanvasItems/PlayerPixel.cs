using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using game_client.Models.PlayerState;
using game_client.Observer;
using shared;

namespace game_client.Models.CanvasItems;

public class PlayerPixel : GameObject, ISubject
{
    private IPlayerState _currentState;
    private readonly TextBlock NameTag;
    private readonly Image PlayerAvatar; 
    private readonly Rectangle HealthBar;
    public ShootAlgorithm? ShootAlgorithm { get; private set; }

    private int health;

    //TODO: remove colors
    public PlayerPixel(string name, Color color, Vector2 location, int health) : base(location)
    { 
        this.health = health;
        NameTag = new()
        {
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        PlayerAvatar = new()
        {
            Width = Constants.PlayerDimensions.X,
            Height = Constants.PlayerDimensions.Y
        };

        try
        {
            PlayerAvatar.Source = new Bitmap("./Assets/player.png");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading player image: {ex.Message}");
        }

        AddToStackPanel(NameTag, true);
        AddToStackPanel(PlayerAvatar);

        HealthBar = new()
        {
            Width = 30,
            Height = 10,
            Fill = Brushes.Green,
        };
        AddToStackPanel(HealthBar, true);

        new HealthBarObserver(this, HealthBar).Update(health);
            
        _currentState = new NormalState(this);
    }

    private readonly List<IObserver> observers = new List<IObserver>();
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

    public void DecreaseHealth(int amount)
    {
        if (health - amount >= 0)
        {
            health-=amount;
        }
        else
        {
            health = 0;
            Console.WriteLine("DEAD"); //TODO: make him actually dead
        }
            
        NotifyObservers();
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
        ShootAlgorithm = shootAlgo;
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