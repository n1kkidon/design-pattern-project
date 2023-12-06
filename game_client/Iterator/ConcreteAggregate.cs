using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Iterator
{
    public class GameObjectCollection : IAggregate
    {
        private List<GameObjectWithKey> _items = new List<GameObjectWithKey>();

        // Method to add a GameObject with its associated key
        public void Add(string key, GameObject gameObject)
        {
            _items.Add(new GameObjectWithKey(key, gameObject));
        }
        public void Remove(string key)
        {
            var itemToRemove = _items.FirstOrDefault(item => item.Key == key);
            if (itemToRemove.GameObject != null)
            {
                _items.Remove(itemToRemove);
            }
        }

        // Create an iterator for the current collection of GameObjectWithKey
        public IGameObjectIterator CreateIterator()
        {
            return new GameObjectIterator(this);
        }

        // Method to get an item by index (used by the iterator)
        public GameObjectWithKey this[int index] => _items[index];

        // Method to get the count of items (used by the iterator)
        public int Count() => _items.Count;
    }

}
