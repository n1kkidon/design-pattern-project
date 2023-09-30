using System.Threading.Tasks;
using game_client.Socket;
using shared;

namespace game_client.Models;

public class Game : GameBase
{
    public Vector2 MovementInput;
    private readonly SocketService socketService;
    protected override async Task Tick()
    {
        if(!MovementInput.Zero)
            await socketService.OnCurrentPlayerMove(MovementInput);        
    }
    public Game() : base()
    {
        MovementInput = new(0, 0);
        socketService = SocketService.GetInstance();
    }
}
