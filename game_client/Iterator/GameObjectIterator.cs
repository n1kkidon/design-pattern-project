using game_client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Iterator
{
    public class GameObjectIterator : IGameObjectIterator
    {
        private GameObjectCollection _collection;
        private int _currentIndex = 0;

        public GameObjectIterator(GameObjectCollection collection)
        {
            _collection = collection;
        }
        public void Remove()
        {
            if (_currentIndex <= 0 || _currentIndex > _collection.Count())
            {
                throw new InvalidOperationException("Cannot remove item at current index.");
            }

            // Remove the last returned item by Next()
            _collection.Remove(_collection[_currentIndex - 1].Key);
        }
        public bool HasNext()
        {
            return _currentIndex < _collection.Count();
        }

        public GameObjectWithKey Next()
        {
            if (!HasNext())
            {
                throw new InvalidOperationException("No more elements");
            }
            return _collection[_currentIndex++]; // Use the index to retrieve the item
        }
    }

}
