namespace shared;

public class CanvasObjectInfo
{
    public string? ImagePath { get; set; }
    public int Health { get; set; }
    public required Vector2 Location {get; set;}
    public string? Name {get; set;}
    public required string Uuid {get; set;}
    public RGB? Color {get; set;}
    public required EntityType EntityType {get; set;}
    public WeaponType? WeaponType { get; set;}
    private float _width = -1;
    private float _height = -1;

    public float Width
    {
        get
        {
            if (_height <= 0 || _width <= 0)
            {
                var dimensions = Constants.GetDimensions(EntityType);
                _width = dimensions.X;
                _height = dimensions.Y;
            }
            return _width;
            
        }
        set => _width = value;
    }

    public float Height
    {
        get
        {
            if (_height <= 0 || _width <= 0)
            {
                var dimensions = Constants.GetDimensions(EntityType);
                _width = dimensions.X;
                _height = dimensions.Y;
            }
            return _height;
            
        }
        set => _height = value;
    }
    public int CoinCount { get; set; }
    
}

public record RGB(byte R, byte G, byte B);

public enum EntityType
{
    PLAYER, ENEMY, COIN, OBSTACLE
}
public enum WeaponType
{
    PISTOL, SNIPER, ROCKET, CANNON, HANDS
}