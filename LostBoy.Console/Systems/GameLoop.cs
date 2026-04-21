using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;
using LostBoy.Rendering;

namespace LostBoy.Systems;

public class GameLoop
{
    private Player _player = new();
    private Map? _currentMap;

    public void Run()
    {
        SetupConsole();
        MainMenu();
    }

    // ── Main Menu ──────────────────────────────────────────

    private void MainMenu()
    {
        bool firstTime = true;

        while (true)
        {
            Console.Clear();
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.Magenta;

            if (OperatingSystem.IsWindows())
            {
                try
                {
                    Console.SetWindowSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);
                    Console.SetBufferSize(Console.LargestWindowWidth / 2, Console.LargestWindowHeight / 2);
                }
                catch { }
            }

            if (firstTime)
            {
                Renderer.TypewriterText(TitleArt);
                firstTime = false;
            }
            else
            {
                Console.Write(TitleArt);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(MenuText);

            switch (Input.ReadChoice())
            {
                case 1:
                    NewGame();
                    return;
                case 2:
                    if (LoadGame()) return;
                    break;
                case 3:
                    ShowControls();
                    break;
                case 4:
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("\n   Goodbye... For now.\n");
                    Thread.Sleep(2000);
                    Environment.Exit(0);
                    return;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n   Please input a valid choice.");
                    Thread.Sleep(1500);
                    break;
            }
        }
    }

    private void NewGame()
    {
        _player = new Player();
        _player.Bag.AddItem(new RustySword(), 1);
        _player.Bag.AddItem(new Potion(), 3);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        Renderer.TypewriterText(StoryContent.Introduction, 10);

        Console.ForegroundColor = ConsoleColor.White;
        Console.Write("\n   Enter your name: ");
        string name = Input.ReadLineClean();
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("   Enter your name: ");
            name = Input.ReadLineClean();
        }
        _player.SetName(name);

        Console.ForegroundColor = ConsoleColor.Blue;
        Renderer.TypewriterText(StoryContent.AfterIntro(name));
        Thread.Sleep(1500);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Renderer.TypewriterText(StoryContent.TutorialTransition, 15);
        Thread.Sleep(2000);

