using System;
using System.Threading.Tasks;
using game_client.Socket;
using shared;

namespace game_client;

public class MoveLeftCommand : ICommand
{
    public async Task Execute()
    {
        var socketService = SocketService.GetInstance();
        await socketService.OnCurrentPlayerMove(Direction.LEFT);
    }
}
