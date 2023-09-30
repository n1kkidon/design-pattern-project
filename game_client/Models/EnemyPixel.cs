using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;
public class EnemyPixel : GameObject
{
    private TextBlock NameTag;
    private Ellipse Pixel;
    public EnemyPixel(string name, Color color, Vector2 location) : base(location){ //initial location 
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
        stackPanel.Children.Add(Pixel);
        stackPanel.Children.Add(NameTag);
    }
}