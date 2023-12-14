using shared;

namespace game_server.Memento
{
    public class PlayerMemento
    {
        public Vector2 Location { get; private set; }
        public int Health { get; private set; }
        public int CoinCount { get; private set; }

        public PlayerMemento(Vector2 location, int health, int coinCount)
        {
            Location = location;
            Health = health;
            CoinCount = coinCount;
        }
    }
}
