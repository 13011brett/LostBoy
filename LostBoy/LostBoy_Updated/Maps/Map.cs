using LostBoy.Core;
using LostBoy.Entities;

namespace LostBoy.Maps;

/// <summary>
/// Terrain tile types that add visual variety to the map.
/// </summary>
public struct Tile
{
    public char Glyph;
    public ConsoleColor Color;
    public bool IsWalkable;

    public static Tile Empty => new() { Glyph = ' ', Color = ConsoleColor.DarkGray, IsWalkable = true };
    public static Tile Floor => new() { Glyph = '·', Color = ConsoleColor.DarkGray, IsWalkable = true };
    public static Tile Rubble => new() { Glyph = '░', Color = ConsoleColor.DarkGray, IsWalkable = true };
    public static Tile Pillar => new() { Glyph = '○', Color = ConsoleColor.Gray, IsWalkable = false };
    public static Tile Torch => new() { Glyph = '♦', Color = ConsoleColor.DarkYellow, IsWalkable = true };
    public static Tile Moss => new() { Glyph = '~', Color = ConsoleColor.DarkGreen, IsWalkable = true };
    public static Tile Crack => new() { Glyph = '▪', Color = ConsoleColor.DarkGray, IsWalkable = true };
    public static Tile Water => new() { Glyph = '≈', Color = ConsoleColor.DarkCyan, IsWalkable = false };
}

/// <summary>
/// Represents a game map/level. Uses composition — HAS enemies, not IS-A player.
/// Now includes terrain tiles for visual variety.
/// </summary>
public class Map
{
    public int Width { get; }
    public int Height { get; }
    public int Difficulty { get; }
    public int MapLevel { get; }
    public string Name { get; }
    public List<Enemy> Enemies { get; } = new();
    public Tile[,] Terrain { get; private set; }

    public Map(int width, int height, int enemyCount, string name = "The Dungeon")
    {
        Width = Math.Min(width, Console.LargestWindowWidth - 5);
        Height = Math.Min(height, Console.LargestWindowHeight - 5);
        Difficulty = enemyCount;
        MapLevel = Math.Max(1, enemyCount / 5);
        Name = name;

        Terrain = new Tile[Width, Height];
        GenerateTerrain();
        SpawnEnemies(enemyCount);
    }

    private void GenerateTerrain()
    {
        // Fill with empty space
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Terrain[x, y] = Tile.Empty;

        // Scatter floor dots for depth perception
        for (int x = 2; x < Width - 2; x++)
            for (int y = 2; y < Height - 2; y++)
            {
                if (Rng.Next(100) < 12)
                    Terrain[x, y] = Tile.Floor;
            }

        // Pillars (non-walkable obstacles)
        int pillarCount = (Width * Height) / 300;
        for (int i = 0; i < pillarCount; i++)
        {
            int px = Rng.Next(4, Width - 4);
            int py = Rng.Next(3, Height - 3);
            Terrain[px, py] = Tile.Pillar;
        }

        // Torches along walls
        for (int x = 4; x < Width - 4; x += Rng.Next(6, 14))
            Terrain[x, 1] = Tile.Torch;
        for (int y = 3; y < Height - 3; y += Rng.Next(4, 8))
        {
            Terrain[1, y] = Tile.Torch;
            Terrain[Width - 2, y] = Tile.Torch;
        }

        // Rubble patches
        int rubblePatches = Rng.Next(2, 5);
        for (int p = 0; p < rubblePatches; p++)
        {
            int cx = Rng.Next(5, Width - 5);
            int cy = Rng.Next(3, Height - 3);
            int size = Rng.Next(2, 4);
            for (int dx = -size; dx <= size; dx++)
                for (int dy = -size / 2; dy <= size / 2; dy++)
                {
                    int nx = cx + dx, ny = cy + dy;
                    if (nx > 1 && nx < Width - 2 && ny > 1 && ny < Height - 2 && Rng.Next(100) < 40)
                        Terrain[nx, ny] = Tile.Rubble;
                }
        }

        // Moss near edges
        int mossPatches = Rng.Next(1, 4);
        for (int p = 0; p < mossPatches; p++)
        {
            int cx = Rng.Next(3, Width - 3);
            int cy = Rng.Next(2, Height - 2);
            for (int dx = -2; dx <= 2; dx++)
                for (int dy = -1; dy <= 1; dy++)
                {
                    int nx = cx + dx, ny = cy + dy;
                    if (nx > 1 && nx < Width - 2 && ny > 1 && ny < Height - 2 && Rng.Next(100) < 30)
                        Terrain[nx, ny] = Tile.Moss;
                }
        }

        // Scattered cracks
        for (int i = 0; i < 8; i++)
        {
            int cx = Rng.Next(3, Width - 3);
            int cy = Rng.Next(2, Height - 2);
            if (Terrain[cx, cy].Glyph == ' ')
                Terrain[cx, cy] = Tile.Crack;
        }
    }

    /// <summary>
    /// Check if a position is walkable (inside borders and not blocked).
    /// </summary>
    public bool IsWalkable(Vec2 pos)
    {
        if (pos.X <= 0 || pos.X >= Width - 1 || pos.Y <= 0 || pos.Y >= Height - 1)
            return false;
        return Terrain[pos.X, pos.Y].IsWalkable;
    }

    public bool IsInBounds(Vec2 pos)
    {
        return pos.X > 0 && pos.X < Width - 1 &&
               pos.Y > 0 && pos.Y < Height - 1;
    }

    public Enemy? GetEnemyAt(Vec2 pos)
    {
        return Enemies.FirstOrDefault(e => e.IsAlive && e.Position == pos);
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Vec2 pos;
            int attempts = 0;
            do
            {
                pos = new Vec2(Rng.Next(3, Width - 3), Rng.Next(2, Height - 2));
                attempts++;
            } while (!Terrain[pos.X, pos.Y].IsWalkable && attempts < 50);

            Enemies.Add(new Enemy(MapLevel, Difficulty, pos));
        }
    }

    // ── Preset Maps ─────────────────────────────────────────

    public static Map CreateTutorialMap()
        => new(60, 25, 3, "The Cellar");

    public static Map CreateDungeon()
        => new(100, 40, 20, "The Dungeon");

    public static Map CreateCastle()
        => new(120, 45, 35, "The Castle");
}