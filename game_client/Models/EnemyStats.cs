using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.Models
{
    public class EasySoldierStats : IEnemyStats
    {
        public int Health => 100;
        public int Damage => 10;
    }

    public class HardSoldierStats : IEnemyStats
    {
        public int Health => 200;
        public int Damage => 20;
    }
    public class EasyKnightStats : IEnemyStats
    {
        public int Health => 150;
        public int Damage => 15;
    }

    public class HardKnightStats : IEnemyStats
    {
        public int Health => 300;
        public int Damage => 30;
    }
}

