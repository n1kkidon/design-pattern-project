using System.Threading.Tasks;

namespace game_client;

public interface ICommand
{
    bool ContinuosExecuteOnKeyDown {get; set;}
    Task Execute();
    Task OnKeyUp();
}