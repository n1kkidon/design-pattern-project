﻿using game_client.Socket;
using shared;
using System.Threading.Tasks;
using game_client.Mediator;

namespace game_client.Models;

public class Sniper: ShootAlgorithm
{
    public Sniper(IMediator mediator = null) : base(mediator)
    {
    }

    public override async Task Shoot(IVector2 position)
    {
        var args = new ProjectileShootArgs(position.ToVector2(), WeaponType.SNIPER);
        await Mediator.Notify(this, "OnCurrentPlayerShoot", args);
        //await socketService.OnCurrentPlayerShoot(position, WeaponType.SNIPER);
    }
}

