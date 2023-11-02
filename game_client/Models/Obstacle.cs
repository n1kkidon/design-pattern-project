using Avalonia.Controls.Shapes;
using Avalonia.Media;

using shared;

namespace game_client.Models;

// Obstacle base class
public class Obstacle : GameObject
{
    public Shape _shape;

    public Obstacle(Vector2 location) : base(location)
    {
        _shape = new Rectangle
        {
            // Set the properties for the obstacle shape
            Width = 20,
            Height = 100,
            Fill = new SolidColorBrush(Colors.Gray)
        };

        AddToStackPanel(_shape);
    }

}

// Decorator abstract class
public abstract class ObstacleDecorator : Obstacle
{
    protected Obstacle _decoratedObstacle;

    public ObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle.Location)
    {
        _decoratedObstacle = decoratedObstacle;
    }

}
public class IndestructibleObstacleDecorator : ObstacleDecorator
{
    public IndestructibleObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle)
    {
        MarkAsIndestructible();
    }

    private void MarkAsIndestructible()
    {
        _shape.Fill = new SolidColorBrush(Colors.Gray);
        _shape.Stroke = new SolidColorBrush(Colors.Red);
        _shape.StrokeThickness = 2;
    }
}
