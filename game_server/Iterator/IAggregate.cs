namespace game_server.Iterator
{
    public interface IAggregate
    {
        IGameObjectIterator CreateIterator();
    }
}
