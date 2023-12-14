using System;
using game_client.Views;

namespace game_client.ChainOfResponsibility;

public class NameValidationHandler : BaseHandler
{
    public override void Handle(PlayerJoinRequest request)
    {
        Console.WriteLine("NameValidationHandler: Checking player name.");
        if (string.IsNullOrEmpty(request.Name))
        {
            request.IsValid = false;
            return;
        }
        NextHandler?.Handle(request);
    }
}