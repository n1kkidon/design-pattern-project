using Avalonia.Media;
using Avalonia;
using shared;
using System;

namespace game_client.Models
{
    public class BasicEnemyFactory : AbstractEnemyFactory
    {
        public override IEnemyPixel CreateEnemyPixel(string difficulty, string name, Color color, Vector2 location)
        {
            return new BasicEnemyPixel(name, color, location);
        }

        public override IEnemyStats CreateEnemyStats(string difficulty)
        {
            return difficulty switch
            {
                "Easy" => new EasyEnemyStats(),
                "Medium" => new MediumEnemyStats(),
                "Hard" => new HardEnemyStats(),
                "Insane" => new InsaneEnemyStats(),
                _ => throw new ArgumentException("Invalid difficulty")
            };
        }
    }
}