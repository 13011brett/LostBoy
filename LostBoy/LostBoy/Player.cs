using System.Runtime.InteropServices;
using System;


public class Player : ICharacter
{
    public struct Vec3 { public float x; public float y; public float z; }; // Z May be used just to dictate the level we're on? Not quite sure. Going to be a 2d game currently.


    private string name;
    private float health;
    private float damage;
    protected Vec3 location;
    private bool bMoving;
    public int level = 1;
    private int experience = 0;
    protected char icon;
    protected ConsoleColor color;
    public float Health
    {
        get { return health; }
        set { health = value; }
    }
    public string Name
    {
        get { return name; }
    }
    public Vec3 Location
    {
        get
        {
            return location;
        }
        set
        {
            location = value;

        }
    }
    public Player(string inName)
    {
        this.health = 100;
        this.damage = 1;
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = inName;
        this.color = ConsoleColor.Green;
    }
    public Player()
    {
        this.health = 100;
        this.damage = 1;
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = "Name Not Set. Now, how did that happen?\n";
        this.color = ConsoleColor.Green;
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
    static public int RandomNumber(int lowerRange, int upperRange)
    {
        
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return random.Next(lowerRange, upperRange);
    }

    public static void DoMovement(Player p, Map map, int key, int amount = 0)
    {
        Console.SetWindowPosition(0, 0);
        if ((Story.GetAsyncKeyState(key) & 0x8000) == 0x8000) // W key
        {
            
            Console.SetCursorPosition((int)(p.location.x), ((int)p.location.y));
            Console.Write(" ");
            if ((int)p.location.y != 1)
            {
                if (key == 0x57)
                {
                    p.location.y -= amount; // W key
                    Enemy.Movement(map);
                }
            }
        
            if((int)p.location.y != (Console.WindowHeight-1))
            {
                if (key == 0x53)
                {
                    p.location.y += amount; // S key
                    Enemy.Movement(map);
                }
            }
            if ((int)p.location.x != 1)
            {
                if (key == 0x41)
                {
                    p.location.x -= amount; // A Key
                    Enemy.Movement(map);
                }
            }
            if((int)p.location.x != (Console.WindowWidth - 2 ))
            {
                if (key == 0x44)
                {
                    p.location.x += amount; // D key
                    Enemy.Movement(map);
                }
            } 
            Console.SetCursorPosition(((int)p.location.x), ((int)p.location.y));
            Console.Write('x');

            System.Threading.Thread.Sleep(50);
        }
    }

    public void Movement(string option) // Not really used, at all. Old, initial idea of movement that I never implemented.
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



}