using Avalonia.Controls.Shapes;
using Avalonia.Media;

using Avalonia.Controls; // For Image control
using Avalonia.Media.Imaging; // For Bitmap

using shared;
using System;

namespace game_client.Models;
// Obstacle base class
    public class Obstacle : GameObject
    {
        public Image _image; // Changed from Shape to Image

        public Obstacle(Vector2 location, string imagePath) : base(location)
        {
            _image = new Image();

            try
            {
                // Load the image from the specified path
                _image.Source = new Bitmap("./Assets/obstacle.png");
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found)
                Console.WriteLine($"Error loading image: {ex.Message}");
            }

            // Set the size of the image (you can adjust this as needed)
            _image.Width = 20;
            _image.Height = 100;

            AddToStackPanel(_image);
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
