using LostBoy.Core;
using LostBoy.Entities;
using LostBoy.Items;
using LostBoy.Maps;
using LostBoy.Systems;
using Microsoft.JSInterop;

namespace LostBoy.Web.Services;

public enum GameScreen
{
    MainMenu,
    Intro,
    IntroName,
    IntroTransition,
    Tutorial,
    Exploring,
    Combat,
    Inventory,
    Paused,
    MapCleared,
    Victory,
    Defeat
}

public class GameEngine : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _game;

    public Player Player { get; private set; } = new();
    public Map? CurrentMap { get; private set; }
    public Enemy? CurrentEnemy { get; private set; }
    public GameScreen Screen { get; set; } = GameScreen.MainMenu;
    public string CombatLog { get; set; } = "";
    public string CombatLogClass { get; set; } = "";
    public List<string> LootLog { get; } = new();
    public bool IsFirstGame { get; set; } = true;

    // Callback to notify Blazor component to re-render
    public event Action? OnStateChanged;

    public GameEngine(IJSRuntime js)
    {
        _js = js;
    }

    public async Task InitCanvas(string canvasId)
    {
        _game = await _js.InvokeAsync<IJSObjectReference>("eval", "window.LostBoyGame");
        await _js.InvokeVoidAsync("LostBoyGame.init", canvasId, null);
    }

    public async Task SetCanvasSize(int cols, int rows)
    {
        await _js.InvokeVoidAsync("LostBoyGame.setSize", cols, rows);
    }

    // ── Input ──────────────────────────────────────────────

    public async Task<bool> IsKeyDown(string key)
    {
        return await _js.InvokeAsync<bool>("LostBoyGame.isKeyDown", key);
    }

    public async Task<bool> ConsumeKey(string key)
    {
        return await _js.InvokeAsync<bool>("LostBoyGame.consumeKey", key);
    }

    // ── Rendering ──────────────────────────────────────────

    public async Task Clear()
    {
        await _js.InvokeVoidAsync("LostBoyGame.clear");
    }

    public async Task DrawCell(int x, int y, char ch, string color)
    {
        await _js.InvokeVoidAsync("LostBoyGame.drawCell", x, y, ch.ToString(), color);
    }

    public async Task DrawString(int x, int y, string text, string color)
    {
        await _js.InvokeVoidAsync("LostBoyGame.drawString", x, y, text, color);
    }

    public async Task DrawMap()
    {
        if (CurrentMap == null) return;
        var map = CurrentMap;

        await SetCanvasSize(map.Width, map.Height + 1);
        await Clear();

        // Batch all tiles for performance
        var xs = new List<int>();
        var ys = new List<int>();
        var chars = new List<string>();
        var colors = new List<string>();

        // Border - top
        xs.Add(0); ys.Add(0); chars.Add("┌"); colors.Add("#555555");
        for (int x = 1; x < map.Width - 1; x++)
        {
            xs.Add(x); ys.Add(0); chars.Add("─"); colors.Add("#555555");
        }
        xs.Add(map.Width - 1); ys.Add(0); chars.Add("┐"); colors.Add("#555555");

        // Border - sides
        for (int y = 1; y < map.Height - 1; y++)
        {
            xs.Add(0); ys.Add(y); chars.Add("│"); colors.Add("#555555");
            xs.Add(map.Width - 1); ys.Add(y); chars.Add("│"); colors.Add("#555555");
        }

        // Border - bottom
        xs.Add(0); ys.Add(map.Height - 1); chars.Add("└"); colors.Add("#555555");
        for (int x = 1; x < map.Width - 1; x++)
        {
            xs.Add(x); ys.Add(map.Height - 1); chars.Add("─"); colors.Add("#555555");
        }
        xs.Add(map.Width - 1); ys.Add(map.Height - 1); chars.Add("┘"); colors.Add("#555555");

        // Terrain
        for (int y = 1; y < map.Height - 1; y++)
        {
            for (int x = 1; x < map.Width - 1; x++)
            {
                var tile = map.Terrain[x, y];
                if (tile.Glyph != ' ')
                {
                    xs.Add(x); ys.Add(y);
                    chars.Add(tile.Glyph.ToString());
                    colors.Add(tile.Color);
                }
            }
        }

        // Enemies
        foreach (var enemy in map.Enemies)
        {
            if (!enemy.IsAlive) continue;
            var pos = enemy.Position;
            if (pos.X <= 0 || pos.X >= map.Width - 1 || pos.Y <= 0 || pos.Y >= map.Height - 1) continue;
            xs.Add(pos.X); ys.Add(pos.Y);
            chars.Add(enemy.Icon.ToString());
            colors.Add(enemy.Color);
        }

        // Player
        xs.Add(Player.Position.X); ys.Add(Player.Position.Y);
        chars.Add(Player.Icon.ToString());
        colors.Add(Player.Color);

        // Map name in top border
        string title = $"─ {map.Name} ";
        for (int i = 0; i < title.Length && i + 2 < map.Width - 2; i++)
        {
            xs.Add(i + 2); ys.Add(0);
            chars.Add(title[i].ToString());
            colors.Add("#888888");
        }

        await _js.InvokeVoidAsync("LostBoyGame.drawBatch",
            xs.ToArray(), ys.ToArray(), chars.ToArray(), colors.ToArray());

        // HUD row
        await DrawHud();
    }

    private async Task DrawHud()
    {
        if (CurrentMap == null) return;
        int y = CurrentMap.Height;
        float pct = Player.Stats.MaxHealth > 0 ? Player.Stats.Health / Player.Stats.MaxHealth : 0;
        int filled = (int)(pct * 10);
        string bar = new string('█', filled) + new string('░', 10 - filled);
        string hud = $" HP:{bar} {Player.Stats.Health:F0}/{Player.Stats.MaxHealth:F0}  Lv:{Player.Level}  EXP:{Player.Experience}/{Player.ExperienceRequired}";

        // Pad to width
        if (hud.Length < CurrentMap.Width)
            hud += new string(' ', CurrentMap.Width - hud.Length);

        await DrawString(0, y, hud, "#888888");
    }

    public async Task RedrawEntity(Vec2 oldPos, Entity entity)
    {
        if (CurrentMap == null) return;

        // Restore terrain at old position
        if (oldPos.X > 0 && oldPos.X < CurrentMap.Width - 1 &&
            oldPos.Y > 0 && oldPos.Y < CurrentMap.Height - 1)
        {
            var tile = CurrentMap.Terrain[oldPos.X, oldPos.Y];
            if (tile.Glyph != ' ')
                await DrawCell(oldPos.X, oldPos.Y, tile.Glyph, tile.Color);
            else
                await DrawCell(oldPos.X, oldPos.Y, ' ', "#0a0a0f");
        }

        // Draw entity at new position
        await DrawCell(entity.Position.X, entity.Position.Y, entity.Icon, entity.Color);
    }

    // ── Game Logic ─────────────────────────────────────────

    public void NewGame()
    {
        Player = new Player();
        Player.Bag.AddItem(new RustySword(), 1);
        Player.Bag.AddItem(new Potion(), 3);
        Screen = GameScreen.Intro;
        NotifyStateChanged();
    }

    public void SetPlayerName(string name)
    {
        Player.SetName(name);
        Screen = GameScreen.IntroTransition;
        NotifyStateChanged();
    }

    public void StartTutorial()
    {
        var tutorial = Map.CreateTutorialMap();
        EnterMap(tutorial);
        Screen = GameScreen.Tutorial;
        NotifyStateChanged();
    }

    public void StartExploring()
    {
        Screen = GameScreen.Exploring;
        NotifyStateChanged();
    }

    public void EnterMap(Map map)
    {
        CurrentMap = map;
        Player.ResetPosition(map.Width, map.Height);
    }

    public void AdvanceMap()
    {
        if (CurrentMap == null) return;

        Map next;
        if (CurrentMap.Difficulty <= 5)
            next = Map.CreateDungeon();
        else if (CurrentMap.Difficulty <= 25)
            next = Map.CreateCastle();
        else
            next = new Map(110, 40, CurrentMap.Difficulty + 15, "The Abyss");

        EnterMap(next);
        Screen = GameScreen.Exploring;
        NotifyStateChanged();
    }

    // Movement cooldown — controls how fast the player moves when holding a key
    private DateTime _lastMoveTime = DateTime.MinValue;
    private const int MoveIntervalMs = 100; // ~10 tiles/sec when holding

    /// <summary>
    /// Main tick — called ~30fps from the Blazor component timer.
    /// Returns true if the canvas needs a full redraw.
    /// </summary>
    public async Task<bool> Tick()
    {
        if (Screen != GameScreen.Exploring || CurrentMap == null) return false;

        bool needsFullRedraw = false;

        // Check combat trigger
        var enemy = CurrentMap.GetEnemyAt(Player.Position);
        if (enemy != null)
        {
            CurrentEnemy = enemy;
            CombatLog = "";
            LootLog.Clear();
            Screen = GameScreen.Combat;
            NotifyStateChanged();
            return false;
        }

        // Check all enemies defeated
        if (CurrentMap.Enemies.All(e => !e.IsAlive))
        {
            Screen = GameScreen.MapCleared;
            NotifyStateChanged();
            return false;
        }

        // Enemy movement
        foreach (var e in CurrentMap.Enemies)
        {
            if (!e.IsAlive) continue;
            var oldPos = e.TryRandomMove(CurrentMap);
            if (oldPos.HasValue)
            {
                await RedrawEntity(oldPos.Value, e);
            }
        }

        // Player movement — uses IsKeyDown (held) with a cooldown
        bool canMove = (DateTime.UtcNow - _lastMoveTime).TotalMilliseconds >= MoveIntervalMs;

        if (canMove)
        {
            var playerOld = Player.Position;
            Vec2 newPos = playerOld;
            bool moved = false;

            if (await IsKeyDown("w") || await IsKeyDown("arrowup"))
            { newPos = new Vec2(playerOld.X, playerOld.Y - 1); moved = true; }
            else if (await IsKeyDown("s") || await IsKeyDown("arrowdown"))
            { newPos = new Vec2(playerOld.X, playerOld.Y + 1); moved = true; }
            else if (await IsKeyDown("a") || await IsKeyDown("arrowleft"))
            { newPos = new Vec2(playerOld.X - 1, playerOld.Y); moved = true; }
            else if (await IsKeyDown("d") || await IsKeyDown("arrowright"))
            { newPos = new Vec2(playerOld.X + 1, playerOld.Y); moved = true; }

            if (moved && CurrentMap.IsWalkable(newPos))
            {
                Player.Position = newPos;
                await RedrawEntity(playerOld, Player);
                await DrawHud();
                _lastMoveTime = DateTime.UtcNow;
            }
        }

        // Inventory (one-shot)
        if (await ConsumeKey("i"))
        {
            Screen = GameScreen.Inventory;
            NotifyStateChanged();
        }

        // Pause (one-shot)
        if (await ConsumeKey("escape"))
        {
            Screen = GameScreen.Paused;
            NotifyStateChanged();
        }

        return needsFullRedraw;
    }

    // ── Combat Actions ─────────────────────────────────────

    public void CombatAttack()
    {
        if (CurrentEnemy == null || !CurrentEnemy.IsAlive || !Player.IsAlive) return;

        var (pDmg, eDmg) = CombatSystem.ExchangeBlows(Player, CurrentEnemy);
        CombatLog = $"You dealt {eDmg:F0} damage, took {pDmg:F0} damage.";
        CombatLogClass = "";

        if (!CurrentEnemy.IsAlive)
        {
            var loot = CombatSystem.ResolveCombatVictory(Player, CurrentEnemy);
            CombatLog = $"{CurrentEnemy.Name} vanquished! +{CurrentEnemy.Experience} EXP";
            CombatLogClass = "victory";
            LootLog.Clear();
            foreach (var item in loot)
                LootLog.Add($"Looted: {item.Name}");

            Screen = GameScreen.Victory;
        }
        else if (!Player.IsAlive)
        {
            CombatLog = "You have been defeated...";
            CombatLogClass = "defeat";
            Screen = GameScreen.Defeat;
        }

        NotifyStateChanged();
    }

    public void CombatRun()
    {
        Screen = GameScreen.Exploring;
        CurrentEnemy = null;
        if (CurrentMap != null)
            Player.ResetPosition(CurrentMap.Width, CurrentMap.Height);
        NotifyStateChanged();
    }

    public async Task ReturnFromCombat()
    {
        CurrentEnemy = null;
        if (!Player.IsAlive)
        {
            Player.Stats.Health = 1; // Mercy revival
        }
        if (CurrentMap != null)
            Player.ResetPosition(CurrentMap.Width, CurrentMap.Height);
        Screen = GameScreen.Exploring;
        NotifyStateChanged();
        await DrawMap();
    }

    // ── Inventory Actions ──────────────────────────────────

    public void EquipItem(Item item)
    {
        if (Player.EquipItem(item))
            NotifyStateChanged();
    }

    public void UseItem(Item item)
    {
        if (Player.UseItem(item))
            NotifyStateChanged();
    }

    public async Task CloseOverlay()
    {
        Screen = GameScreen.Exploring;
        NotifyStateChanged();
        await DrawMap();
    }

    public void ReturnToMenu()
    {
        Screen = GameScreen.MainMenu;
        IsFirstGame = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnStateChanged?.Invoke();

    public async ValueTask DisposeAsync()
    {
        if (_game != null)
            await _game.DisposeAsync();
    }
}