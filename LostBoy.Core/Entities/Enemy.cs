using LostBoy.Core;
using LostBoy.Items;
using LostBoy.Maps;

namespace LostBoy.Entities;

public enum MonsterType
{
    Skeleton,
    Zombie,
    Spider,
    Goblin,
    Wraith,
    Ogre,
    DarkKnight,
    Demon
}

public class Enemy : Entity
{
    public MonsterType MonsterType { get; }
    public List<Item> LootTable { get; } = new();

    private static readonly (MonsterType type, char icon, string color, float hpMult, float dmgMult)[] MonsterDefs =
    {
        (MonsterType.Skeleton,    '☠', "#AAAAAA", 0.8f,  0.7f),
        (MonsterType.Zombie,      'Z', "#6B8E23", 1.0f,  0.6f),
        (MonsterType.Spider,      '♦', "#8B4513", 0.6f,  0.9f),
        (MonsterType.Goblin,      'g', "#228B22", 0.7f,  0.8f),
        (MonsterType.Wraith,      'W', "#9370DB", 0.9f,  1.1f),
        (MonsterType.Ogre,        'O', "#CD853F", 1.5f,  1.0f),
        (MonsterType.DarkKnight,  'K', "#DC143C", 1.3f,  1.3f),
        (MonsterType.Demon,       'D', "#FF4500", 1.8f,  1.5f),
    };

    public Enemy(int mapLevel, int difficulty, Vec2 position)
    {
        // Pick monster type based on difficulty
        int maxIndex = Math.Min(MonsterDefs.Length, 2 + difficulty / 5);
        int idx = Rng.Next(0, maxIndex);
        var def = MonsterDefs[idx];

        MonsterType = def.type;
        Icon = def.icon;
        Color = def.color;
        Name = def.type.ToString();
        Position = position;
        Level = Math.Max(1, mapLevel + Rng.Next(-1, 2));

        float baseHp = 40 + (difficulty * 8);
        float baseDmg = 5 + (difficulty * 2);

        Stats = new StatsBuilder()
            .SetHealth(baseHp * def.hpMult)
            .SetArmor(difficulty * 2)
            .Build();

        Damage = baseDmg * def.dmgMult;
        Experience = 20 + (difficulty * 5) + (Level * 10);

        // Generate loot
        var loot = Items.LootTable.GenerateLoot(difficulty);
        if (loot != null) LootTable.Add(loot);
    }

    /// <summary>
    /// Try to move in a random direction. Returns old position if moved, null if didn't.
    /// </summary>
    public Vec2? TryRandomMove(Map map)
    {
        if (Rng.Next(100) > 15) return null; // Only move 15% of ticks

        var oldPos = Position;
        int dir = Rng.Next(4);
        Vec2 newPos = dir switch
        {
            0 => new Vec2(Position.X, Position.Y - 1),
            1 => new Vec2(Position.X, Position.Y + 1),
            2 => new Vec2(Position.X - 1, Position.Y),
            _ => new Vec2(Position.X + 1, Position.Y),
        };

        if (map.IsWalkable(newPos))
        {
            Position = newPos;
            return oldPos;
        }

        return null;
    }
}
