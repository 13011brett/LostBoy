# Lost Boy

A retro dungeon-crawling RPG built in C# / .NET 8. Explore procedurally generated maps, fight monsters, loot gear, and level up — playable in the browser or from your terminal.

![gameplay](gameplay.png)

## Gameplay

You wake up chained in a dark cellar with no memory of how you got there. A mysterious stranger sets you free and sends you into the depths below. Fight your way through Skeletons, Wraiths, and Demons across increasingly dangerous maps.

### Controls

| Key | Action |
|-----|--------|
| `W` `A` `S` `D` | Move |
| `I` | Open inventory |
| `K` | Attack (in combat) |
| `R` | Run away (in combat) |
| `ESC` | Pause / Save |

Walk into an enemy to start a fight. Defeat all enemies on a map to advance to the next area.

## Features

- **Two platforms** — play in the browser (Blazor WebAssembly) or the console
- **Real-time combat** with health bars, armor reduction, and loot drops
- **Inventory system** — equip armor and weapons across multiple slots, use consumable potions
- **Leveling** — gain EXP from kills, level up to increase HP, damage, and unlock better gear
- **Procedural maps** — terrain with pillars, water pools, rubble, torches, and wall segments
- **Multiple enemy types** — Skeletons, Zombies, Spiders, Goblins, Wraiths, Ogres, Dark Knights, and Demons
- **Varied loot table** — swords, axes, helms, gauntlets, boots, chainmail, amulets, and potions
- **Save / Load** — XML-based save files in the console version
- **Map progression** — The Cellar → The Dungeon → The Castle → The Abyss

## Project Structure

```
LostBoyWeb/
├── LostBoy.Core/         Shared game logic (entities, items, maps, combat, stats)
├── LostBoy.Console/      Console frontend (Windows terminal rendering, P/Invoke input)
├── LostBoy.Web/           Blazor WebAssembly frontend (canvas rendering, browser input)
└── LostBoy.slnx
```

`LostBoy.Core` is a class library containing all platform-independent game logic. Both the Console and Web projects reference it and provide their own rendering and input layers.

## Building & Running

Requires [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

### Web (Blazor WebAssembly)

```bash
dotnet run --project LostBoy.Web
```

Open the URL shown in the terminal (usually `https://localhost:5001`). Works in any modern browser.

### Console

```bash
dotnet run --project LostBoy.Console
```

Best experienced in Windows Terminal at a reasonable window size. Requires Windows for real-time keyboard input (uses `GetAsyncKeyState`).

### Build All

```bash
dotnet build
```

## Tech Stack

- **C# / .NET 8**
- **Blazor WebAssembly** — browser frontend
- **HTML5 Canvas** via JS interop — map rendering in the web version
- **System.Console** — terminal rendering in the console version
- **XML** — save file serialization

## License

MIT
