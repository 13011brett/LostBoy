namespace LostBoy.Core;

/// <summary>
/// Universal stats container used by both entities and items.
/// Replaces the old Stats / EntityStats / ItemStats / ConsumableStats hierarchy.
/// The distinction between entity stats and item stats is now handled by context, not subclasses.
/// </summary>
public class Stats : ICloneable
{
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Mana { get; set; }
    public float MaxMana { get; set; }
    public int AttackPower { get; set; }
    public float Armor { get; set; }
    public int Strength { get; set; }
    public int Dexterity { get; set; }
    public int Vitality { get; set; }
    public int Intelligence { get; set; }
    public float MovementSpeed { get; set; }

    // Item-specific
    public int RequiredLevel { get; set; } = 1;

    public object Clone() => MemberwiseClone();

    public Stats Copy() => (Stats)Clone();

    /// <summary>
    /// Generates an affix string based on the dominant stat.
    /// </summary>
    public string GetAffix()
    {
        var candidates = new (float value, string name)[]
        {
            (Health, " of Health"),
            (Mana, " of Mana"),
            (Armor, " of Defense"),
            (AttackPower, " of Power"),
            (Strength, " of Strength"),
            (Dexterity, " of Dexterity"),
            (Intelligence, " of Intelligence"),
            (Vitality, " of Vitality"),
        };

        string best = "";
        float bestValue = 0;

        foreach (var (value, name) in candidates)
        {
            if (value > bestValue)
            {
                bestValue = value;
                best = name;
            }
        }

        return best;
    }

    /// <summary>
    /// Add another stats block onto this one (for equipping items).
    /// </summary>
    public void Add(Stats other)
    {
        Health += other.Health;
        MaxHealth += other.Health; // Item health adds to max pool
        Mana += other.Mana;
        MaxMana += other.Mana;
        AttackPower += other.AttackPower;
        Armor += other.Armor;
        Strength += other.Strength;
        Dexterity += other.Dexterity;
        Vitality += other.Vitality;
        Intelligence += other.Intelligence;
    }

    /// <summary>
    /// Remove another stats block from this one (for unequipping items).
    /// </summary>
    public void Remove(Stats other)
    {
        Health -= other.Health;
        MaxHealth -= other.Health;
        Mana -= other.Mana;
        MaxMana -= other.Mana;
        AttackPower -= other.AttackPower;
        Armor -= other.Armor;
        Strength -= other.Strength;
        Dexterity -= other.Dexterity;
        Vitality -= other.Vitality;
        Intelligence -= other.Intelligence;
    }
}

/// <summary>
/// Fluent builder for Stats objects.
/// Replaces StatsBuilder, ItemStatsBuilder, and ConsumableStatsBuilder.
/// </summary>
public class StatsBuilder
{
    private readonly Stats _stats = new();

    public StatsBuilder SetHealth(float hp)
    {
        _stats.Health = hp;
        _stats.MaxHealth = hp;
        return this;
    }

    public StatsBuilder SetMana(float mana)
    {
        _stats.Mana = mana;
        _stats.MaxMana = mana;
        return this;
    }

    public StatsBuilder SetAttackPower(int ap)
    {
        _stats.AttackPower = ap;
        return this;
    }

    public StatsBuilder SetArmor(float armor)
    {
        _stats.Armor = armor;
        return this;
    }

    public StatsBuilder SetStrength(int str)
    {
        _stats.Strength = str;
        return this;
    }

    public StatsBuilder SetDexterity(int dex)
    {
        _stats.Dexterity = dex;
        return this;
    }

    public StatsBuilder SetVitality(int vit)
    {
        _stats.Vitality = vit;
        return this;
    }

    public StatsBuilder SetIntelligence(int intel)
    {
        _stats.Intelligence = intel;
        return this;
    }

    public StatsBuilder SetMovementSpeed(float speed)
    {
        _stats.MovementSpeed = speed;
        return this;
    }

    public StatsBuilder SetRequiredLevel(int level)
    {
        _stats.RequiredLevel = level;
        return this;
    }

    public Stats Build() => _stats.Copy();
}
