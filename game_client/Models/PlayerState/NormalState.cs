using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models.PlayerState
{
    public class NormalState : IPlayerState
    {
        private PlayerPixel _player;

        public NormalState(PlayerPixel player)
        {
            Console.WriteLine("Player is now in Normal State.");
            _player = player;
        }

        public void Shoot(IVector2 position)
        {
            _player.ShootAlgorithm.Shoot(position);
        }

        public void TakeDamage()
        {
            _player.DecreaseHealth();
        }
    }
}
