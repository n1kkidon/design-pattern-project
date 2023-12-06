using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Iterator
{
    public struct GameObjectWithKey
    {
        public string Key { get; private set; }
        public GameObject GameObject { get; private set; }

        public GameObjectWithKey(string key, GameObject gameObject)
        {
            Key = key;
            GameObject = gameObject;
        }
    }

}
