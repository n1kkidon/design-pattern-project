using System;
using System.Threading.Tasks;
using shared;

namespace game_client.Models;

public class Game : GameBase
{
    public Vector2 MovementInput = new(0, 0);
    public event Action? OnTick;
    protected override async Task Tick()
    {
        OnTick?.Invoke();
        if(!MovementInput.IsZero())
            await Mediator.Notify(this, "OnCurrentPlayerMove", MovementInput);
    }
}
