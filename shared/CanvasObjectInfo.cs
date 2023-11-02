namespace shared;

public class CanvasObjectInfo
{
    public required Vector2 Location {get; set;}
    public string? Name {get; set;}
    public required string Uuid {get; set;}
    public RGB? Color {get; set;}
    public required EntityType EntityType {get; set;}
}

public record RGB(byte R, byte G, byte B);

public enum EntityType
{
    PLAYER, ENEMY, COIN, OBSTACLE
}