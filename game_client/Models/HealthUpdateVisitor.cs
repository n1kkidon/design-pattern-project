
using game_client.Models.CanvasItems;

namespace game_client.Models;
public class HealthUpdateVisitor : IHealthUpdateVisitor
{
    public void Visit(PlayerPixel playerPixel, int amount)
    {
        playerPixel.DecreaseHealth(amount);
    }

    public void Visit(EnemyPixel enemyPixel, int amount)
    {
        enemyPixel.DecreaseHealth(amount);
    }
}
