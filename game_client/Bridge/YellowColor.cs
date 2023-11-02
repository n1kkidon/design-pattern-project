using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Bridge
{
    public class YellowColor : IColor
    {
        public SolidColorBrush GetBrush()
        {
            return new SolidColorBrush(Colors.Yellow);
        }
    }
}
