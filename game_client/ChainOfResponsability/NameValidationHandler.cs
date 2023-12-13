using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_client.ChainOfResponsability
{
    public class NameValidationHandler : IPlayerJoinHandler
    {
        private IPlayerJoinHandler _nextHandler;

        public void SetNext(IPlayerJoinHandler handler)
        {
            _nextHandler = handler;
        }

        public void Handle(PlayerJoinRequest request)
        {
            Console.WriteLine("NameValidationHandler: Checking player name.");
            if (string.IsNullOrEmpty(request.Name))
            {
                request.IsValid = false;
                return;
            }
            _nextHandler?.Handle(request);
        }
    }
}
