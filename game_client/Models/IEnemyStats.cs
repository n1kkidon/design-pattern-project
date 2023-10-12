using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public interface IEnemyStats
    {
        int Health { get; }
        int Damage { get; }
    }

}
