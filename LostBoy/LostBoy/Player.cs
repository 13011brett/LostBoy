using System.Runtime.InteropServices;
using System;

public class Player : ICharacter
{


    [DllImport("user32.dll")]
    internal static extern ushort GetAsyncKeyState(int vKey); // Used for getting keys pressed.


    private string name;
    private float health;
    private float damage;
    private Vec3 location;
    private bool bMoving;
    public int level = 1;
    private int experience = 0;
    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public Player(string inName)
    {
        this.health = 100;
        this.damage = 1;
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = inName;
    }
    public Player()
    {
        this.health = 100;
        this.damage = 1;
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = "Name Not Set. Now, how did that happen?\n";
    }
    public void GetName()
    {
        do
        {
            Console.Write("Enter your name: ");
            this.name = Console.ReadLine();
        } while (this.name == "");
    }


    public float RandomFloatNumber(float lowerRange, float upperRange)
    {
        Random random = new Random();
        return (float)random.NextDouble() + random.Next((int)lowerRange, (int)upperRange);


    }
    public int RandomNumber(int lowerRange, int upperRange)
    {
        Random random = new Random();
        return random.Next(lowerRange, upperRange);
    }
    public void Movement(string option) // Movement for now, may do something like getting ASYNC KeyState
    {
        switch (option)
        {
            case "d":
                Console.WriteLine("You have moved to the right.");
                this.location.x++;
                break;
            case "a":
                Console.WriteLine("You have moved to the left.");
                this.location.x--;
                break;
            case "w":
                Console.WriteLine("You have moved forward.");
                this.location.y++;
                break;
            case "s":
                Console.WriteLine("You have moved backward.");
                this.location.y--;
                break;
            default:
                Console.WriteLine("\n\n Not a proper Movement Key. \n\n");
                break;
        }
    }
    public void TimedText(string inText, int inSpeed = 50, bool clearConsole = false) // This allows for text to be timed, need to make a skippable feature into this as well, and make it print out the text as a whole if so.
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



}