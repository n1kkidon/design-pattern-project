using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Composite
{
    public interface IGameComponent
    {
        void Operation();
        bool IsComposite();
        void IncreaseSize();
        void ChangePosition();
    }
}
