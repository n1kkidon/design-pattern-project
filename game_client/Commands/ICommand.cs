using System.Threading.Tasks;

namespace game_client;

public interface ICommand
{
    Task Execute();
}
