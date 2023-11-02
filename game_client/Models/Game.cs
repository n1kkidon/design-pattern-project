using System;
using System.Drawing.Text;
using System.Threading.Tasks;
using game_client.Socket;
using shared;

namespace game_client.Models;

public class Game : GameBase
{
    public Vector2 MovementInput;
    private readonly SocketService socketService;
    public event Action? OnTick;
    protected override async Task Tick()
    {
        OnTick?.Invoke();
        if(!MovementInput.Zero)
            await socketService.OnCurrentPlayerMove(MovementInput);        
    }

    public static Game GetInstance()
    {
        _isnt ??= new();
        return _isnt;
    }
    private static Game? _isnt;
    private Game() : base()
    {
        MovementInput = new(0, 0);
        socketService = SocketService.GetInstance();
    }
}
