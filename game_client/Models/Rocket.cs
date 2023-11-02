using game_client.Socket;
using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public class Rocket: ShootAlgorithm
    {
        public Rocket(SocketService service) : base(service) { }

        public override async Task Shoot(IVector2 position)
        {
            socketService.setWeaponProjectiles(WeaponType.ROCKET);
            await socketService.OnCurrentPlayerShoot(position, WeaponType.ROCKET);
        }
    }
}
