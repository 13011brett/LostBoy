using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using LostBoy.Items;
using LostBoy;

public class Player
{
    public struct Vec3 { public float x; public float y; public float z; }; // Z May be used just to dictate the level we're on? Not quite sure. Going to be a 2d game currently.


    private string name;
    public Stats stats { get; set; }
    public Inventory playerInventory = new Inventory();
    public float Health { get; protected set; }
    public float Armor { get; protected set; }
    public int ExperienceRequired { get; protected set; }
    public float damage { get; protected set; }
    protected Vec3 location;
    private bool bMoving;
    public int level = 1;
    public int Experience { get; protected set; }
    protected char icon = 'p';
    protected ConsoleColor color;
    //public float Health
    //{
    //    get { return health; }
    //    set { health = value; }
    //}
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
    /*    public Player(string inName)
        {
            this.Health = 100;
            this.damage = level*(RandomNumber(level, level+3));
            this.Experience = 0;
            this.location = new Vec3() { x = 0, y = 0, z = 0 };
            this.name = inName;
            this.color = ConsoleColor.Green;
            this.level = 2;
        }*/
    public Player()
    {
        this.Health = 10000;
        this.damage = level * (RandomNumber(level, level + 50));
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = "Name Not Set. Now, how did that happen?\n";
        this.color = ConsoleColor.Blue;
        this.ExperienceRequired = ((this.level * this.level) * 100);
        this.stats = new StatsBuilder()
            .SetAPBase(100)
            .SetHealth(100)
            .Build();


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
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return (float)random.NextDouble() + random.Next((int)lowerRange, (int)upperRange);


    }
    static public int RandomNumber(int lowerRange, int upperRange)
    {
        
        Random random = new Random(Guid.NewGuid().GetHashCode());
        return random.Next(lowerRange, upperRange);
    }

    public static void DoMovement(Player p, Map map, int amount = 0)
    {
        Console.SetWindowPosition(0, 0);
        if (Story.GetKey(0x57) || Story.GetKey(0x53) || Story.GetKey(0x41) || Story.GetKey(0x44))
        {
            
            Console.SetCursorPosition((int)(p.location.x), ((int)p.location.y));
            Console.Write(" ");
            if ((int)p.location.y != 1 && (Story.GetAsyncKeyState(0x57) & 0x8000) == 0x8000)
            {
                p.location.y -= amount; // W key
                Enemy.Movement(map);
            }
        
            if((int)p.location.y != (Console.WindowHeight-1) && (Story.GetAsyncKeyState(0x53) & 0x8000) == 0x8000)
            {

                p.location.y += amount; // S key
                Enemy.Movement(map);
                
            }
            if ((int)p.location.x != 1 && (Story.GetAsyncKeyState(0x41) & 0x8000) == 0x8000)
            {
                 p.location.x -= amount; // A Key
                 Enemy.Movement(map);
                
            }
            if((int)p.location.x != (Console.WindowWidth - 2) && (Story.GetAsyncKeyState(0x44) & 0x8000) == 0x8000)
            {   
                p.location.x += amount; // D key
                Enemy.Movement(map);
                
            } 
            Console.SetCursorPosition(((int)p.location.x), ((int)p.location.y));
            Console.Write(p.icon);

            System.Threading.Thread.Sleep(50);
        }
    }
    public void ResetLocation(ref Map map)
    {
        this.location.x = (int)((map.MapSize.x / 2));
        this.location.y = (int)(map.MapSize.y-1);
        this.location.z = map.MapSize.z;
        Console.CursorVisible = false;
        Console.SetCursorPosition(((int)this.location.x / 2), ((int)this.location.y));
    }

    public static void Damage(ref Player attacker, Player attackee)
    {
        attacker.Health -= attackee.damage;
        attackee.Health -= attacker.damage;
    }

    public static void GainExperience(Player player, Player enemy)
    {
        if(enemy.Health <= 0)
        {
            player.Experience += enemy.Experience;
            GetLevel(player);
        }
    }



    public static void GetLevel(Player player)
    {

        while(player.Experience >= player.ExperienceRequired)
        {
            player.Experience -= player.ExperienceRequired;
            player.level++;
            player.ExperienceRequired = ((player.level * player.level) * 100);
        }

    }





}