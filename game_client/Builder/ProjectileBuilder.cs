using Avalonia.Media;
using game_client.Models;
using shared;

namespace game_client.Builder
{
    public class ProjectileBuilder : IProjectileBuilder
    {
        private Vector2 _location;
        private Color _color;
        private int _width, _height;

        public void SetLocation(Vector2 location) => _location = location;
        public void SetColor(Color color) => _color = color;
        public void SetSize(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public Projectile Build() => new Projectile(_location, _color, _width, _height);
    }

}
