using System;
using Avalonia.Controls;
using System.Collections.Generic;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Composite;
using game_client.Observer;
using shared;

namespace game_client.Models.CanvasItems;
public class EnemyPixel : GameObject, IGameComponent
{
    private TextBlock NameTag;
    protected Ellipse Pixel;
    private string _name;
    private readonly Rectangle HealthBar;
    private int _size = 16;
    private int health;
    public EnemyPixel(string name, Color color, Vector2 location, int health) : base(location){ 
        _name = name;
        this.health = health;
        NameTag = new(){
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Pixel = new(){
            Fill = new SolidColorBrush(color),
            Width = 16,
            Height = 16
        };
        AddToStackPanel(Pixel);
        AddToStackPanel(NameTag, true);

        HealthBar = new()
        {
            Width = 30,
            Height = 10,
            Fill = Brushes.Green,
        };
        AddToStackPanel(HealthBar, true);

        new EnemyHealthBarObserver(this, HealthBar).Update(health);
        
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
    }

    public void Operation()
    {
        Console.WriteLine($"{_name} of size {_size} is performing an operation.");
    }

    public bool IsComposite() => false;

    public void IncreaseSize()
    {
        _size += 10; // Increase size by 1
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Pixel.Width = _size;
            Pixel.Height = _size;
            Pixel.InvalidateVisual();
        });
        Console.WriteLine($"{_name} increased to size {_size}.");
    }

    public void ShapeShift()
    {
        // Move the enemy by one pixel in a random direction
        Random rnd = new Random();
        int direction = rnd.Next(4);


        Dispatcher.UIThread.InvokeAsync(() =>
        {
            switch (direction)
            {
                case 0:
                    {
                        NameTag.Text = "Chameleon";
                        NameTag.Foreground = new SolidColorBrush(Colors.ForestGreen);
                        break;
                    }
                case 1:
                    {
                        NameTag.Text = "Chameleon";
                        NameTag.Foreground = new SolidColorBrush(Colors.Brown);
                        break;
                    }
                case 2:
                    {
                        NameTag.Text = "Chameleon";
                        NameTag.Foreground = new SolidColorBrush(Colors.Cyan);
                        break;
                    }
                case 3:
                    {
                        NameTag.Text = "Chameleon";
                        NameTag.Foreground = new SolidColorBrush(Colors.Violet);
                        break;
                    }
            }
            Pixel.InvalidateVisual();
        });
    }

    public void Accept(IHealthUpdateVisitor visitor, int amount)
    {
        visitor.Visit(this, amount);
    }
}