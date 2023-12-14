using game_client.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using game_client.Mediator;
using shared;

namespace game_client.Models;

public abstract class ShootAlgorithm : BaseComponent
{
    protected ShootAlgorithm(IMediator mediator = null) : base(mediator)
    {
    }

    public abstract Task Shoot(IVector2 position);
}

