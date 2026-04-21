using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;

namespace LostBoy.Rendering;

public static class Renderer
{
    private const char H = '─';
    private const char V = '│';
    private const char TL = '┌';
    private const char TR = '┐';
    private const char BL = '└';
    private const char BR = '┘';

    // ── Color Mapping ───────────────────────────────────────

    private static ConsoleColor HexToConsoleColor(string hex) => hex switch
    {
        "#22C55E" => ConsoleColor.Green,
        "#AAAAAA" => ConsoleColor.Gray,
        "#6B8E23" => ConsoleColor.DarkGreen,
        "#8B4513" => ConsoleColor.DarkYellow,
        "#228B22" => ConsoleColor.Green,
        "#9370DB" => ConsoleColor.Magenta,
        "#CD853F" => ConsoleColor.Yellow,
        "#DC143C" => ConsoleColor.Red,
        "#FF4500" => ConsoleColor.Red,
        "#555555" => ConsoleColor.DarkGray,
        "#888888" => ConsoleColor.Gray,
        "#333333" => ConsoleColor.DarkGray,
        "#D4A017" => ConsoleColor.DarkYellow,
        "#2E8B57" => ConsoleColor.DarkGreen,
        "#1E90FF" => ConsoleColor.DarkCyan,
        _ => ConsoleColor.White,
    };

    // ── Map Rendering ───────────────────────────────────────

