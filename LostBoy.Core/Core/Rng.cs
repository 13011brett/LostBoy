namespace LostBoy.Core;

public static class Rng
{
    private static readonly Random _random = new();

    public static int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);
    public static int Next(int maxExclusive) => _random.Next(maxExclusive);
    public static float NextFloat(float min, float max) => (float)(_random.NextDouble() * (max - min) + min);
    public static T Pick<T>(IList<T> items) => items[_random.Next(items.Count)];
}
