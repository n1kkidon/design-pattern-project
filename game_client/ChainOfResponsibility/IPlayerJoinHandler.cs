using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.ChainOfResponsibility
{
    public interface IPlayerJoinHandler
    {
        IPlayerJoinHandler SetNext(IPlayerJoinHandler handler);
        void Handle(PlayerJoinRequest request);
    }
}
