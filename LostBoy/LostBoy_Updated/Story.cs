using System.Runtime.InteropServices;
using System;
using System.Xml;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

public class Story // Not sure if making a story object is better than instantiating within a player object. Just didn't want to do this everytime or have this derived in other classes.
{

    [DllImport("user32.dll")]
    public static extern ushort GetAsyncKeyState(int vKey); // Used for getting keys pressed.


    public static string introduction = "#   ___        ________   ________   _________        ________   ________       ___    ___ \r\n#  |\\  \\      |\\   __  \\ |\\   ____\\ |\\___   ___\\     |\\   __  \\ |\\   __  \\     |\\  \\  /  /|\r\n#  \\ \\  \\     \\ \\  \\|\\  \\\\ \\  \\___|_\\|___ \\  \\_|     \\ \\  \\|\\ /_\\ \\  \\|\\  \\    \\ \\  \\/  / /\r\n#   \\ \\  \\     \\ \\  \\\\\\  \\\\ \\_____  \\    \\ \\  \\       \\ \\   __  \\\\ \\  \\\\\\  \\    \\ \\    / / \r\n#    \\ \\  \\____ \\ \\  \\\\\\  \\\\|____|\\  \\    \\ \\  \\       \\ \\  \\|\\  \\\\ \\  \\\\\\  \\    \\/  /  /  \r\n#     \\ \\_______\\\\ \\_______\\ ____\\_\\  \\    \\ \\__\\       \\ \\_______\\\\ \\_______\\ __/  / /    \r\n#      \\|_______| \\|_______||\\_________\\    \\|__|        \\|_______| \\|_______||\\___/ /     \r\n#                           \\|_________|                                      \\|___|/      \r\n#                                                                                          \r\n";
    public static string introduction2 = "Please Select an option! \n\n\n1. Start a New Game.\n2. Load a Saved Game.\n3. View Controls.\n4. Exit the game.";
    public static string introduction3 = "On a moonlit night, unlike any other, you stand outside of a breathtaking castle. You take in the air as you stare at the water that separates the land. " +
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
    protected class FileHolder
    {
        public string fileName;
        public int choice;

        public FileHolder(string fileName, int choice)
        {
            this.fileName = fileName;
            this.choice = choice;
        }
    }
    public static void DoIntro(ref Player p)
    {
        bool FirstTime = true;
        while (true)
        {
            Console.Clear();
            if (FirstTime)
            {
                Story.TimedText(introduction + introduction2 + "\n");
                FirstTime = false;
            }
            else Console.WriteLine(introduction + introduction2 + "\n");

            switch (ChoiceInt())
            {

                case 1: // Create new save file, save is based off of the name currently. 
                    Story.TimedText(introduction3, 10, true);
                    p.GetName();
                    Story.afterIntro(p.Name);
                    return;
                case 2: // Load a saved game.
                    Console.Clear();
                    
                    Console.WriteLine("Please select your save file.\n\n");
                    var files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.xml", SearchOption.AllDirectories);
                    List<FileHolder> filesList = new List<FileHolder>();
                    int i = 0;
                    foreach(var file in files)
                    {

                        filesList.Add(new FileHolder(file, i));
                        Console.WriteLine(i + "\t" + "Name: " + Path.GetFileNameWithoutExtension(file));
                        i++;
                        
                    }
                    int topMaxSelection = Console.CursorTop - i;
                    int bottomMaxSelection = (topMaxSelection + i) - 1;
                    int currentSelection = bottomMaxSelection;
                    Console.SetCursorPosition(Console.CursorLeft, currentSelection);
                    System.Threading.Thread.Sleep(300);
                    while (true)
                    {
                        if(GetKey(0x26, 100) && topMaxSelection < currentSelection && bottomMaxSelection >= currentSelection)
                        {
                            currentSelection--;
                            i--;
                            Console.SetCursorPosition(Console.CursorLeft, currentSelection);
                        }
                        if (GetKey(0x28, 100) && topMaxSelection <= currentSelection && bottomMaxSelection > currentSelection)
                        {
                            currentSelection++;
                            i++;
                            Console.SetCursorPosition(Console.CursorLeft, currentSelection);
                        }
                        if (GetKey(0x0D))
                        {
                            foreach(var file in filesList)
                            {
                                if (file.choice == (i - 1))
                                {
                                    try

                                    { 

                                        p = Player.CreatePlayerFromXmlString(File.ReadAllText(file.fileName));
                                        return;
                                    }
                                    catch(Exception e)
                                    {
                                        Console.Clear();
                                        Console.WriteLine("Invalid Save File!");
                                        System.Threading.Thread.Sleep(1000);
                                        break;

                                    }

                                        
                                }
                                
                            }
                            break;
                        }
                    }
                    break;
                    
                    //Console.ReadKey();
                    //if (File.Exists("playerData.xml"))
                    //{
                    //    p = Player.CreatePlayerFromXmlString(File.ReadAllText("playerData.xml"));   
                    //}
                    return;
                case 3:
                    Console.Clear();
                    Console.WriteLine("W = Move Forward");
                    Console.WriteLine("\nA = Move Left");
                    Console.WriteLine("\nS = Move Backwards");
                    Console.WriteLine("\nD = Move Right");
                    Console.WriteLine("\nI = View Inventory");
                    Console.WriteLine("\nESC = Pause Game");
                    Console.WriteLine("\nPress Any Key to Continue.");
                    Console.ReadKey();
                    break;
                case 4:
                    Environment.Exit(0);
                    return;
                default:
                    Console.WriteLine("Please input a valid choice.");
                    System.Threading.Thread.Sleep(1500);
                    break;
            }
        }

        
    }

    public static int ChoiceInt()
    {
        string choice;
        int x = 0;
        choice = Console.ReadLine();
        if (int.TryParse(choice, out x)) return int.Parse(choice);
        else return 0;
        
    }
    public static void TimedText(string inText, int inSpeed = 10, bool clearConsole = false) // This allows for text to be timed, need to make a skippable feature into this as well, and make it print out the text as a whole if so.
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
        if ((GetAsyncKeyState(Key) & 0x8000) == 0x8000) {
            System.Threading.Thread.Sleep(timer);
            return true;

                }
        return false;
    }

}