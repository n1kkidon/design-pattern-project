using System;
using Avalonia.Media;
using game_client.Bridge;
using shared;

namespace game_client.Models;
public class CanvasObjectFactory
{
    public GameObject CreateCanvasObject(CanvasObjectInfo info)
    {
        //var YellowColor = new YellowColor();
        //var YellowSquareShape = new Circle(YellowColor);
        return info.EntityType switch
        {
            EntityType.COIN => new CoinView(info.Location),
            EntityType.PLAYER => new PlayerPixel(info.Name!, 
                Color.FromRgb(info.Color!.R, info.Color.G, info.Color.B), info.Location, info.Health),
            EntityType.ENEMY => new EnemyPixel(info.Name!, 
                Color.FromRgb(info.Color!.R, info.Color.G, info.Color.B), info.Location, info.Health),
            EntityType.OBSTACLE => new Obstacle(info.Location, info.ImagePath),
            _ => throw new ArgumentException("Invalid GameObject type."),
        };
    }
}
