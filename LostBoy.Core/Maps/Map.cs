using LostBoy.Core;
using LostBoy.Entities;

namespace LostBoy.Maps;

public struct Tile
{
    public char Glyph;
    public string Color;
    public bool IsWalkable;

    public static Tile Empty => new() { Glyph = ' ', Color = "#333333", IsWalkable = true };
    public static Tile Floor => new() { Glyph = '·', Color = "#555555", IsWalkable = true };
    public static Tile Rubble => new() { Glyph = '░', Color = "#555555", IsWalkable = true };
    public static Tile Pillar => new() { Glyph = '○', Color = "#888888", IsWalkable = false };
    public static Tile Torch => new() { Glyph = '♦', Color = "#D4A017", IsWalkable = true };
    public static Tile Moss => new() { Glyph = '~', Color = "#2E8B57", IsWalkable = true };
    public static Tile Crack => new() { Glyph = '▪', Color = "#555555", IsWalkable = true };
    public static Tile Water => new() { Glyph = '≈', Color = "#1E90FF", IsWalkable = false };
}

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
        Width = width;
        Height = height;
        Difficulty = enemyCount;
        MapLevel = Math.Max(1, enemyCount / 5);
        Name = name;

        Terrain = new Tile[Width, Height];
        GenerateTerrain();
        SpawnEnemies(enemyCount);
    }

    private void GenerateTerrain()
    {
        for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                Terrain[x, y] = Tile.Empty;

        for (int x = 2; x < Width - 2; x++)
            for (int y = 2; y < Height - 2; y++)
                if (Rng.Next(100) < 12) Terrain[x, y] = Tile.Floor;

        int pillarCount = (Width * Height) / 150;
        for (int i = 0; i < pillarCount; i++)
        {
            int px = Rng.Next(4, Width - 4);
            int py = Rng.Next(3, Height - 3);
            Terrain[px, py] = Tile.Pillar;
            if (Rng.Next(100) < 30)
            {
                if (px + 2 < Width - 3) Terrain[px + 2, py] = Tile.Pillar;
                if (Rng.Next(100) < 50 && px + 4 < Width - 3) Terrain[px + 4, py] = Tile.Pillar;
            }
        }

        int waterPools = Rng.Next(1, 3);
        for (int p = 0; p < waterPools; p++)
        {
            int cx = Rng.Next(8, Width - 8);
            int cy = Rng.Next(4, Height - 4);
            int sizeX = Rng.Next(2, 5);
            int sizeY = Rng.Next(1, 3);
            for (int dx = -sizeX; dx <= sizeX; dx++)
                for (int dy = -sizeY; dy <= sizeY; dy++)
                {
                    int nx = cx + dx, ny = cy + dy;
                    if (nx > 2 && nx < Width - 3 && ny > 2 && ny < Height - 3)
                    {
                        if (Math.Abs(dx) == sizeX && Math.Abs(dy) == sizeY) continue;
                        Terrain[nx, ny] = Tile.Water;
                    }
                }
        }

        int wallSegments = Rng.Next(1, 3);
        for (int w = 0; w < wallSegments; w++)
        {
            int wx = Rng.Next(6, Width / 2);
            int wy = Rng.Next(3, Height - 3);
            int wLen = Rng.Next(4, 12);
            int gapPos = Rng.Next(1, wLen - 1);
            for (int i = 0; i < wLen; i++)
            {
                if (i == gapPos || i == gapPos + 1) continue;
                int nx = wx + i;
                if (nx > 1 && nx < Width - 2) Terrain[nx, wy] = Tile.Pillar;
            }
        }

        for (int x = 4; x < Width - 4; x += Rng.Next(6, 14))
            Terrain[x, 1] = Tile.Torch;
        for (int y = 3; y < Height - 3; y += Rng.Next(4, 8))
        {
            Terrain[1, y] = Tile.Torch;
            Terrain[Width - 2, y] = Tile.Torch;
        }

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

        for (int i = 0; i < 8; i++)
        {
            int cx = Rng.Next(3, Width - 3);
            int cy = Rng.Next(2, Height - 2);
            if (Terrain[cx, cy].Glyph == ' ') Terrain[cx, cy] = Tile.Crack;
        }
    }

    public bool IsWalkable(Vec2 pos)
    {
        if (pos.X <= 0 || pos.X >= Width - 1 || pos.Y <= 0 || pos.Y >= Height - 1) return false;
        return Terrain[pos.X, pos.Y].IsWalkable;
    }

    public bool IsInBounds(Vec2 pos) =>
        pos.X > 0 && pos.X < Width - 1 && pos.Y > 0 && pos.Y < Height - 1;

    public Enemy? GetEnemyAt(Vec2 pos) =>
        Enemies.FirstOrDefault(e => e.IsAlive && e.Position == pos);

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

    public static Map CreateTutorialMap() => new(60, 25, 3, "The Cellar");
    public static Map CreateDungeon() => new(80, 30, 20, "The Dungeon");
    public static Map CreateCastle() => new(100, 35, 35, "The Castle");
}
