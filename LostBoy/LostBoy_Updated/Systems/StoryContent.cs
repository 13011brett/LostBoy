namespace LostBoy.Systems;

/// <summary>
/// Contains all story/narrative text.
/// Separated from game logic so content can be edited independently.
/// </summary>
public static class StoryContent
{
    public static readonly string Title = @"
   ┌─────────────────────────────────────────────────────────────────┐
   │   ___        ________   ________   _________                   │
   │  |\  \      |\   __  \ |\   ____\ |\___   ___\                │
   │  \ \  \     \ \  \|\  \\ \  \___|_\|___ \  \_|                │
   │   \ \  \     \ \  \\\  \\ \_____  \    \ \  \                 │
   │    \ \  \____ \ \  \\\  \\|____|\  \    \ \  \                │
   │     \ \_______\\ \_______\ ____\_\  \    \ \__\               │
   │      \|_______| \|_______||\_________\    \|__|               │
   │                            \|_________|                        │
   │                                                                │
   │         ________   ________       ___    ___                   │
   │        |\   __  \ |\   __  \     |\  \  /  /|                 │
   │        \ \  \|\ /_\ \  \|\  \    \ \  \/  / /                │
   │         \ \   __  \\ \  \\\  \    \ \    / /                  │
   │          \ \  \|\  \\ \  \\\  \    \/  /  /                   │
   │           \ \_______\\ \_______\ __/  / /                     │
   │            \|_______| \|_______||\___/ /                      │
   │                                  \|___|/                      │
   └─────────────────────────────────────────────────────────────────┘
";

    public static readonly string Menu = @"
   ┌──────────────────────────────┐
   │   1.  New Game               │
   │   2.  Load Game              │
   │   3.  Controls               │
   │   4.  Exit                   │
   └──────────────────────────────┘
";

    public static readonly string Controls = @"
   ┌──────────────────────────────┐
   │        CONTROLS              │
   ├──────────────────────────────┤
   │   W     Move Forward         │
   │   A     Move Left            │
   │   S     Move Backward        │
   │   D     Move Right           │
   │   I     Inventory            │
   │   ESC   Pause Menu           │
   │                              │
   │   In Combat:                 │
   │   K     Attack               │
   │   R     Run Away             │
   └──────────────────────────────┘

   Press any key to continue...
";

    public static readonly string Introduction =
        "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. " +
        "You take in the air as you stare at the water that separates the land. " +
        "\"What a beautiful sight! I wonder what this castle was like before it was abandoned..\" " +
        "You think to yourself.\n\n" +
        "As soon as that thought finishes playing in your mind, a sudden chill goes throughout " +
        "your body, completely freezing you to a halt. People pass by you as you stand there " +
        "in shock, not able to move a muscle, other than your eyes.\n\n" +
        "Your attention is quickly diverted to the sight of a malformed castle, being tarnished " +
        "by a fantastical beast. You can't believe your eyes; you faint in almost an instant...\n\n" +
        "You wake up in a dimly lit room; the air reeks of damp laundry. Something is eerily " +
        "familiar about this place, but you can't piece it together in the shocked state you are in.\n\n" +
        "You attempt to push yourself off of the ground, and quickly realize your arms are chained " +
        "to the floor behind you. You begin to panic further, trying to pull yourself off of the " +
        "rusty chains.\n\n" +
        "As you begin to scream \"LET ME OUT!\" A voice comes from the distance,\n\n" +
        "Unknown Man: \"Hush! You'll wake the beasts..\"\n\n" +
        "You freeze dead in your tracks, chains in hand, paralyzed as you were when you saw the " +
        "fortress being destroyed. Your eyes slowly shift to the center of the room, where you " +
        "can see the man sitting at a dusty table, with some strange contraptions upon it.\n\n" +
        "Your body, without your control, starts moving towards the table, but are quickly stopped " +
        "by the chains.. Or so you thought. The man had done something with the chains, now they " +
        "are dragging on the floor behind you as you begin to sit at the only other chair at the " +
        "table, against your own will.\n\n" +
        "Strangely enough, the man reaches out to hold one of your hands, and proceeds to speak.\n\n" +
        "Unknown Man: \"I've been alone for so long... Please, tell me, what is your name?\"\n";

    public static string AfterIntro(string name)
        => $"\nAh I see, what a lovely name.. Now how did you end up here, {name}?\n";
}
