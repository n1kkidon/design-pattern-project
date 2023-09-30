using game_client.Models;

namespace game_client.Commands.PlayerMovement;
public interface IMoveCommand
{
    public Game game {get; set;}
}