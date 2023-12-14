using Avalonia.Controls.Shapes;
using Avalonia.Media;
using shared;

namespace game_client.Models.CanvasItems;

public class Projectile : GameObject, ICloneable
{
    private Ellipse Shape;
    private int height = 0;
    private int width = 0;
    private bool isFinalized = false;
    private int explosionRange = 0;
    public Color Color {
        get => ((SolidColorBrush)Shape.Fill).Color;
        set => Shape.Fill = new SolidColorBrush(value);
    }
    
    public Projectile(Vector2 location, Color color) : base(location)
    {
        Shape = new Ellipse
        {
            Fill = new SolidColorBrush(color),
            Width = 20,
            Height = 20
        };
        AddToStackPanel(Shape);
    }
    public Projectile(Vector2 location, Color color, int width, int height) : base(location)
    {
        this.height = height;
        this.width = width;
        Shape = new Ellipse
        {
            Fill = new SolidColorBrush(color),
            Width = width,
            Height = height
        };
        AddToStackPanel(Shape);
    }

    public void setFinalized (bool isFinalized)
    {
        this.isFinalized = isFinalized;
    }

    public void setExplosionRange (int explosionRange)
    {
        this.explosionRange = explosionRange;
    }

    public ICloneable Clone() {
        return new Projectile(this.Location, this.Color, this.height, this.width);
    }

    public ICloneable ShallowClone() {
        return (ICloneable)this.MemberwiseClone();
    }
    public ICloneable DeepCopy() {
        var clone = new Projectile(this.Location, this.Color, this.height, this.width) {
    };
        return clone;
    }


}
