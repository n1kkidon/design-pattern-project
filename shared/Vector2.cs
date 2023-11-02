using System.Drawing;

namespace shared;

public struct Vector2 : IVector2
{
    public float X{get; set;}
    public float Y{get; set;}
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public bool IsZero()
    {
        return X == 0 && Y == 0;
    }

    public static Vector2 operator +(Vector2 v1, Vector2 v2) => new(v1.X + v2.X, v1.Y + v2.Y);
    public override string ToString() => $"<{X}, {Y}>";
    public Vector2 ToVector2() => this;
}

public interface IVector2
{
    float X {get; set;}
    float Y {get; set;}
    bool IsZero();
    Vector2 ToVector2();
    public static IVector2 operator +(IVector2 v1, IVector2 v2) => new Vector2(v1.X + v2.X, v1.Y + v2.Y);
}

public struct PointAdapter : IVector2
{
    public float X { get => _point.X; set => _point.X = (int)value; }
    /// <summary>
    /// Y value is inverted (value = MapHeight - value)
    /// </summary>
    public float Y { get => Constants.MapHeight-(float)_point.Y; set => _point.Y = (int)value; }
    private Point _point;
    public PointAdapter(Point point) => _point = point;
    public bool IsZero() => X == 0 && Y == 0;
    public Vector2 ToVector2() => new(X, Y);
}