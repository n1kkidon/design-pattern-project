using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;

namespace game_client.Models
{
    public class ImageFlyweightFactory
    {
        private readonly Dictionary<string, Bitmap> _images = new Dictionary<string, Bitmap>();

        public Bitmap GetImage(string imagePath)
        {
            if (!_images.ContainsKey(imagePath))
            {
                try
                {
                    _images[imagePath] = new Bitmap(imagePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading image: {ex.Message}");
                    return null;
                }
            }

            return _images[imagePath];
        }
    }
}
