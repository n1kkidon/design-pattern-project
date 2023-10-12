using Avalonia.Media;
using Avalonia;
using shared;

namespace game_client.Models
{
    public class BasicEnemyFactory : AbstractEnemyFactory
    {
        public override IEnemyPixel CreateEnemyPixel(string name, Color color, Vector2 location)
        {
            return new BasicEnemyPixel(name, color, location);
        }

        public override IEnemyStats CreateEnemyStats()
        {
            return new BasicEnemyStats();
        }
    }

}
