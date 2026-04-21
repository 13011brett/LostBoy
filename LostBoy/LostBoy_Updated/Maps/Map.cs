using LostBoy.Core;
using LostBoy.Entities;

namespace LostBoy.Maps;

/// <summary>
/// Represents a game map/level. Uses composition — HAS enemies, not IS-A player.
/// This was one of the biggest architectural issues: Map inherited from Player.
/// </summary>
public class Map
{
    public int Width { get; }
    public int Height { get; }
    public int Difficulty { get; }
    public int MapLevel { get; }
    public List<Enemy> Enemies { get; } = new();
    public char BorderChar { get; set; } = '─';

    public Map(int width, int height, int enemyCount)
    {
        // Clamp to reasonable console sizes
        Width = Math.Min(width, Console.LargestWindowWidth - 5);
        Height = Math.Min(height, Console.LargestWindowHeight - 5);
        Difficulty = enemyCount;
        MapLevel = Math.Max(1, enemyCount / 5);

        SpawnEnemies(enemyCount);
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var position = new Vec2(
                Rng.Next(2, Width - 2),
                Rng.Next(2, Height - 2)
            );

            Enemies.Add(new Enemy(MapLevel, Difficulty, position));
        }
    }

    /// <summary>
    /// Check if a position is within the playable area (inside borders).
    /// </summary>
    public bool IsInBounds(Vec2 pos)
    {
        return pos.X > 0 && pos.X < Width - 1 &&
               pos.Y > 0 && pos.Y < Height - 1;
    }

    /// <summary>
    /// Find an enemy at the given position, if any.
    /// </summary>
    public Enemy? GetEnemyAt(Vec2 pos)
    {
        return Enemies.FirstOrDefault(e => e.IsAlive && e.Position == pos);
    }
}
