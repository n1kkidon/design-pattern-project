using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Bridge
{
    public class Square : ObjectShape
    {
        public Square(IColor color) : base(color) { }

        public override Control Draw()
        {
            var fill = color.GetBrush();
            return new Rectangle { Fill = fill, Width = 16, Height = 16 };
        }
    }
}
