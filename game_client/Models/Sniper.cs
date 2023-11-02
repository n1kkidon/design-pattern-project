﻿using game_client.Socket;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public class Sniper: ShootAlgorithm
    {
        public Sniper(SocketService service) : base(service) { }

        public override async Task Shoot(Vector2 position)
        {
            // Implement shooting logic specific to a pistol
            socketService.setWeaponProjectiles(WeaponType.SNIPER);
            await socketService.OnCurrentPlayerShoot(position);
        }
    }
}
