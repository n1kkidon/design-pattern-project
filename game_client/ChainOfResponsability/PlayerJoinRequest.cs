using shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.ChainOfResponsability
{
    public class PlayerJoinRequest
    {
        public string Name { get; set; }
        public WeaponType SelectedWeapon { get; set; }
        public bool IsValid { get; set; } = true;

    }
}
