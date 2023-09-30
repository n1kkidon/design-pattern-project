namespace shared;

public class PlayerInfo
{
    public required Vector2 Location {get; set;}
    public required string Name {get; set;}
    public required string Uuid {get; set;}
    public required RGB Color {get; set;}
}

public record RGB(byte R, byte G, byte B);
