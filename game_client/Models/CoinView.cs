using Avalonia.Media;
using Avalonia.Controls.Shapes;
using shared;
using Avalonia.Controls;
using game_client.Bridge;

namespace game_client.Models
{
    public class CoinView : GameObject
    {
        public CoinView(Vector2 location, ObjectShape shape) : base(location)
        {
            var objectToDraw = shape.Draw();
            AddToStackPanel(objectToDraw);
        }
    }
}
