using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Iterator
{
    public interface IGameObjectIterator
    {
        bool HasNext();
        GameObjectWithKey Next();
        void Remove();
    }
}
