namespace LostBoy.Systems;

public static class StoryContent
{
    public static readonly string Introduction =
        "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. " +
        "You take in the air as you stare at the water that separates the land. " +
        "\"What a beautiful sight! I wonder what this castle was like before it was abandoned..\" " +
        "You think to yourself.\n\n" +
        "As soon as that thought finishes playing in your mind, a sudden chill goes throughout " +
        "your body, completely freezing you to a halt.\n\n" +
        "Your attention is quickly diverted to the sight of a malformed castle, being tarnished " +
        "by a fantastical beast. You can't believe your eyes; you faint in almost an instant...\n\n" +
        "You wake up in a dimly lit room; the air reeks of damp laundry. Something is eerily " +
        "familiar about this place, but you can't piece it together in the shocked state you are in.\n\n" +
        "You attempt to push yourself off of the ground, and quickly realize your arms are chained " +
        "to the floor behind you. You begin to panic further, trying to pull yourself off of the " +
        "rusty chains.\n\n" +
        "As you begin to scream \"LET ME OUT!\" A voice comes from the distance,\n\n" +
        "Unknown Man: \"Hush! You'll wake the beasts..\"\n\n" +
        "The man reaches out and speaks.\n\n" +
        "Unknown Man: \"I've been alone for so long... Please, tell me, what is your name?\"\n";

    public static readonly string TutorialTransition =
        "The man nods slowly and gestures toward a rusted door at the far end of the room.\n\n" +
        "Unknown Man: \"Beyond that door lies the cellar. It's where they keep the weaker ones.. " +
        "the ones that couldn't survive above.\"\n\n" +
        "He slides a worn chainmail vest and a small vial across the table.\n\n" +
        "Unknown Man: \"Take these. You'll need them. And remember — those beasts won't wait " +
        "for you to be ready. Walk toward them, and steel yourself.\"\n\n" +
        "The door creaks open on its own. The darkness beyond beckons...";

    public static string AfterIntro(string name)
        => $"Unknown Man: \"Ah.. {name}. What a name to carry into a place like this.\"";
}
