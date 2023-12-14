using shared;

namespace game_server.Memento
{
    public class PlayerOriginator
    {
        public Vector2 Location { get; set; }
        public int Health { get; set; }
        public int CoinCount { get; set; }

        public PlayerMemento SaveToMemento()
        {
            return new PlayerMemento(Location, Health, CoinCount);
        }

        public void RestoreFromMemento(PlayerMemento memento)
        {
            Location = memento.Location;
            Health = memento.Health;
            CoinCount = memento.CoinCount;
        }
    }

}
