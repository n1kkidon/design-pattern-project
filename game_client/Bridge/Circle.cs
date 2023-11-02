using Avalonia.Controls.Shapes;
using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Bridge
{
    public class Circle : ObjectShape
    {
        public Circle(IColor color) : base(color) { }

        public override Control Draw()
        {
            var fill = color.GetBrush();
            return new Ellipse { Fill = fill, Width = 16, Height = 16 };
        }
    }
}
