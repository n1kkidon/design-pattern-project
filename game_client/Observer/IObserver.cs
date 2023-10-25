using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Observer
{
    public interface IObserver
    {
        void Update(int health);
    }

}
