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
    public float LocationY
    {
        get
        {
            return location.y;
        }
        set
        {
            location.y = value;

        }
    }
    public float LocationX
    {
        get
        {
            return location.x;
        }
        set
        {
            location.x = value;

        }
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
    static public int RandomNumber(int lowerRange, int upperRange)
    {
        
        Random random = new Random(Guid.NewGuid().GetHashCode());
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




}