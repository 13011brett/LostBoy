using LostBoy.Entities;
using LostBoy.Items;

namespace LostBoy.Systems;

/// <summary>
/// Pure combat logic — no rendering. The UI layer calls these methods and handles display.
/// </summary>
public static class CombatSystem
{
    public static (float playerDamageTaken, float enemyDamageTaken) ExchangeBlows(Player player, Enemy enemy)
    {
        float pDmg = player.TakeDamage(enemy.Damage);
        float eDmg = enemy.TakeDamage(player.Damage);
        return (pDmg, eDmg);
    }

    /// <summary>
    /// Resolve post-combat rewards. Returns list of looted items.
    /// </summary>
    public static List<Item> ResolveCombatVictory(Player player, Enemy enemy)
    {
        player.GainExperience(enemy.Experience);
        var looted = new List<Item>();
        foreach (var loot in enemy.LootTable)
        {
            if (player.Bag.AddItem(loot))
                looted.Add(loot);
        }
        return looted;
    }
}
