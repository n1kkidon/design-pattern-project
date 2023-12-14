using System.Threading.Tasks;

namespace game_client.Mediator;

public interface IMediator
{
    Task Notify(BaseComponent sender, string ev, object args);
}