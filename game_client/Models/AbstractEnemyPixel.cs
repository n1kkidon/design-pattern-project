using System;
using shared;

namespace game_client.Models
{
    public class BasicEnemyPixel : EnemyPixel, IEnemyPixel
    {
        private Random _random = new Random();

        public BasicEnemyPixel(string name, Avalonia.Media.Color color, Vector2 location)
            : base(name, color, location)
        {

        }
        public void DisplayEnemy()
        {
            MoveToRandomPosition();
        }

        private void MoveToRandomPosition()
        {
            int newX = _random.Next(0, 500);
            int newY = _random.Next(0, 800);

            this.Location = new Vector2(newX, newY);

            Pixel.Margin = new Avalonia.Thickness(newX, newY, 0, 0);
        }
    }


}
