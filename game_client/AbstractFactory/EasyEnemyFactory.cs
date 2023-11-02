using System;
using Avalonia.Media;
using shared;

namespace game_client.AbstractFactory;

public class EasyEnemyFactory : AbstractEnemyFactory
{
    public override IEnemyPixel CreateEnemyPixel(string name, Color color, Vector2 location)
    {
        return new BasicEnemyPixel(name, color, location);
    }

    public override IEnemyStats CreateEnemyStats(string enemyType)
    {
        return enemyType switch
        {
            "Soldier" => new EasySoldierStats(),
            "Knight" => new EasyKnightStats(),
            _ => throw new ArgumentException("Invalid enemy type for Easy difficulty")
        };
    }
}