    public static void InitializeMap(Map map)
    {
        Console.Clear();
        Console.CursorVisible = false;

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(map.Width, map.Height + 1);
                Console.SetBufferSize(map.Width, map.Height + 1);
            }
            catch { }
        }
    }

    public static void DrawBorder(Map map)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.SetCursorPosition(0, 0);
        Console.Write(TL);
        string titleBar = $"{H} {map.Name} ";
        Console.Write(titleBar);
        Console.Write(new string(H, Math.Max(0, map.Width - titleBar.Length - 2)));
        Console.Write(TR);

        for (int y = 1; y < map.Height - 1; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(V);
            Console.SetCursorPosition(map.Width - 1, y);
            Console.Write(V);
        }

        Console.SetCursorPosition(0, map.Height - 1);
        Console.Write(BL);
        Console.Write(new string(H, map.Width - 2));
        Console.Write(BR);

        Console.ForegroundColor = prev;
    }

    public static void DrawTerrain(Map map)
    {
        for (int y = 1; y < map.Height - 1; y++)
        {
            for (int x = 1; x < map.Width - 1; x++)
            {
                var tile = map.Terrain[x, y];
                if (tile.Glyph != ' ')
                {
                    Console.ForegroundColor = HexToConsoleColor(tile.Color);
                    Console.SetCursorPosition(x, y);
                    Console.Write(tile.Glyph);
                }
            }
        }
    }

    public static void DrawEnemies(Map map)
    {
        foreach (var enemy in map.Enemies)
        {
            if (!enemy.IsAlive) continue;
            var pos = enemy.Position;
            if (pos.X <= 0 || pos.X >= map.Width - 1 ||
                pos.Y <= 0 || pos.Y >= map.Height - 1)
                continue;
            DrawEntity(enemy);
        }
    }

    public static void DrawHud(Map map, Player player)
    {
        int hudY = map.Height;
        var prev = Console.ForegroundColor;

        Console.SetCursorPosition(0, hudY);
        Console.Write(new string(' ', map.Width));

        Console.ForegroundColor = ConsoleColor.Red;
        WriteAt(1, hudY, $"HP:{FormatMiniBar(player.Stats.Health, player.Stats.MaxHealth)}");

        Console.ForegroundColor = ConsoleColor.Cyan;
        WriteAt(28, hudY, $"Lv:{player.Level}");

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        WriteAt(34, hudY, $"EXP:{player.Experience}/{player.ExperienceRequired}");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        string hint = "[I]nventory [ESC]Pause";
        int hintX = Math.Max(0, map.Width - hint.Length - 2);
        WriteAt(hintX, hudY, hint);

        Console.ForegroundColor = prev;
    }

    public static void DrawEntity(Entity entity)
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = HexToConsoleColor(entity.Color);
        Console.SetCursorPosition(entity.Position.X, entity.Position.Y);
        Console.Write(entity.Icon);
        Console.ForegroundColor = prev;
    }

    public static void ClearPosition(Vec2 pos)
    {
        Console.SetCursorPosition(pos.X, pos.Y);
        Console.Write(' ');
    }

    public static void MoveEntity(Entity entity, Vec2 oldPos, Map map)
    {
        var tile = map.Terrain[oldPos.X, oldPos.Y];
        if (tile.Glyph != ' ')
        {
            Console.ForegroundColor = HexToConsoleColor(tile.Color);
            Console.SetCursorPosition(oldPos.X, oldPos.Y);
            Console.Write(tile.Glyph);
        }
        else
        {
            ClearPosition(oldPos);
        }

        DrawEntity(entity);
    }

    public static void DrawFullMap(Map map, Player player)
    {
        InitializeMap(map);
        DrawBorder(map);
        DrawTerrain(map);
        DrawEnemies(map);
        DrawEntity(player);
        DrawHud(map, player);
    }

    // ── Combat Rendering ────────────────────────────────────

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

        DrawBox(0, 0, 40, 8, $" {player.Name} ");
        Console.ForegroundColor = ConsoleColor.Green;
        WriteAt(2, 2, $"Health: {FormatHealthBar(player.Stats.Health, player.Stats.MaxHealth)}");
        Console.ForegroundColor = ConsoleColor.White;
        WriteAt(2, 3, $"Level:  {player.Level}");
        WriteAt(2, 4, $"EXP:    {player.Experience}/{player.ExperienceRequired}");
        WriteAt(2, 5, $"Damage: {player.Damage:F1}");
        WriteAt(2, 6, $"Armor:  {player.Stats.Armor:F0}");

        DrawBox(42, 0, 36, 8, $" {enemy.Name} ");
        Console.ForegroundColor = HexToConsoleColor(enemy.Color);
        WriteAt(44, 2, $"Health: {FormatHealthBar(enemy.Stats.Health, enemy.Stats.MaxHealth)}");
        Console.ForegroundColor = ConsoleColor.White;
        WriteAt(44, 3, $"Level:  {enemy.Level}");
        WriteAt(44, 4, $"Type:   {enemy.MonsterType}");

        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteAt(2, 10, "╔══════════════════════════════════╗");
        WriteAt(2, 11, "║   [K] Attack      [R] Run Away   ║");
        WriteAt(2, 12, "╚══════════════════════════════════╝");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void UpdateCombatHealth(Player player, Enemy enemy)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        WriteAt(2, 2, $"Health: {FormatHealthBar(player.Stats.Health, player.Stats.MaxHealth)}   ");
        Console.ForegroundColor = HexToConsoleColor(enemy.Color);
        WriteAt(44, 2, $"Health: {FormatHealthBar(enemy.Stats.Health, enemy.Stats.MaxHealth)}   ");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void DrawVictory(Enemy enemy)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteAt(2, 14, $"  ★ {enemy.Name} vanquished! +{enemy.Experience} EXP");
        Console.ForegroundColor = ConsoleColor.White;
        Thread.Sleep(1500);
    }

    public static void DrawDefeat()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        WriteAt(2, 14, "  You are too weak to fight! Retreating...");
        Console.ForegroundColor = ConsoleColor.White;
        Thread.Sleep(2000);
    }

    // ── Inventory Rendering ─────────────────────────────────

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

        DrawBox(0, 0, 50, 10, " Character Stats ");
        int row = 2;
        WriteAt(2, row++, $"Name:    {player.Name}");
        WriteAt(2, row++, $"Level:   {player.Level}");

        Console.ForegroundColor = ConsoleColor.Green;
        WriteAt(2, row++, $"Health:  {FormatHealthBar(player.Stats.Health, player.Stats.MaxHealth)}");
        Console.ForegroundColor = ConsoleColor.White;

        WriteAt(2, row++, $"EXP:     {player.Experience}/{player.ExperienceRequired}");
        WriteAt(2, row++, $"Attack:  {player.Stats.AttackPower}");
        WriteAt(2, row++, $"Armor:   {player.Stats.Armor:F0}");
        WriteAt(2, row++, $"Damage:  {player.Damage:F1}");

        int itemBoxHeight = Math.Max(4, player.Bag.Items.Count + 4);
        DrawBox(0, 11, 70, itemBoxHeight, " Inventory ");

        row = 13;
        Console.ForegroundColor = ConsoleColor.DarkGray;
        WriteAt(2, row - 1, "#   Item Name                    Qty   Status");
        WriteAt(2, row, new string('─', 52));
        row++;

        if (player.Bag.Items.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteAt(4, row, "(empty)");
        }
        else
        {
            int slot = 1;
            foreach (var item in player.Bag.Items)
            {
                item.InventorySlot = slot;
                string status = item.IsEquipped ? "[Equipped]" : "";
                string qty = item.MaxQuantity > 1 ? $"x{item.Quantity}" : "";

                Console.ForegroundColor = item.IsEquipped ? ConsoleColor.Cyan : ConsoleColor.White;
                WriteAt(2, row, $"{slot,-4}{item.Name,-32}{qty,-6}{status}");
                row++;
                slot++;
            }
        }

        Console.ForegroundColor = ConsoleColor.DarkGray;
        row += 2;
        WriteAt(2, row, "[E] Equip/Use Item    [ESC] Close Inventory");
        Console.ForegroundColor = ConsoleColor.White;
    }

    public static void DrawItemDetail(Item item, int startRow)
    {
        for (int i = 0; i < 8; i++)
        {
            Console.SetCursorPosition(2, startRow + i);
            Console.Write(new string(' ', 60));
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        WriteAt(2, startRow, $"── {item.Name} ──");
        Console.ForegroundColor = ConsoleColor.White;

        int r = startRow + 1;
        var s = item.BonusStats;
        if (s.RequiredLevel > 1) WriteAt(4, r++, $"Required Level: {s.RequiredLevel}");
        if (s.Health > 0) WriteAt(4, r++, $"Health:  +{s.Health:F0}");
        if (s.Armor > 0) WriteAt(4, r++, $"Armor:   +{s.Armor:F0}");
        if (s.AttackPower > 0) WriteAt(4, r++, $"Attack:  +{s.AttackPower}");
        if (s.Strength > 0) WriteAt(4, r++, $"Strength: +{s.Strength}");
        if (s.Dexterity > 0) WriteAt(4, r++, $"Dexterity: +{s.Dexterity}");
    }

    // ── Pause Menu ──────────────────────────────────────────

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
        DrawBox(2, 1, 35, 10, " Paused ");
        WriteAt(5, 3, "1.  Save Game");
        WriteAt(5, 5, "2.  Quit Game");
        WriteAt(5, 7, "3.  Return to Main Menu");
        WriteAt(5, 9, "4.  Continue Game");
    }

    // ── Tutorial Overlay ────────────────────────────────────

    public static void DrawTutorialOverlay()
    {
        int cx = 10;
        int cy = 3;

        DrawBox(cx, cy, 42, 12, " Welcome, Adventurer ");

        Console.ForegroundColor = ConsoleColor.White;
        WriteAt(cx + 2, cy + 2, "Move with  W A S D");
        WriteAt(cx + 2, cy + 4, "Walk into enemies to fight them.");
        WriteAt(cx + 2, cy + 5, "In combat: K to attack, R to run.");
        WriteAt(cx + 2, cy + 7, "Press  I  to open your inventory.");
        WriteAt(cx + 2, cy + 8, "Press ESC to pause and save.");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        WriteAt(cx + 2, cy + 10, "Press any key to begin...");

        Console.ReadKey(true);
    }

    // ── Utilities ───────────────────────────────────────────

    public static void DrawBox(int x, int y, int width, int height, string title = "")
    {
        var prev = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGray;

        Console.SetCursorPosition(x, y);
        Console.Write(TL);
        if (!string.IsNullOrEmpty(title))
        {
            string bar = $"{H}{title}";
            Console.Write(bar);
            Console.Write(new string(H, Math.Max(0, width - bar.Length - 2)));
        }
        else
        {
            Console.Write(new string(H, width - 2));
        }
        Console.Write(TR);

        for (int row = 1; row < height - 1; row++)
        {
            Console.SetCursorPosition(x, y + row);
            Console.Write(V);
            Console.Write(new string(' ', width - 2));
            Console.SetCursorPosition(x + width - 1, y + row);
            Console.Write(V);
        }

        Console.SetCursorPosition(x, y + height - 1);
        Console.Write(BL);
        Console.Write(new string(H, width - 2));
        Console.Write(BR);

        Console.ForegroundColor = prev;
    }

    public static void WriteAt(int x, int y, string text)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(text);
    }

    public static string FormatHealthBar(float current, float max)
    {
        const int barWidth = 15;
        float pct = max > 0 ? Math.Clamp(current / max, 0, 1) : 0;
        int filled = (int)(pct * barWidth);
        int empty = barWidth - filled;
        string bar = new string('█', filled) + new string('░', empty);
        return $"{bar} {current:F0}/{max:F0}";
    }

    public static string FormatMiniBar(float current, float max)
    {
        const int barWidth = 10;
        float pct = max > 0 ? Math.Clamp(current / max, 0, 1) : 0;
        int filled = (int)(pct * barWidth);
        int empty = barWidth - filled;
        string bar = new string('█', filled) + new string('░', empty);
        return $"{bar} {current:F0}/{max:F0}";
    }

    public static void TypewriterText(string text, int delayMs = 10)
    {
        for (int i = 0; i < text.Length; i++)
        {
            Console.Write(text[i]);
            Thread.Sleep(delayMs);

            if (Input.IsKeyDown(Input.VK_ESCAPE))
            {
                Console.Write(text[(i + 1)..]);
                Input.ConsumeKey(Input.VK_ESCAPE);
                break;
            }
        }
    }
}
