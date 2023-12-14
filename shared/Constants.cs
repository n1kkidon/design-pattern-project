namespace shared;

public class Constants
{
    public const string ServerIP = "http://localhost:5000";
    //public const string ServerIP = "https://nikkidon.org";

    
    public const int MapHeight = 500;
    public const int MapWidth = 800;

    //increment of how much a player moves onClick. This is temp.
    public const int MoveStep = 6;
    public const int ProjectileDistPerTick = 15;
    public const int PlayerHealth = 25;
    public const int EnemyDamage = 2;
    
    public static readonly Vector2 ObstacleDimensionsTree = new Vector2(50, 50);
    public static readonly Vector2 CoinDimensions = new Vector2(22, 22);
    public static readonly Vector2 PlayerDimensions = new Vector2(30, 30);

    public static Vector2 GetDimensions(EntityType entityType)
    {
        return entityType switch
        {
            EntityType.COIN => CoinDimensions,
            EntityType.PLAYER => PlayerDimensions,
            EntityType.OBSTACLE => ObstacleDimensionsTree,
            _ => new Vector2(0, 0)
        };
    }
}
