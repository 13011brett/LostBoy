using LostBoy.Core;

namespace LostBoy.Items;

public enum ItemSlot
{
    None,
    Head,
    Shoulders,
    Chest,
    Gloves,
    Belt,
    Legs,
    Feet,
    Necklace,
    Ring,
    Hands
}

/// <summary>
/// Base class for all items that can be picked up, equipped, or consumed.
/// Replaces ObtainableItem (which implemented a fake IConvertible that shadowed System.IConvertible).
/// </summary>
public class Item
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "Unknown Item";
    public Stats BonusStats { get; set; } = new();
    public ItemSlot Slot { get; set; } = ItemSlot.None;
    public bool IsEquippable { get; set; }
    public bool IsEquipped { get; set; }
    public bool IsConsumable { get; set; }
    public int Quantity { get; set; } = 1;
    public int MaxQuantity { get; set; } = 1;
    public int InventorySlot { get; set; }

    /// <summary>
    /// Apply this consumable's effects to a Stats block.
    /// Takes Stats directly instead of Player to avoid circular dependency.
    /// </summary>
    public bool Consume(Stats targetStats)
    {
        if (!IsConsumable || Quantity <= 0) return false;

        // Heal
        if (BonusStats.Health > 0)
        {
            targetStats.Health = Math.Min(
                targetStats.Health + BonusStats.Health,
                targetStats.MaxHealth);
        }

        // Restore mana
        if (BonusStats.Mana > 0)
        {
            targetStats.Mana = Math.Min(
                targetStats.Mana + BonusStats.Mana,
                targetStats.MaxMana);
        }

        Quantity--;
        return true;
    }
}

// ── Concrete Items ──────────────────────────────────────────

public class Chainmail : Item
{
    public Chainmail()
    {
        int armor = Rng.Next(50, 256);
        int health = Rng.Next(50, 256);

        BonusStats = new StatsBuilder()
            .SetArmor(armor)
            .SetHealth(health)
            .SetRequiredLevel(2)
            .Build();

        Name = "Chainmail" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Chest;
    }
}

public class Potion : Item
{
    public Potion()
    {
        BonusStats = new StatsBuilder()
            .SetHealth(1000)
            .Build();

        Name = "Health Potion";
        IsConsumable = true;
        IsEquippable = false;
        MaxQuantity = 10;
    }
}
