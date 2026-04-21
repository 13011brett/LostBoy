using LostBoy.Core;
using LostBoy.Items;
using LostBoy.Maps;

namespace LostBoy.Entities;

public enum MonsterType
{
    Troll,
    Ogre,
    Demon
}

/// <summary>
/// Enemy entity. Now properly inherits from Entity instead of Player.
/// Monster type determines icon and color.
/// </summary>
public class Enemy : Entity
{
    public MonsterType MonsterType { get; }
    public List<Item> LootTable { get; } = new();

    public Enemy(int mapLevel, int mapDifficulty, Vec2 position)
    {
        MonsterType = (MonsterType)Rng.Next(0, 3);
        Level = Math.Max(1, mapLevel); // Never level 0
        Position = position;

        // Scale stats with level — monsters should always be a threat
        int baseAP = 30 + (20 * Level);
        float baseHP = 50 + (Level * Rng.NextFloat(8, 20));

        Stats = new StatsBuilder()
            .SetAttackPower(baseAP)
            .SetHealth(baseHP)
            .SetArmor(5 * Level)
            .Build();

        // Damage formula ensures enemies always deal meaningful damage
        Damage = Math.Max(8, (Stats.AttackPower / 10f) + (3 * Level));
        Experience = (Level * mapDifficulty) + Rng.Next(5, mapDifficulty + 10);

        // Monster type affects stats slightly
        (Icon, Color, Name) = MonsterType switch
        {
            MonsterType.Troll => ('T', ConsoleColor.Yellow, "Troll"),
            MonsterType.Ogre => ('O', ConsoleColor.Magenta, "Ogre"),
            MonsterType.Demon => ('D', ConsoleColor.DarkRed, "Demon"),
            _ => ('?', ConsoleColor.Gray, "Unknown Beast"),
        };

        // Ogres are tankier, Demons hit harder
        if (MonsterType == MonsterType.Ogre)
        {
            Stats.Health *= 1.3f;
            Stats.MaxHealth *= 1.3f;
            Stats.Armor += 10;
        }
        else if (MonsterType == MonsterType.Demon)
        {
            Damage *= 1.4f;
        }

        // Generate loot from the loot table instead of always Chainmail
        var loot = Items.LootTable.GenerateLoot(mapDifficulty);
        if (loot != null) LootTable.Add(loot);
    }

    /// <summary>
    /// Attempt random movement in one of 4 directions.
    /// Returns the old position if moved (caller handles rendering), null otherwise.
    /// </summary>
    public Vec2? TryRandomMove(Map map)
    {
        if (!IsAlive) return null;

        int direction = Rng.Next(0, 1000); // Low chance of moving each tick
        if (direction > 3) return null;

        var oldPos = Position;
        var newPos = direction switch
        {
            0 => new Vec2(Position.X, Position.Y + 1),
            1 => new Vec2(Position.X, Position.Y - 1),
            2 => new Vec2(Position.X + 1, Position.Y),
            3 => new Vec2(Position.X - 1, Position.Y),
            _ => Position
        };

        if (newPos != oldPos && map.IsWalkable(newPos))
        {
            Position = newPos;
            return oldPos;
        }

        return null;
    }
}