using System;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;

public class PlayerPixel : GameObject
{
    private TextBlock NameTag;
    private Rectangle Pixel;

    public PlayerPixel(string name, Color color, Vector2 location) : base(location){ //initial location 
        NameTag = new(){
            Foreground = new SolidColorBrush(Colors.White),
            Text = name,
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
        };
        Pixel = new(){
            Fill = new SolidColorBrush(color),
            Width = 15,
            Height = 15
        };
        stackPanel.Children.Add(NameTag);
        stackPanel.Children.Add(Pixel);
    }
}
