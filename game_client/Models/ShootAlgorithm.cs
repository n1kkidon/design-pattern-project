using game_client.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using shared;

namespace game_client.Models;

public abstract class ShootAlgorithm
{
    protected SocketService socketService;

    protected ShootAlgorithm(SocketService service)
    {
        socketService = service;
    }

    public abstract Task Shoot(IVector2 position);
}

