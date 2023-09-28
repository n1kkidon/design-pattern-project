using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.Shapes;

namespace game_client.Views
{
    public class CoinView
    {
        public StackPanel CoinObject;
        private Rectangle CoinShape;

        public CoinView(double x, double y)
        {
            CoinObject = new StackPanel();
            CoinShape = new Rectangle
            {
                Fill = new SolidColorBrush(Colors.Gold),
                Width = 10,
                Height = 10
            };
            CoinObject.Children.Add(CoinShape);
            Canvas.SetLeft(CoinObject, x);
            Canvas.SetTop(CoinObject, y);
        }
    }
}
