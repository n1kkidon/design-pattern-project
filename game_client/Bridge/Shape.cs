using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Bridge
{
    public abstract class ObjectShape
    {
        protected IColor color;
        public abstract Control Draw();

        protected ObjectShape(IColor color)
        {
            this.color = color;
        }
    }

}
