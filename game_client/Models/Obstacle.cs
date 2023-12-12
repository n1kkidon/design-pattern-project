using Avalonia.Controls.Shapes;
using Avalonia.Media;

using Avalonia.Controls;
using Avalonia.Media.Imaging; 

using shared;
using System;

namespace game_client.Models;
// Obstacle base class
    public class Obstacle : GameObject
    {
        public Image _image; 
        private string _imagePath;

        public Obstacle(Vector2 location, string imagePath) : base(location)
        {
            _imagePath = imagePath;
            _image = new Image();
        }

        public void LoadImage()
        {
            if(_image.Source == null) 
            {
                try
                {
                    _image.Source = new Bitmap(_imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                }
                _image.Width= 50;
                _image.Height = 50;
                AddToStackPanel(_image);
            }
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
