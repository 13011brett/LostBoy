namespace LostBoy.Items;

public class Inventory
{
    private const int MaxSlots = 10;

    public List<Item> Items { get; } = new();

    public int EquippedCount => Items.Count(i => i.IsEquipped);
    public int FreeSlots => MaxSlots - (Items.Count - EquippedCount);

    public bool AddItem(Item item, int quantity = 1)
    {
        var existing = Items.FirstOrDefault(i => i.Id == item.Id);
        if (existing != null)
        {
            existing.Quantity = Math.Min(existing.Quantity + quantity, existing.MaxQuantity);
            return true;
        }
        if (FreeSlots <= 0) return false;
        item.Quantity = Math.Min(quantity, item.MaxQuantity);
        Items.Add(item);
        return true;
    }

    public void RemoveItem(Item item) => Items.Remove(item);

    public void PruneEmpty() => Items.RemoveAll(i => i.Quantity <= 0 && !i.IsEquipped);
}
