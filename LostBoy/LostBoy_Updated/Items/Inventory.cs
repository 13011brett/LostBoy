namespace LostBoy.Items;

/// <summary>
/// Manages a collection of items with slot limits and equip tracking.
/// </summary>
public class Inventory
{
    private const int MaxSlots = 10;

    public List<Item> Items { get; } = new();

    public int EquippedCount => Items.Count(i => i.IsEquipped);
    public int FreeSlots => MaxSlots - (Items.Count - EquippedCount);

    /// <summary>
    /// Add an item to inventory. Stacks if a matching item exists and has room.
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        // Try to stack with existing item of same type
        var existing = Items.FirstOrDefault(i => i.Id == item.Id);
        if (existing != null)
        {
            existing.Quantity = Math.Min(existing.Quantity + quantity, existing.MaxQuantity);
            return true;
        }

        // Check if we have room (equipped items don't count against bag slots)
        if (FreeSlots <= 0) return false;

        item.Quantity = Math.Min(quantity, item.MaxQuantity);
        Items.Add(item);
        return true;
    }

    /// <summary>
    /// Remove an item from inventory entirely.
    /// </summary>
    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    /// <summary>
    /// Remove items with zero or negative quantity (cleanup after consumption).
    /// </summary>
    public void PruneEmpty()
    {
        Items.RemoveAll(i => i.Quantity <= 0 && !i.IsEquipped);
    }
}
