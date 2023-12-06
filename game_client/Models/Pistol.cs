﻿using game_client.Socket;
using shared;
using System.Threading.Tasks;

namespace game_client.Models;

public class Pistol : ShootAlgorithm
{
    public Pistol(SocketService service) : base(service) { }

    public override async Task Shoot(IVector2 position)
    {
        socketService.setWeaponProjectiles(WeaponType.PISTOL);
        await socketService.OnCurrentPlayerShoot(position, WeaponType.PISTOL);
    }
}
