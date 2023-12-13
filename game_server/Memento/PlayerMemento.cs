using shared;

namespace game_server.Memento
{
    public class PlayerMemento
    {
        public Vector2 Location { get; private set; }

        public PlayerMemento(Vector2 location)
        {
            Location = location;
        }
    }
}
