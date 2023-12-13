using shared;

namespace game_server.Memento
{
    public class PlayerOriginator
    {
        public Vector2 Location { get; set; }

        public PlayerMemento SaveToMemento()
        {
            return new PlayerMemento(Location);
        }

        public void RestoreFromMemento(PlayerMemento memento)
        {
            Location = memento.Location;
        }
    }

}
