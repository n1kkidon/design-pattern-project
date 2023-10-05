using Avalonia.Media;
using Avalonia.Controls.Shapes;
using shared;

namespace game_client.Models
{
    public class CoinView : GameObject
    {
        private Rectangle CoinShape;

        public CoinView(Vector2 location) : base(location)
        {
            CoinShape = new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Gold),
                Width = 10,
                Height = 10
            };
            AddToStackPanel(CoinShape);
        }
    }
}
