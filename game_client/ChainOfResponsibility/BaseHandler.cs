namespace game_client.ChainOfResponsibility;

public class BaseHandler : IPlayerJoinHandler
{
    protected IPlayerJoinHandler? NextHandler;

    public IPlayerJoinHandler SetNext(IPlayerJoinHandler handler)
    {
        NextHandler = handler;
        return NextHandler;
    }

    public virtual void Handle(PlayerJoinRequest request)
    {
        NextHandler?.Handle(request);
    }
}