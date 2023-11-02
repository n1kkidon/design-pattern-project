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
        // Initialize the shape or other properties of the obstacle
        _shape = new Rectangle
        {
            // Set the properties for the obstacle shape
            Width = 20,
            Height = 100,
            Fill = new SolidColorBrush(Colors.Gray)
        };

        AddToStackPanel(_shape);
    }

    // Additional functionality specific to the obstacle can be added here
}

// Decorator abstract class
public abstract class ObstacleDecorator : Obstacle
{
    protected Obstacle _decoratedObstacle;

    public ObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle.Location)
    {
        _decoratedObstacle = decoratedObstacle;
    }

    // Override methods from the Obstacle class if needed to add new behavior
}

// Concrete Decorator example
public class IndestructibleObstacleDecorator : ObstacleDecorator
{
    public IndestructibleObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle)
    {
        // Modify the obstacle to indicate danger
        MarkAsIndestructible();
    }

    private void MarkAsIndestructible()
    {
        // Change the color of the obstacle to red to indicate danger
        // Assuming _shape is a Shape object from the base Obstacle class
        _shape.Fill = new SolidColorBrush(Colors.Gray);
        _shape.Stroke = new SolidColorBrush(Colors.Red);
        _shape.StrokeThickness = 2;
    }
}
