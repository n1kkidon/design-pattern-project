using System;
using Avalonia.Controls;
using shared;

namespace game_client.Models.CanvasItems;

public class Obstacle : GameObject
{
    public Image _image;
    private static readonly ImageFlyweightFactory _imageFactory = new ImageFlyweightFactory();
    private readonly string _imagePath;

    public Obstacle(Vector2 location, string imagePath) : base(location)
    {
        _imagePath = imagePath;
        _image = new Image();
    }

    public void LoadImage()
    {
        if (_image.Source == null)
        {
            _image.Source = _imageFactory.GetImage(_imagePath);

            if (_image.Source != null)
            {
                _image.Width = Constants.ObstacleDimensionsTree.X;
                _image.Height = Constants.ObstacleDimensionsTree.Y;
                AddToStackPanel(_image);
            }
            else
            {
                Console.WriteLine("Error: Image could not be loaded.");
            }
        }
            
        Console.WriteLine(new Vector2(GetWidth(), GetHeight()));
    }
}

// Decorator abstract class
// public abstract class ObstacleDecorator : Obstacle
// {
//     protected Obstacle _decoratedObstacle;
//     public ObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle.Location)
//     {
//         _decoratedObstacle = decoratedObstacle;
//     }
// }
// public class IndestructibleObstacleDecorator : ObstacleDecorator
// {
//     public IndestructibleObstacleDecorator(Obstacle decoratedObstacle) : base(decoratedObstacle)
//     {
//         MarkAsIndestructible();
//     }

//     private void MarkAsIndestructible()
//     {
//         _shape.Fill = new SolidColorBrush(Colors.Green);
//         _shape.Stroke = new SolidColorBrush(Colors.Red);
//         _shape.StrokeThickness = 2;
//     }
// }
