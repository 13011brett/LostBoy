using LostBoy.Core;

namespace LostBoy.Entities;

public abstract class Entity
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; } = "Unknown";
    public Vec2 Position { get; set; }
    public Stats Stats { get; set; } = new();
    public char Icon { get; protected set; } = '?';
    public string Color { get; protected set; } = "#FFFFFF";
    public int Level { get; set; } = 1;
    public int Experience { get; set; }
    public float Damage { get; set; }
    public float ArmorFactor { get; set; } = 75f;

    public bool IsAlive => Stats.Health > 0;

    public float TakeDamage(float incomingDamage)
    {
        float reduction = Stats.Armor / ArmorFactor;
        float actual = Math.Max(0, incomingDamage - reduction);
        Stats.Health -= actual;
        if (Stats.Health < 0) Stats.Health = 0;
        return actual;
    }
}
