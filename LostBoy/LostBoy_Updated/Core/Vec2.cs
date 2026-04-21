namespace LostBoy.Core;

/// <summary>
/// Unified 2D position used throughout the game.
/// Replaces the old Player.Vec3 struct and Entity.Vec3 class.
/// </summary>
public struct Vec2
{
    public int X { get; set; }
    public int Y { get; set; }

    public Vec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vec2 Zero => new(0, 0);

    public static double Distance(Vec2 a, Vec2 b)
    {
        int dx = a.X - b.X;
        int dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public static bool operator ==(Vec2 a, Vec2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vec2 a, Vec2 b) => !(a == b);

    public override bool Equals(object? obj) => obj is Vec2 other && this == other;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
}
