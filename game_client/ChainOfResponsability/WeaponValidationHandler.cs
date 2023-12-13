using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.ChainOfResponsability
{
    public class WeaponValidationHandler : IPlayerJoinHandler
    {
        private IPlayerJoinHandler _nextHandler;

        public void SetNext(IPlayerJoinHandler handler)
        {
            _nextHandler = handler;
        }

        public void Handle(PlayerJoinRequest request)
        {
            Console.WriteLine("WeaponValidationHandler: Checking weapon selection.");
            _nextHandler?.Handle(request);

        }
    }
}
