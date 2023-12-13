using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.ChainOfResponsability
{
    public interface IPlayerJoinHandler
    {
        void SetNext(IPlayerJoinHandler handler);
        void Handle(PlayerJoinRequest request);
    }
}
