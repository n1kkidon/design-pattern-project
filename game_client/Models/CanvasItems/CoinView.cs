using Avalonia.Controls;
using shared;

namespace game_client.Models.CanvasItems;

public class CoinView : GameObject
{
    private static readonly ImageFlyweightFactory ImageFactory = new ImageFlyweightFactory();

    public CoinView(Vector2 location) : base(location)
    {
        string imagePath = "./Assets/coin.png";
        var coinImage = new Image
        {
            Width = Constants.CoinDimensions.X,
            Height = Constants.CoinDimensions.Y,
            Source = ImageFactory.GetImage(imagePath)
        };



        AddToStackPanel(coinImage);
    }
}