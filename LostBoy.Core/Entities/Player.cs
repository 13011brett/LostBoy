using LostBoy.Core;
using LostBoy.Items;

namespace LostBoy.Entities;

public class Player : Entity
{
    public Inventory Bag { get; } = new();
    public int ExperienceRequired { get; private set; }

    public Player()
    {
        Name = "Unnamed Hero";
        Icon = '@';
        Color = "#22C55E"; // green
        Level = 1;
        ExperienceRequired = 100;

        Stats = new StatsBuilder()
            .SetHealth(120)
            .SetAttackPower(50)
            .SetArmor(20)
            .Build();

        Damage = (Stats.AttackPower / 10f) + (5 * Level);
    }

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

    public bool EquipItem(Item item)
    {
        if (!item.IsEquippable || item.IsEquipped) return false;
        if (Level < item.BonusStats.RequiredLevel) return false;

        var existing = Bag.Items.FirstOrDefault(i => i.Slot == item.Slot && i.IsEquipped);
        if (existing != null) UnequipItem(existing);

        Stats.Add(item.BonusStats);
        item.IsEquipped = true;
        return true;
    }

    public void UnequipItem(Item item)
    {
        if (!item.IsEquipped) return;
        Stats.Remove(item.BonusStats);
        item.IsEquipped = false;
    }

    public bool UseItem(Item item)
    {
        if (!item.IsConsumable) return false;
        if (Level < item.BonusStats.RequiredLevel) return false;
        bool used = item.Consume(Stats);
        if (used) Bag.PruneEmpty();
        return used;
    }

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

    public void ResetPosition(int mapWidth, int mapHeight)
    {
        Position = new Vec2(mapWidth / 2, mapHeight - 2);
    }

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
