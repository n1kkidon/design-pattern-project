using game_client.Models.CanvasItems;

namespace game_client.Models;

public interface IHealthUpdateVisitor
{
    void Visit(PlayerPixel playerPixel, int amount);
    void Visit(EnemyPixel enemyPixel, int amount);
}