using System.Threading.Tasks;
using game_client.Commands.PlayerMovement;
using game_client.Models;

namespace game_client;

public class MoveLeftCommand : ICommand, IMoveCommand
{
    public Game game {get; set;}    
    public bool ContinuosExecuteOnKeyDown { get; set; }

    public MoveLeftCommand(bool continuosExecuteOnKeyDown, Game game)
    {
        this.game = game;
        ContinuosExecuteOnKeyDown = continuosExecuteOnKeyDown;
    } 
    public Task Execute()
    {
        game.MovementInput.X = -1f;
        return Task.CompletedTask;
    }
    public Task OnKeyUp()
    {
        game.MovementInput.X = 0f;
        return Task.CompletedTask;
    }
}
