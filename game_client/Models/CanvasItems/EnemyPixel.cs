using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using game_client.Composite;
using shared;

namespace game_client.Models.CanvasItems;
public class EnemyPixel : GameObject, IGameComponent
{
    private TextBlock NameTag;
    protected Ellipse Pixel;
    private string _name;
    private int _size = 16;
    public EnemyPixel(string name, Color color, Vector2 location, int health) : base(location){ //initial location 
        _name = name;
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
}