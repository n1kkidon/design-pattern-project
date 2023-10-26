using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;

public class Projectile : GameObject
{
    private Ellipse Shape;

    public Projectile(Vector2 location) : base(location)
    {
        Shape = new Ellipse
        {
            Fill = new SolidColorBrush(Colors.White),
            Width = 8,
            Height = 8
        };
        AddToStackPanel(Shape);
    }
}
