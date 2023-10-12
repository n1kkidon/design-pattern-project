using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public class BasicEnemyStats : IEnemyStats
    {
        public int Health => 100;
        public int Damage => 10;
    }

}
