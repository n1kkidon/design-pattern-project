using Avalonia.Media;
using game_client.Models;
using game_client.Models.CanvasItems;
using shared;

namespace game_client.Builder
{
    public interface IProjectileBuilder
    {
        void SetLocation(Vector2 location);
        void SetColor(Color color);
        void SetSize(int width, int height);
        Projectile Build();
    }
}
