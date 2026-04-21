using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Rendering;

namespace LostBoy.Systems;

/// <summary>
/// Handles combat encounters between the player and enemies.
/// Extracted from the old Map.AttackSequence static method.
/// </summary>
public static class CombatSystem
{
    /// <summary>
    /// Resolve a single round of mutual damage.
    /// Both sides hit each other simultaneously.
    /// </summary>
    public static void ExchangeBlows(Player player, Enemy enemy)
    {
        player.TakeDamage(enemy.Damage);
        enemy.TakeDamage(player.Damage);
    }

    /// <summary>
    /// Run an interactive combat encounter. Returns true if the player won.
    /// </summary>
    public static bool RunEncounter(Player player, Enemy enemy)
    {
        Renderer.DrawCombatScreen(player, enemy);

        while (player.IsAlive && enemy.IsAlive)
        {
            if (Input.IsKeyDown(Input.VK_K))
            {
                ExchangeBlows(player, enemy);
                Renderer.DrawCombatScreen(player, enemy);
                Thread.Sleep(100);
            }

            if (Input.IsKeyDown(Input.VK_R))
            {
                return false; // Player ran away
            }

            Thread.Sleep(10); // Prevent busy loop
        }

        if (!player.IsAlive)
        {
            Renderer.DrawDefeat();
            player.Stats.Health = 0;
            return false;
        }

        // Player wins
        Renderer.DrawVictory(enemy);
        player.GainExperience(enemy.Experience);

        // Drop loot
        foreach (var loot in enemy.LootTable)
        {
            if (player.Bag.AddItem(loot))
            {
                Renderer.WriteAt(2, 15, $"  Looted: {loot.Name}");
                Thread.Sleep(800);
            }
        }

        return true;
    }
}
