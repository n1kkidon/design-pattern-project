using Avalonia.Media;
using game_client.Models;
using shared;

namespace game_client.Builder;

public class ProjectileDirector
{
    public Projectile Construct(IProjectileBuilder builder, Vector2 location, Color color, int width, int height)
    {
        builder.SetLocation(location);
        builder.SetColor(color);
        builder.SetSize(width, height);
        return builder.Build();
    }
}


