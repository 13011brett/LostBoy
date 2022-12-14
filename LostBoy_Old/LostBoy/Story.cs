using System.Runtime.InteropServices;
using System;

public class Story // Not sure if making a story object is better than instantiating within a player object. Just didn't want to do this everytime or have this derived in other classes.
{

    [DllImport("user32.dll")]
    public static extern ushort GetAsyncKeyState(int vKey); // Used for getting keys pressed.



    public static string introduction = "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. You take in the air as you stare at the water that separates the land. " +
        "\"What a beautiful sight! I wonder what this castle was like before it was abandoned..\" You think to yourself. As soon as that thought finishes playing in your mind, a sudden chill " +
        "goes throughout your body, completely freezing you to a a halt. People pass by you as you stand there in shock, not able to move a muscle, other than your eyes. Your attention is quickly " +
        "diverted to the sight of a malformed castle, being tarnished by a fantastical beast. You can't believe your eyes; you faint in almost an instant... \n\n\n\n " +
        "You wake up in a dimly lit room; the air reeks of damp laundry. Something is eerily familiar about this place, but you can't piece it together in the shocked state you are in.\n" +
        "You attempt to push yourself off of the ground, and quickly realize your arms are chained to the floor behind you. You begin to panic further, trying to pull yourself off of the rusty chains. " +
        "As you begin to scream \"LET ME OUT!\" A voice comes from the distance,\n\nUnknown Man: \"Hush! You'll wake the beasts..\"\n\n" +
        "You freeze dead in your tracks, chains in hand, paralyzed as you were when you saw the fortress being destroyed. Your eyes slowly shift to the center of the room, where you can see the man" +
        " sitting at a dusty table, with some strange contraptions upon it. Your body, without your control, starts moving towards the table, but are quickly stopped by the chains.. Or so you thought. " +
        "The man had done something with the chains, now they are dragging on the floor behind you as you begin to sit at the only other chair at the table, against your own will.\n\n" +
        "Strangely enough, the man reaches out to hold one of your hands, and proceeds to speak. \nUnknown Man: \"I've been alone for so long... Please, tell me, what is your name?\"\n";

    public static void afterIntro(string name)
    {
        Console.Write("\nAh I see, what a lovely name.. Now how did you end up here " + name + "?"); 
    } 


    public static void TimedText(string inText, int inSpeed = 50, bool clearConsole = false) // This allows for text to be timed, need to make a skippable feature into this as well, and make it print out the text as a whole if so.
    {

        for (int i = 1; i <= inText.Length; i++)
        {
            if (clearConsole) // Clear console if chosen, otherwise false.
            {
                Console.Clear();
                clearConsole = false;
            }

            Console.Write(inText[i - 1]); // Main loop of execution for text, takes it one char at a time. Like a char array. -1 to account for array starting at 0.
            System.Threading.Thread.Sleep(inSpeed);

            if ((GetAsyncKeyState(27) & 0x8000) == 0x8000) // If key pressed, clear the text, and output the text as a whole. 27 = Escape key. 
            {
                Console.WriteLine(inText.Substring(i, (inText.Length - i)));
                break;
            };

        }
    }

    public static bool GetKey(int Key, int timer = 0)
    {
        if ((GetAsyncKeyState(Key) & 0x8000) == 0x8000) return true;
        return false;
    }

}