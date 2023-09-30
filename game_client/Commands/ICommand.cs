using System.Threading.Tasks;
using game_client.Models;

namespace game_client;

public interface ICommand
{
    bool ContinuosExecuteOnKeyDown {get; set;}
    Task Execute();
    Task OnKeyUp();

}
