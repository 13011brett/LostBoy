using LostBoy.Core;

namespace LostBoy.Items;

public enum ItemSlot
{
    None, Head, Shoulders, Chest, Gloves, Belt, Legs, Feet, Necklace, Ring, Hands
}

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

    public bool Consume(Stats targetStats)
    {
        if (!IsConsumable || Quantity <= 0) return false;
        if (BonusStats.Health > 0)
            targetStats.Health = Math.Min(targetStats.Health + BonusStats.Health, targetStats.MaxHealth);
        if (BonusStats.Mana > 0)
            targetStats.Mana = Math.Min(targetStats.Mana + BonusStats.Mana, targetStats.MaxMana);
        Quantity--;
        return true;
    }
}

public class Chainmail : Item
{
    public Chainmail()
    {
        int armor = Rng.Next(30, 150);
        int health = Rng.Next(20, 100);
        BonusStats = new StatsBuilder().SetArmor(armor).SetHealth(health).SetRequiredLevel(2).Build();
        Name = "Chainmail" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Chest;
    }
}

public class IronHelm : Item
{
    public IronHelm()
    {
        int armor = Rng.Next(15, 80);
        int health = Rng.Next(10, 60);
        BonusStats = new StatsBuilder().SetArmor(armor).SetHealth(health).SetRequiredLevel(1).Build();
        Name = "Iron Helm" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Head;
    }
}

public class LeatherBoots : Item
{
    public LeatherBoots()
    {
        int armor = Rng.Next(10, 50);
        int dex = Rng.Next(5, 20);
        BonusStats = new StatsBuilder().SetArmor(armor).SetDexterity(dex).SetRequiredLevel(1).Build();
        Name = "Leather Boots" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Feet;
    }
}

public class SteelGauntlets : Item
{
    public SteelGauntlets()
    {
        int armor = Rng.Next(10, 60);
        int str = Rng.Next(5, 25);
        BonusStats = new StatsBuilder().SetArmor(armor).SetStrength(str).SetRequiredLevel(3).Build();
        Name = "Steel Gauntlets" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Gloves;
    }
}

public class RustySword : Item
{
    public RustySword()
    {
        int ap = Rng.Next(20, 80);
        BonusStats = new StatsBuilder().SetAttackPower(ap).SetRequiredLevel(1).Build();
        Name = "Rusty Sword" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Hands;
    }
}

public class BattleAxe : Item
{
    public BattleAxe()
    {
        int ap = Rng.Next(40, 120);
        int str = Rng.Next(5, 15);
        BonusStats = new StatsBuilder().SetAttackPower(ap).SetStrength(str).SetRequiredLevel(3).Build();
        Name = "Battle Axe" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Hands;
    }
}

public class AmuletOfVitality : Item
{
    public AmuletOfVitality()
    {
        int health = Rng.Next(50, 200);
        int vit = Rng.Next(5, 20);
        BonusStats = new StatsBuilder().SetHealth(health).SetVitality(vit).SetRequiredLevel(2).Build();
        Name = "Amulet" + BonusStats.GetAffix();
        IsEquippable = true;
        Slot = ItemSlot.Necklace;
    }
}

public class Potion : Item
{
    public Potion()
    {
        BonusStats = new StatsBuilder().SetHealth(100).Build();
        Name = "Health Potion";
        IsConsumable = true;
        IsEquippable = false;
        MaxQuantity = 10;
    }
}

public class GreaterPotion : Item
{
    public GreaterPotion()
    {
        BonusStats = new StatsBuilder().SetHealth(300).Build();
        Name = "Greater Health Potion";
        IsConsumable = true;
        IsEquippable = false;
        MaxQuantity = 5;
    }
}

public static class LootTable
{
    public static Item? GenerateLoot(int mapDifficulty)
    {
        if (Rng.Next(100) < 40) return null;
        int roll = Rng.Next(100);

        if (mapDifficulty <= 5)
        {
            return roll switch
            {
                < 20 => new Potion(),
                < 40 => new IronHelm(),
                < 55 => new LeatherBoots(),
                < 70 => new RustySword(),
                < 85 => new Chainmail(),
                _ => new AmuletOfVitality(),
            };
        }
        else if (mapDifficulty <= 25)
        {
            return roll switch
            {
                < 15 => new Potion(),
                < 25 => new GreaterPotion(),
                < 40 => new Chainmail(),
                < 55 => new SteelGauntlets(),
                < 70 => new BattleAxe(),
                < 85 => new AmuletOfVitality(),
                _ => new IronHelm(),
            };
        }
        else
        {
            return roll switch
            {
                < 15 => new GreaterPotion(),
                < 30 => new BattleAxe(),
                < 45 => new SteelGauntlets(),
                < 60 => new Chainmail(),
                < 75 => new AmuletOfVitality(),
                < 90 => new LeatherBoots(),
                _ => new RustySword(),
            };
        }
    }
}
