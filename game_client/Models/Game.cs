using System.Threading.Tasks;
using game_client.Socket;
using shared;

namespace game_client.Models;

public class Game : GameBase
{
    public Vector2 MovementInput;
    protected override async Task Tick()
    {
        var socketService = SocketService.GetInstance();
        await socketService.OnCurrentPlayerMove(MovementInput);        
    }
    public Game() : base()
    {
        MovementInput = new(0, 0);
    }
}
