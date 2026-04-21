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
        Level = mapLevel;
        Position = position;

        Stats = new StatsBuilder()
            .SetAttackPower(50 * Level)
            .SetHealth(Level * Rng.NextFloat(1, 10) + 70)
            .Build();

        Damage = (Stats.AttackPower / 15f) + 5;
        Experience = (Level * mapDifficulty) + Rng.Next(0, mapDifficulty);

        // Set appearance based on monster type
        (Icon, Color, Name) = MonsterType switch
        {
            MonsterType.Troll => ('T', ConsoleColor.Yellow, "Troll"),
            MonsterType.Ogre => ('O', ConsoleColor.Magenta, "Ogre"),
            MonsterType.Demon => ('D', ConsoleColor.DarkRed, "Demon"),
            _ => ('?', ConsoleColor.Gray, "Unknown Beast"),
        };

        // Generate loot
        LootTable.Add(new Chainmail());
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