using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models.PlayerState
{
    public class InjuredState : IPlayerState
    {
        private PlayerPixel _player;

        public InjuredState(PlayerPixel player)
        {
            Console.WriteLine("Player is now in Injured State.");
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
