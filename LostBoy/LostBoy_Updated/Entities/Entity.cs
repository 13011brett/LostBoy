using LostBoy.Core;

namespace LostBoy.Entities;

/// <summary>
/// Base class for all game entities (players, enemies).
/// This replaces both the old unused Entity class AND the Player class as a base for Enemy.
/// Map no longer inherits from this — it uses composition instead.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = "Unknown";
    public Vec2 Position { get; set; }
    public Stats Stats { get; set; } = new();
    public char Icon { get; protected set; } = '?';
    public ConsoleColor Color { get; protected set; } = ConsoleColor.White;
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public float Damage { get; set; }
    public float ArmorFactor { get; set; } = 75f;

    public bool IsAlive => Stats.Health > 0;

    /// <summary>
    /// Apply damage from an attacker to this entity.
    /// Returns the actual damage dealt.
    /// </summary>
    public float TakeDamage(float incomingDamage)
    {
        float reduction = Stats.Armor / ArmorFactor;
        float actual = Math.Max(0, incomingDamage - reduction);
        Stats.Health -= actual;

        if (Stats.Health < 0)
            Stats.Health = 0;

        return actual;
    }
}
