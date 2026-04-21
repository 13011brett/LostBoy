using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;
using LostBoy.Rendering;

namespace LostBoy.Systems;

/// <summary>
/// Main game loop orchestrator.
/// Replaces the old Map.ScreenMovement / Map.DrawMap / Story.DoIntro static method soup.
/// </summary>
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

            if (firstTime)
            {
                Renderer.TypewriterText(StoryContent.Title);
                firstTime = false;
            }
            else
            {
                Console.Write(StoryContent.Title);
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(StoryContent.Menu);

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
        _player.Bag.AddItem(new Chainmail(), 1);
        _player.Bag.AddItem(new Chainmail(), 1);
        _player.Bag.AddItem(new Potion(), 2);

        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Blue;
        Renderer.TypewriterText(StoryContent.Introduction, 10);

        Console.Write("\n   Enter your name: ");
        string name = Input.ReadLineClean();
        while (string.IsNullOrWhiteSpace(name))
        {
            Console.Write("   Enter your name: ");
            name = Input.ReadLineClean();
        }
        _player.SetName(name);

        Renderer.TypewriterText(StoryContent.AfterIntro(name));
        Thread.Sleep(2000);

        _currentMap = new Map(100, 40, 20);
        EnterMap(_currentMap);
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
        Console.Write(StoryContent.Controls);
        Console.ReadKey(true);
    }

    // ── Map / Exploration Loop ─────────────────────────────

    private void EnterMap(Map map)
    {
        _currentMap = map;
        _player.ResetPosition(map.Width, map.Height);
        Renderer.DrawFullMap(map, _player);
        ExplorationLoop(map);
    }

    private void ExplorationLoop(Map map)
    {
        while (true)
        {
            // Check for combat
            var enemy = map.GetEnemyAt(_player.Position);
            if (enemy != null)
            {
                bool won = CombatSystem.RunEncounter(_player, enemy);
                if (!won && !_player.IsAlive)
                {
                    _player.Stats.Health = 1; // Mercy revival
                }
                _player.ResetPosition(map.Width, map.Height);
                Renderer.DrawFullMap(map, _player);
            }

            // Enemy movement
            foreach (var e in map.Enemies)
            {
                var oldPos = e.TryRandomMove(1, 1, map.Width - 2, map.Height - 2);
                if (oldPos.HasValue)
                {
                    Renderer.MoveEntity(e, oldPos.Value);
                }
            }

            // Player movement
            HandlePlayerMovement(map);

            // Inventory
            if (Input.IsKeyDown(Input.VK_I))
            {
                HandleInventory();
                Renderer.DrawFullMap(map, _player);
            }

            // Pause menu
            if (Input.IsKeyDown(Input.VK_ESCAPE))
            {
                if (HandlePauseMenu(map)) return; // Return to main menu
                Renderer.DrawFullMap(map, _player);
            }

            Thread.Sleep(10); // Prevent busy loop
        }
    }

    private void HandlePlayerMovement(Map map)
    {
        bool moved = false;
        var oldPos = _player.Position;
        var newPos = oldPos;

        if (Input.IsKeyDown(Input.VK_W) && oldPos.Y > 1)
        {
            newPos = new Vec2(oldPos.X, oldPos.Y - 1);
            moved = true;
        }
        else if (Input.IsKeyDown(Input.VK_S) && oldPos.Y < map.Height - 2)
        {
            newPos = new Vec2(oldPos.X, oldPos.Y + 1);
            moved = true;
        }
        else if (Input.IsKeyDown(Input.VK_A) && oldPos.X > 1)
        {
            newPos = new Vec2(oldPos.X - 1, oldPos.Y);
            moved = true;
        }
        else if (Input.IsKeyDown(Input.VK_D) && oldPos.X < map.Width - 2)
        {
            newPos = new Vec2(oldPos.X + 1, oldPos.Y);
            moved = true;
        }

        if (moved)
        {
            _player.Position = newPos;
            Renderer.MoveEntity(_player, oldPos);
            Thread.Sleep(50);
        }
    }

    // ── Inventory UI ───────────────────────────────────────

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

        while (true)
        {
            Renderer.DrawInventory(_player);

            if (Input.IsKeyDown(Input.VK_ESCAPE, 200))
                return;

            if (Input.IsKeyDown(Input.VK_E))
            {
                Console.SetCursorPosition(2, Console.CursorTop + 2);
                Console.Write("Select item #: ");
                int choice = Input.ReadChoice();

                var item = _player.Bag.Items.FirstOrDefault(i => i.InventorySlot == choice);
                if (item == null) continue;

                // Show item stats
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\n   {item.Name}");
                Console.ForegroundColor = ConsoleColor.White;

                var s = item.BonusStats;
                if (s.RequiredLevel > 1) Console.WriteLine($"   Required Level: {s.RequiredLevel}");
                if (s.Health > 0) Console.WriteLine($"   Health: +{s.Health:F0}");
                if (s.Armor > 0) Console.WriteLine($"   Armor: +{s.Armor:F0}");
                if (s.AttackPower > 0) Console.WriteLine($"   Attack Power: +{s.AttackPower}");
                if (s.Strength > 0) Console.WriteLine($"   Strength: +{s.Strength}");

                if (item.IsEquippable && !item.IsEquipped)
                {
                    Console.Write("\n   Equip this item? (y/n): ");
                    if (Input.ReadLineClean().ToLower() == "y")
                    {
                        if (_player.EquipItem(item))
                            Console.WriteLine($"   Equipped {item.Name}!");
                        else
                            Console.WriteLine("   Cannot equip — level too low.");
                        Thread.Sleep(1000);
                    }
                }
                else if (item.IsEquipped)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("\n   Already equipped.");
                    Thread.Sleep(800);
                }
                else if (item.IsConsumable)
                {
                    Console.Write("\n   Use this item? (y/n): ");
                    if (Input.ReadLineClean().ToLower() == "y")
                    {
                        if (_player.UseItem(item))
                            Console.WriteLine($"   Used {item.Name}! (x{item.Quantity} remaining)");
                        else
                            Console.WriteLine("   Cannot use — level too low.");
                        Thread.Sleep(1000);
                    }
                }
            }

            Thread.Sleep(10);
        }
    }

    // ── Pause Menu ─────────────────────────────────────────

    /// <summary>
    /// Returns true if the player wants to return to the main menu.
    /// </summary>
    private bool HandlePauseMenu(Map map)
    {
        Thread.Sleep(200); // Debounce
        Renderer.DrawPauseMenu();

        switch (Input.ReadChoice())
        {
            case 1: // Save
                Console.Write("\n   Save file name: ");
                string fileName = Input.ReadLineClean();
                if (string.IsNullOrWhiteSpace(fileName)) fileName = _player.Name;
                SaveSystem.Save(_player, map, fileName);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   Saved!");
                Thread.Sleep(1500);
                return false;

            case 2: // Quit
                Environment.Exit(0);
                return true;

            case 3: // Main menu
                MainMenu();
                return true;

            case 4: // Continue
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

        if (OperatingSystem.IsWindows())
        {
            try
            {
                Console.SetWindowSize(
                    Console.LargestWindowWidth / 2,
                    Console.LargestWindowHeight - 10);
            }
            catch { /* Not critical if this fails */ }

            // Disable window resizing via Win32 API
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
            const int SC_SIZE = 0xF000;
            const int SC_MINIMIZE = 0xF020;
            const int SC_MAXIMIZE = 0xF030;

            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
        }
        catch { /* Non-critical */ }
    }
}
