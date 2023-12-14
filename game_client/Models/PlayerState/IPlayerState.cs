using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models.PlayerState
{
    public interface IPlayerState
    {
        void Shoot(IVector2 position);
        void TakeDamage(int amount);
    }
}
