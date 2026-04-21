using LostBoy.Core;
using LostBoy.Items;

namespace LostBoy.Entities;

/// <summary>
/// The player character. No longer a base class for Map or Enemy.
/// Rendering logic has been moved to the Renderer.
/// </summary>
public class Player : Entity
{
    public Inventory Bag { get; } = new();
    public int ExperienceRequired { get; private set; }

    public Player()
    {
        Name = "Unnamed Hero";
        Icon = '@';
        Color = ConsoleColor.Green;
        Level = 1;
        ExperienceRequired = 100;

        Stats = new StatsBuilder()
            .SetHealth(100)
            .SetAttackPower(100)
            .SetArmor(1000)
            .Build();

        Damage = (Stats.AttackPower / 15f) + (10 * Level);
    }

    /// <summary>
    /// Private constructor for loading from save data.
    /// </summary>
    private Player(string name, float hp, float maxHp, int armor) : this()
    {
        Name = name;
        Stats = new StatsBuilder()
            .SetHealth(hp)
            .SetArmor(armor)
            .Build();
        Stats.MaxHealth = maxHp;
    }

    public void SetName(string name) => Name = name;

    /// <summary>
    /// Equip an item, unequipping any existing item in the same slot first.
    /// </summary>
    public bool EquipItem(Item item)
    {
        if (!item.IsEquippable || item.IsEquipped) return false;
        if (Level < item.BonusStats.RequiredLevel) return false;

        // Unequip existing item in same slot
        var existing = Bag.Items.FirstOrDefault(i => i.Slot == item.Slot && i.IsEquipped);
        if (existing != null)
        {
            UnequipItem(existing);
        }

        Stats.Add(item.BonusStats);
        item.IsEquipped = true;
        return true;
    }

    /// <summary>
    /// Unequip an item, removing its stat bonuses.
    /// </summary>
    public void UnequipItem(Item item)
    {
        if (!item.IsEquipped) return;

        Stats.Remove(item.BonusStats);
        item.IsEquipped = false;
    }

    /// <summary>
    /// Use a consumable item.
    /// </summary>
    public bool UseItem(Item item)
    {
        if (!item.IsConsumable) return false;
        if (Level < item.BonusStats.RequiredLevel) return false;

        bool used = item.Consume(Stats);
        if (used) Bag.PruneEmpty();
        return used;
    }

    /// <summary>
    /// Award experience and handle leveling up.
    /// </summary>
    public void GainExperience(int amount)
    {
        Experience += amount;

        while (Experience >= ExperienceRequired)
        {
            Experience -= ExperienceRequired;
            Level++;
            Stats.MaxHealth += 30;
            Stats.Health = Stats.MaxHealth;
            Damage += 5;
            ExperienceRequired = Level * Level * 100;
        }
    }

    /// <summary>
    /// Reset position to center-bottom of a map.
    /// </summary>
    public void ResetPosition(int mapWidth, int mapHeight)
    {
        Position = new Vec2(mapWidth / 2, mapHeight - 2);
    }

    /// <summary>
    /// Create a player from saved XML data.
    /// </summary>
    public static Player FromSaveData(string name, float hp, float maxHp, int armor,
        int level, float damage, int exp, int expReq)
    {
        var p = new Player(name, hp, maxHp, armor)
        {
            Level = level,
            Damage = damage,
            Experience = exp,
            ExperienceRequired = expReq
        };
        return p;
    }
}
