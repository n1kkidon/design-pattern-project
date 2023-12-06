using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;
using game_client.Composite;
using System;
using game_client.Socket;
using Avalonia.Threading;

namespace game_client.Models;
public class EnemyPixel : GameObject, IGameComponent
{
    private TextBlock NameTag;
    protected Ellipse Pixel;
    private string _name;
    private int _size;
    private Vector2 _position;
    public EnemyPixel(string name, Color color, Vector2 location) : base(location){ //initial location 
        _name = name;
        _size = 16;
        _position = location;
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
    }

    public void Operation()
    {
        Console.WriteLine($"{_name} of size {_size} at {_position} is performing an operation.");
    }

    public bool IsComposite() => false;

    public void IncreaseSize()
    {
        _size++; // Increase size by 1
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            Pixel.Width = _size;
            Pixel.Height = _size;
        });
        Console.WriteLine($"{_name} increased to size {_size}.");
    }

    public void ChangePosition()
    {
        // Move the enemy by one pixel in a random direction
        Random rnd = new Random();
        int direction = rnd.Next(4);
        switch (direction)
        {
            case 0: _position.X++; break; // Right
            case 1: _position.X--; break; // Left
            case 2: _position.Y++; break; // Down
            case 3: _position.Y--; break; // Up
        }
        Console.WriteLine($"{_name} moved to new position {_position}.");
    }
}