        var tutorial = Map.CreateTutorialMap();
        EnterMap(tutorial, showTutorial: true);
    }

    private bool LoadGame()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n   Select your save file:\n");

        var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml", SearchOption.AllDirectories);
        if (files.Length == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   No save files found!");
            Thread.Sleep(1500);
            return false;
        }

        for (int i = 0; i < files.Length; i++)
        {
            Console.WriteLine($"   {i + 1}. {Path.GetFileNameWithoutExtension(files[i])}");
        }

        Console.Write("\n   Choice: ");
        int choice = Input.ReadChoice();
        if (choice < 1 || choice > files.Length)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   Invalid selection!");
            Thread.Sleep(1500);
            return false;
        }

        var result = SaveSystem.Load(files[choice - 1]);
        if (result == null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("   Invalid save file!");
            Thread.Sleep(1500);
            return false;
        }

        (_player, _currentMap) = result.Value;
        EnterMap(_currentMap);
        return true;
    }

    private void ShowControls()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(ControlsText);
        Console.ReadKey(true);
    }

    // ── Map / Exploration Loop ─────────────────────────────

    private void EnterMap(Map map, bool showTutorial = false)
    {
        _currentMap = map;
        _player.ResetPosition(map.Width, map.Height);
        Renderer.DrawFullMap(map, _player);

        if (showTutorial)
        {
            Renderer.DrawTutorialOverlay();
            Renderer.DrawFullMap(map, _player);
        }

        ExplorationLoop(map);
    }

    private void ExplorationLoop(Map map)
    {
        while (true)
        {
            var enemy = map.GetEnemyAt(_player.Position);
            if (enemy != null)
            {
                bool won = RunCombatEncounter(_player, enemy);
                if (!won && !_player.IsAlive)
                {
                    _player.Stats.Health = 1;
                }
                _player.ResetPosition(map.Width, map.Height);
                Renderer.DrawFullMap(map, _player);
            }

            if (map.Enemies.All(e => !e.IsAlive))
            {
                HandleMapCleared(map);
                return;
            }

            foreach (var e in map.Enemies)
            {
                if (!e.IsAlive) continue;
                var oldPos = e.TryRandomMove(map);
                if (oldPos.HasValue)
                {
                    Renderer.MoveEntity(e, oldPos.Value, map);
                }
            }

            HandlePlayerMovement(map);

            if (Input.IsKeyDown(Input.VK_I))
            {
                Input.ConsumeKey(Input.VK_I);
                HandleInventory();
                Renderer.DrawFullMap(map, _player);
            }

            if (Input.IsKeyDown(Input.VK_ESCAPE))
            {
                Input.ConsumeKey(Input.VK_ESCAPE);
                if (HandlePauseMenu(map)) return;
                Renderer.DrawFullMap(map, _player);
            }

            Thread.Sleep(10);
        }
    }

    /// <summary>
    /// Console combat encounter — interactive loop with keyboard input.
    /// </summary>
    private bool RunCombatEncounter(Player player, Enemy enemy)
    {
        Renderer.DrawCombatScreen(player, enemy);

        while (player.IsAlive && enemy.IsAlive)
        {
            if (Input.IsKeyDown(Input.VK_K))
            {
                CombatSystem.ExchangeBlows(player, enemy);
                Renderer.UpdateCombatHealth(player, enemy);
                Thread.Sleep(100);
            }

            if (Input.IsKeyDown(Input.VK_R))
            {
                return false;
            }

            Thread.Sleep(10);
        }

        if (!player.IsAlive)
        {
            Renderer.DrawDefeat();
            player.Stats.Health = 0;
            return false;
        }

        Renderer.DrawVictory(enemy);
        var loot = CombatSystem.ResolveCombatVictory(player, enemy);
        foreach (var item in loot)
        {
            Renderer.WriteAt(2, 16, $"  Looted: {item.Name}");
            Thread.Sleep(800);
        }

        return true;
    }

    private void HandleMapCleared(Map map)
    {
        Console.Clear();

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(60, 15);
                Console.SetBufferSize(60, 15);
            }
            catch { }
        }

        Console.ForegroundColor = ConsoleColor.Yellow;
        Renderer.DrawBox(5, 2, 50, 8, " Area Cleared! ");
        Console.ForegroundColor = ConsoleColor.White;
        Renderer.WriteAt(8, 4, $"You've cleared {map.Name}!");
        Renderer.WriteAt(8, 6, "A new area awaits...");
        Renderer.WriteAt(8, 8, "Press any key to continue.");
        Console.ReadKey(true);

        Map next;
        if (map.Difficulty <= 5)
            next = Map.CreateDungeon();
        else if (map.Difficulty <= 25)
            next = Map.CreateCastle();
        else
            next = new Map(130, 50, map.Difficulty + 15, "The Abyss");

        EnterMap(next);
    }

    private void HandlePlayerMovement(Map map)
    {
        var oldPos = _player.Position;
        Vec2 newPos = oldPos;
        bool moved = false;

        if (Input.IsKeyDown(Input.VK_W))
        { newPos = new Vec2(oldPos.X, oldPos.Y - 1); moved = true; }
        else if (Input.IsKeyDown(Input.VK_S))
        { newPos = new Vec2(oldPos.X, oldPos.Y + 1); moved = true; }
        else if (Input.IsKeyDown(Input.VK_A))
        { newPos = new Vec2(oldPos.X - 1, oldPos.Y); moved = true; }
        else if (Input.IsKeyDown(Input.VK_D))
        { newPos = new Vec2(oldPos.X + 1, oldPos.Y); moved = true; }

        if (moved && map.IsWalkable(newPos))
        {
            _player.Position = newPos;
            Renderer.MoveEntity(_player, oldPos, map);
            Renderer.DrawHud(map, _player);
            Thread.Sleep(50);
        }
    }

    // ── Inventory UI ────────────────────────────────────────

    private void HandleInventory()
    {
        if (_player.Bag.Items.Count == 0)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n   You have no items currently!");
            Thread.Sleep(1500);
            return;
        }

        Renderer.DrawInventory(_player);
        bool needsRedraw = false;

        while (true)
        {
            if (needsRedraw)
            {
                Renderer.DrawInventory(_player);
                needsRedraw = false;
            }

            if (Input.IsKeyDown(Input.VK_ESCAPE, 200))
                return;

            if (Input.IsKeyDown(Input.VK_E))
            {
                int detailRow = 13 + _player.Bag.Items.Count + 3;

                for (int i = 0; i < 3; i++)
                {
                    Console.SetCursorPosition(2, detailRow + i);
                    Console.Write(new string(' ', 60));
                }

                Console.ForegroundColor = ConsoleColor.White;
                Renderer.WriteAt(2, detailRow, "Select item #: ");
                Console.CursorVisible = true;
                int choice = Input.ReadChoice();
                Console.CursorVisible = false;

                var item = _player.Bag.Items.FirstOrDefault(i => i.InventorySlot == choice);
                if (item == null) continue;

                Renderer.DrawItemDetail(item, detailRow + 2);

                int promptRow = detailRow + 9;

                if (item.IsEquippable && !item.IsEquipped)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Renderer.WriteAt(4, promptRow, "Equip this item? (y/n): ");
                    Console.CursorVisible = true;
                    if (Input.ReadLineClean().ToLower() == "y")
                    {
                        if (_player.EquipItem(item))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Renderer.WriteAt(4, promptRow + 1, $"Equipped {item.Name}!");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Renderer.WriteAt(4, promptRow + 1, "Cannot equip — level too low.");
                        }
                        Thread.Sleep(1000);
                        needsRedraw = true;
                    }
                    Console.CursorVisible = false;
                }
                else if (item.IsEquipped)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Renderer.WriteAt(4, promptRow, "Already equipped.");
                    Thread.Sleep(800);
                }
                else if (item.IsConsumable)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Renderer.WriteAt(4, promptRow, "Use this item? (y/n): ");
                    Console.CursorVisible = true;
                    if (Input.ReadLineClean().ToLower() == "y")
                    {
                        if (_player.UseItem(item))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Renderer.WriteAt(4, promptRow + 1, $"Used {item.Name}! (x{item.Quantity} remaining)");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Renderer.WriteAt(4, promptRow + 1, "Cannot use — level too low.");
                        }
                        Thread.Sleep(1000);
                        needsRedraw = true;
                    }
                    Console.CursorVisible = false;
                }
            }

            Thread.Sleep(10);
        }
    }

    // ── Pause Menu ─────────────────────────────────────────

    private bool HandlePauseMenu(Map map)
    {
        Thread.Sleep(200);
        Renderer.DrawPauseMenu();

        switch (Input.ReadChoice())
        {
            case 1:
                Console.Write("\n   Save file name: ");
                string fileName = Input.ReadLineClean();
                if (string.IsNullOrWhiteSpace(fileName)) fileName = _player.Name;
                SaveSystem.Save(_player, map, fileName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   Saved!");
                Thread.Sleep(1500);
                return false;

            case 2:
                Environment.Exit(0);
                return true;

            case 3:
                MainMenu();
                return true;

            case 4:
            default:
                return false;
        }
    }

    // ── Console Setup ──────────────────────────────────────

    private static void SetupConsole()
    {
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.Clear();
        Console.Title = "Lost Boy";
        Console.CursorVisible = false;
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(
                    Console.LargestWindowWidth / 2,
                    Console.LargestWindowHeight - 10);
            }
            catch { }

            DisableWindowResize();
        }
    }

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

    [System.Runtime.InteropServices.DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    private static void DisableWindowResize()
    {
        if (!OperatingSystem.IsWindows()) return;
        try
        {
            const int MF_BYCOMMAND = 0x00000000;
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            DeleteMenu(sysMenu, 0xF000, MF_BYCOMMAND);
            DeleteMenu(sysMenu, 0xF020, MF_BYCOMMAND);
            DeleteMenu(sysMenu, 0xF030, MF_BYCOMMAND);
        }
        catch { }
    }

    // ── Static Text ────────────────────────────────────────

    private static readonly string TitleArt = @"
   ┌─────────────────────────────────────────────────────────────────┐
   │   ___        ________   ________   _________                    │
   │  |\  \      |\   __  \ |\   ____\ |\___   ___\                  │
   │  \ \  \     \ \  \|\  \\ \  \___|_\|___ \  \_|                  │
   │   \ \  \     \ \  \\\  \\ \_____  \    \ \  \                   │
   │    \ \  \____ \ \  \\\  \\|____|\  \    \ \  \                  │
   │     \ \_______\\ \_______\ ____\_\  \    \ \__\                 │
   │      \|_______| \|_______||\_________\    \|__|                 │
   │                            \|_________|                         │
   │                                                                 │
   │         ________   ________       ___    ___                    │
   │        |\   __  \ |\   __  \     |\  \  /  /|                   │
   │        \ \  \|\ /_\ \  \|\  \    \ \  \/  / /                   │
   │         \ \   __  \\ \  \\\  \    \ \    / /                    │
   │          \ \  \|\  \\ \  \\\  \    \/  /  /                     │
   │           \ \_______\\ \_______\ __/  / /                       │
   │            \|_______| \|_______||\___/ /                        │
   │                                  \|___|/                        │
   └─────────────────────────────────────────────────────────────────┘
";

    private static readonly string MenuText = @"
   ┌──────────────────────────────┐
   │   1.  New Game               │
   │   2.  Load Game              │
   │   3.  Controls               │
   │   4.  Exit                   │
   └──────────────────────────────┘
";

    private static readonly string ControlsText = @"
   ┌──────────────────────────────────┐
   │          CONTROLS                │
   ├──────────────────────────────────┤
   │                                  │
   │   W     Move Forward             │
   │   A     Move Left                │
   │   S     Move Backward            │
   │   D     Move Right               │
   │   I     Open Inventory           │
   │   ESC   Pause / Save Game        │
   │                                  │
   │   In Combat:                     │
   │   K     Attack                   │
   │   R     Run Away                 │
   │                                  │
   │   Tips:                          │
   │   · Walk into enemies to fight   │
   │   · Equip armor for defense      │
   │   · Use potions to heal          │
   │   · Clear all enemies to advance │
   │                                  │
   └──────────────────────────────────┘

   Press any key to continue...
";
}
