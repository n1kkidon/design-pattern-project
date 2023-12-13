using Avalonia.Controls;
using shared;

namespace game_client.Models
{
    public class CoinView : GameObject
    {
        private static readonly ImageFlyweightFactory ImageFactory = new ImageFlyweightFactory();

        public CoinView(Vector2 location) : base(location)
        {
            string imagePath = "./Assets/coin.png";
            var coinImage = new Image
            {
                Width = 22,
                Height = 22,
                Source = ImageFactory.GetImage(imagePath)
            };



            AddToStackPanel(coinImage);
        }
    }
}
