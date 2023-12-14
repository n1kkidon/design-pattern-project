using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models.PlayerState
{
    public class DeadState : IPlayerState
    {
        private PlayerPixel _player;

        public DeadState(PlayerPixel player)
        {
            Console.WriteLine("Player is now in Dead State.");
            _player = player;
        }

        public void Shoot(IVector2 position)
        {
            Console.WriteLine("Dead players can't shoot.");
        }

        public void TakeDamage(int amount)
        {
            // Dead players don't take damage
        }
    }
}
