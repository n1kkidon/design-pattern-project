namespace game_server.Iterator
{
    public interface IGameObjectIterator
    {
        bool HasNext();
        GameObjectWithKey Next();
        void Remove();
    }
}
