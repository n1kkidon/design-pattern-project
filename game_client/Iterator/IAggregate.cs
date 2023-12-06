using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Iterator
{
    public interface IAggregate
    {
        IGameObjectIterator CreateIterator();
    }
}
