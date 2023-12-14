namespace game_client.Mediator;

public class BaseComponent
{
    protected IMediator Mediator;

    public BaseComponent(IMediator mediator = null)
    {
        Mediator = mediator;
    }

    public void SetMediator(IMediator mediator)
    {
        Mediator = mediator;
    }
}