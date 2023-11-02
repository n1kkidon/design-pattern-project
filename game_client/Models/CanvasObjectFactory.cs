using System;
using Avalonia.Media;
using shared;

namespace game_client.Models;
public class CanvasObjectFactory
{
    public GameObject CreateCanvasObject(CanvasObjectInfo info)
    {
        return info.EntityType switch
        {
            EntityType.PLAYER => new PlayerPixel(info.Name!, 
                Color.FromRgb(info.Color!.R, info.Color.G, info.Color.B), info.Location),
            EntityType.ENEMY => new EnemyPixel(info.Name!, 
                Color.FromRgb(info.Color!.R, info.Color.G, info.Color.B), info.Location),
            EntityType.COIN => new CoinView(info.Location),
            EntityType.OBSTACLE => new IndestructibleObstacleDecorator(new Obstacle(info.Location)),
            _ => throw new ArgumentException("Invalid GameObject type."),
        };
    }
}
