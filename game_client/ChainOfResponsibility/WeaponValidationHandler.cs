using System;

namespace game_client.ChainOfResponsibility;

public class WeaponValidationHandler : BaseHandler
{
    public override void Handle(PlayerJoinRequest request)
    {
        Console.WriteLine("WeaponValidationHandler: Checking weapon selection.");
        NextHandler?.Handle(request);
    }
}