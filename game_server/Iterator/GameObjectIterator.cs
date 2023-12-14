namespace game_server.Iterator
{
    public class GameObjectIterator : IGameObjectIterator
    {
        private readonly GameObjectCollection _collection;
        private int _currentIndex = 0;

        public GameObjectIterator(GameObjectCollection collection) => _collection = collection;

        public void Remove()
        {
            if (_currentIndex <= 0 || _currentIndex > _collection.Count())
                return;

            // Remove the last returned item by Next()
            _collection.Remove(_collection[_currentIndex - 1].Key);
            _currentIndex--;
        }
        public bool HasNext() => _currentIndex < _collection.Count();

        public GameObjectWithKey Next()
        {
            if (!HasNext())
                throw new InvalidOperationException("No more elements");
            return _collection[_currentIndex++]; // Use the index to retrieve the item
        }
    }

}
