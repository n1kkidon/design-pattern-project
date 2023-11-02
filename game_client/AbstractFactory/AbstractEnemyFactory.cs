using Avalonia.Media;
using shared;

namespace game_client.AbstractFactory
{
    public abstract class AbstractEnemyFactory
    {
        public abstract IEnemyPixel CreateEnemyPixel(string name, Color color, Vector2 location);
        public abstract IEnemyStats CreateEnemyStats(string enemyType);
    }
}
