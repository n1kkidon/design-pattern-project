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
                "EasySoldier" => new EasySoldierStats(),
                "HardSoldier" => new HardSoldierStats(),
                "EasyKnight" => new EasyKnightStats(),
                "HardKnight" => new HardKnightStats(),
                _ => throw new ArgumentException("Invalid difficulty")
            };
        }
    }
}