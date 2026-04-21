using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;

namespace LostBoy.Rendering;

/// <summary>
/// Handles all console rendering. Game logic classes no longer call Console.Write directly.
/// This separation makes it possible to swap rendering backends in the future.
/// </summary>
public static class Renderer
{
    // Box-drawing characters for a cleaner look
    private const char BORDER_H = '─';
    private const char BORDER_V = '│';
    private const char CORNER_TL = '┌';
    private const char CORNER_TR = '┐';
    private const char CORNER_BL = '└';
    private const char CORNER_BR = '┘';

    /// <summary>
    /// Set up the console window for a map.
    /// </summary>
    public static void InitializeMap(Map map)
    {
        Console.Clear();
        Console.CursorVisible = false;

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(map.Width, map.Height);
                Console.SetBufferSize(map.Width, map.Height);
            }
            catch { /* Ignore if we can't resize */ }
        }
    }

    /// <summary>
    /// Draw the map border using box-drawing characters.
    /// </summary>
    public static void DrawBorder(Map map)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        // Top border
        Console.SetCursorPosition(0, 0);
        Console.Write(CORNER_TL);
        Console.Write(new string(BORDER_H, map.Width - 2));
        Console.Write(CORNER_TR);

        // Side borders
        for (int y = 1; y < map.Height - 1; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(BORDER_V);
            Console.SetCursorPosition(map.Width - 1, y);
            Console.Write(BORDER_V);
        }

        // Bottom border
        Console.SetCursorPosition(0, map.Height - 1);
        Console.Write(CORNER_BL);
        Console.Write(new string(BORDER_H, map.Width - 2));
        Console.Write(CORNER_BR);

        Console.ForegroundColor = prevColor;
    }

    /// <summary>
    /// Draw all living enemies on the map.
    /// </summary>
    public static void DrawEnemies(Map map)
    {
        foreach (var enemy in map.Enemies)
        {
            if (!enemy.IsAlive) continue;

            // Clamp to inside border
            var pos = enemy.Position;
            if (pos.X <= 0 || pos.X >= map.Width - 1 ||
                pos.Y <= 0 || pos.Y >= map.Height - 1)
                continue;

            DrawEntity(enemy);
        }
    }

    /// <summary>
    /// Draw a single entity at its position.
    /// </summary>
    public static void DrawEntity(Entity entity)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = entity.Color;
        Console.SetCursorPosition(entity.Position.X, entity.Position.Y);
        Console.Write(entity.Icon);
        Console.ForegroundColor = prevColor;
    }

    /// <summary>
    /// Clear a position on screen.
    /// </summary>
    public static void ClearPosition(Vec2 pos)
    {
        Console.SetCursorPosition(pos.X, pos.Y);
        Console.Write(' ');
    }

    /// <summary>
    /// Clear a position and redraw the entity at its new position.
    /// </summary>
    public static void MoveEntity(Entity entity, Vec2 oldPos)
    {
        ClearPosition(oldPos);
        DrawEntity(entity);
    }

    /// <summary>
    /// Draw the full map (border + enemies + player).
    /// </summary>
    public static void DrawFullMap(Map map, Player player)
    {
        InitializeMap(map);
        DrawBorder(map);
        DrawEnemies(map);
        DrawEntity(player);
    }

    /// <summary>
    /// Draw a combat screen.
    /// </summary>
    public static void DrawCombatScreen(Player player, Enemy enemy)
    {
        Console.Clear();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(Math.Min(80, Console.LargestWindowWidth),
                                       Math.Min(30, Console.LargestWindowHeight));
            }
            catch { }
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.SetCursorPosition(0, 0);

        DrawBox(0, 0, 40, 7, "Your Stats");
        WriteAt(2, 2, $"Health: {FormatHealthBar(player.Stats.Health, player.Stats.MaxHealth)}");
        WriteAt(2, 3, $"Level:  {player.Level}");
        WriteAt(2, 4, $"EXP:    {player.Experience}/{player.ExperienceRequired}");
        WriteAt(2, 5, $"Damage: {player.Damage:F1}");

        DrawBox(42, 0, 36, 7, $"{enemy.Name}");
        Console.ForegroundColor = enemy.Color;
        WriteAt(44, 2, $"Health: {FormatHealthBar(enemy.Stats.Health, enemy.Stats.MaxHealth)}");
        Console.ForegroundColor = ConsoleColor.White;
        WriteAt(44, 3, $"Level:  {enemy.Level}");
        WriteAt(44, 4, $"Type:   {enemy.MonsterType}");

        WriteAt(2, 9, "╔══════════════════════════════╗");
        WriteAt(2, 10, "║  [K] Attack    [R] Run Away  ║");
        WriteAt(2, 11, "╚══════════════════════════════╝");
    }

    /// <summary>
    /// Show a victory message after defeating an enemy.
    /// </summary>
    public static void DrawVictory(Enemy enemy)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteAt(2, 13, $"★ {enemy.Name} vanquished! +{enemy.Experience} EXP");
        Console.ForegroundColor = ConsoleColor.White;
        Thread.Sleep(1500);
    }

    /// <summary>
    /// Show a defeat message.
    /// </summary>
    public static void DrawDefeat()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteAt(2, 13, "You are too weak to fight! Retreating...");
        Console.ForegroundColor = ConsoleColor.White;
        Thread.Sleep(2000);
    }

    /// <summary>
    /// Draw the inventory screen.
    /// </summary>
    public static void DrawInventory(Player player)
    {
        Console.Clear();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(Math.Min(80, Console.LargestWindowWidth),
                                       Math.Min(40, Console.LargestWindowHeight));
                Console.SetBufferSize(Math.Min(80, Console.LargestWindowWidth),
                                       Math.Min(40, Console.LargestWindowHeight));
            }
            catch { }
        }

        Console.ForegroundColor = ConsoleColor.White;

        DrawBox(0, 0, 50, 10, "Character Stats");
        int row = 2;
        WriteAt(2, row++, $"Name:    {player.Name}");
        WriteAt(2, row++, $"Level:   {player.Level}");
        WriteAt(2, row++, $"Health:  {FormatHealthBar(player.Stats.Health, player.Stats.MaxHealth)}");
        WriteAt(2, row++, $"EXP:     {player.Experience}/{player.ExperienceRequired}");
        WriteAt(2, row++, $"Attack:  {player.Stats.AttackPower}");
        WriteAt(2, row++, $"Armor:   {player.Stats.Armor:F0}");
        WriteAt(2, row++, $"Damage:  {player.Damage:F1}");

        row = 12;
        DrawBox(0, 11, 70, player.Bag.Items.Count + 4, "Inventory");
        WriteAt(2, row, "#   Item Name                    Qty   Status");
        WriteAt(2, row + 1, new string('─', 50));
        row += 2;

        int slot = 1;
        foreach (var item in player.Bag.Items)
        {
            item.InventorySlot = slot;
            string status = item.IsEquipped ? "[Equipped]" : "";
            string qty = item.MaxQuantity > 1 ? $"x{item.Quantity}" : "";

            if (item.IsEquipped)
                Console.ForegroundColor = ConsoleColor.Cyan;
            else
                Console.ForegroundColor = ConsoleColor.White;

            WriteAt(2, row, $"{slot,-4}{item.Name,-32}{qty,-6}{status}");
            row++;
            slot++;
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        row += 2;
        WriteAt(2, row, "[E] Equip/Use Item    [ESC] Close Inventory");
        Console.ForegroundColor = ConsoleColor.White;
    }

    /// <summary>
    /// Draw the pause menu.
    /// </summary>
    public static void DrawPauseMenu()
    {
        Console.Clear();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(40, 15);
                Console.SetBufferSize(40, 15);
            }
            catch { }
        }

        Console.ForegroundColor = ConsoleColor.White;
        DrawBox(2, 1, 35, 10, "Paused");
        WriteAt(5, 3, "1.  Save Game");
        WriteAt(5, 5, "2.  Quit Game");
        WriteAt(5, 7, "3.  Return to Main Menu");
        WriteAt(5, 9, "4.  Continue Game");
    }

    /// <summary>
    /// Draw a titled box at a position.
    /// </summary>
    public static void DrawBox(int x, int y, int width, int height, string title = "")
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        // Top
        Console.SetCursorPosition(x, y);
        Console.Write(CORNER_TL);
        if (!string.IsNullOrEmpty(title))
        {
            string titleBar = $"─ {title} ";
            Console.Write(titleBar);
            Console.Write(new string(BORDER_H, Math.Max(0, width - titleBar.Length - 2)));
        }
        else
        {
            Console.Write(new string(BORDER_H, width - 2));
        }
        Console.Write(CORNER_TR);

        // Sides
        for (int row = 1; row < height - 1; row++)
        {
            Console.SetCursorPosition(x, y + row);
            Console.Write(BORDER_V);
            Console.SetCursorPosition(x + width - 1, y + row);
            Console.Write(BORDER_V);
        }

        // Bottom
        Console.SetCursorPosition(x, y + height - 1);
        Console.Write(CORNER_BL);
        Console.Write(new string(BORDER_H, width - 2));
        Console.Write(CORNER_BR);

        Console.ForegroundColor = prevColor;
    }

    /// <summary>
    /// Write text at a specific position.
    /// </summary>
    public static void WriteAt(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    /// <summary>
    /// Generate a visual health bar string.
    /// </summary>
    public static string FormatHealthBar(float current, float max)
    {
        const int barWidth = 15;
        float pct = max > 0 ? current / max : 0;
        int filled = (int)(pct * barWidth);
        int empty = barWidth - filled;

        string bar = new string('█', Math.Max(0, filled)) + new string('░', Math.Max(0, empty));
        return $"{bar} {current:F0}/{max:F0}";
    }

    /// <summary>
    /// Display text character by character with typewriter effect.
    /// Press Escape to skip.
    /// </summary>
    public static void TypewriterText(string text, int delayMs = 10)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Console.Write(text[i]);
            Thread.Sleep(delayMs);

            if (Input.IsKeyDown(Input.VK_ESCAPE))
            {
                Console.Write(text[(i + 1)..]);
                break;
            }
        }
    }

    /// <summary>
    /// Transition effect — fills screen then clears.
    /// </summary>
    public static void TransitionClear()
    {
        Console.Clear();
    }
}
