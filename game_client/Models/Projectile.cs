using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models;

public class Projectile : GameObject, ICloneable
{
    private Ellipse Shape;

    public Color Color {
        get => ((SolidColorBrush)Shape.Fill).Color;
        set => Shape.Fill = new SolidColorBrush(value);
    }
    public Projectile(Vector2 location, Color color) : base(location)
    {
        Shape = new Ellipse
        {
            Fill = new SolidColorBrush(color),
            Width = 8,
            Height = 8
        };
        AddToStackPanel(Shape);
    }

    public ICloneable Clone() {
        return new Projectile(this.Location, Colors.Blue);
    }

    public ICloneable ShallowClone() {
        return (ICloneable)this.MemberwiseClone();
    }
    public ICloneable DeepCopy() {
        var clone = new Projectile(this.Location, Colors.Red) {
    };
        return clone;
    }


}
