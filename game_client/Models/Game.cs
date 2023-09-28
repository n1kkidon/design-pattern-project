using System.Threading.Tasks;

namespace game_client.Models;

public class Game : GameBase
{
    protected override async Task Tick()
    {
        foreach(var key in InputHandler.KeysWithRegistedCommands)
        {
            if(Keyboard.IsKeyPressed(key))
            {
                await InputHandler.HandleInput(key); 
                //TODO: rework the Direction thing, cannot normalize movement currently, char is moving diagonals at 2x speed
            }
        }
    }
}
