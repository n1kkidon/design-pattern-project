using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public class EasyEnemyStats : IEnemyStats
    {
        public int Health => 100;
        public int Damage => 10;
    }

    public class MediumEnemyStats : IEnemyStats
    {
        public int Health => 200;
        public int Damage => 20;
    }

    public class HardEnemyStats : IEnemyStats
    {
        public int Health => 300;
        public int Damage => 30;
    }

    public class InsaneEnemyStats : IEnemyStats
    {
        public int Health => 400;
        public int Damage => 40;
    }
}

