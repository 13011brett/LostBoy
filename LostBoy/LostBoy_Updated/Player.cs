using System.Runtime.InteropServices;
using System;
using System.Runtime.CompilerServices;
using LostBoy.Items;
using LostBoy;
using System.Reflection.Emit;

public class Player
{
    public struct Vec3 { public float x; public float y; public float z; }; // Z May be used just to dictate the level we're on? Not quite sure. Going to be a 2d game currently.


    private string name;
    public Stats stats { get; set; }
    public Inventory playerInventory = new Inventory();
    public float Armor { get; protected set; }
    public int ExperienceRequired { get; protected set; }
    public float damage { get; protected set; }
    protected Vec3 location;
    private bool bMoving;
    public int level = 1;
    public int Experience { get; protected set; }
    public char icon { get; protected set; } = 'p';
    protected ConsoleColor color;
    public Map CurrentMap { get; set; }
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
        
        this.damage = level * (RandomNumber(level, level + 50));
        this.location = new Vec3() { x = 0, y = 0, z = 0 };
        this.name = "Name Not Set. Now, how did that happen?\n";
        this.color = ConsoleColor.Blue;
        this.ExperienceRequired = ((this.level * this.level) * 100);
        this.stats = new StatsBuilder()
            .SetAPBase(100)
            .SetHealth(100)
            .SetArmor(9123)
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
        attacker.stats.Health -= attackee.damage;
        attackee.stats.Health -= attacker.damage;
    }

    public static void GainExperience(Player player, Player enemy)
    {
        if(enemy.stats.Health <= 0)
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


    public void EquipItem(ObtainableItem item)
    {
        foreach(var piece in playerInventory.InventoryItems)
        {
            if(item.itemslot == piece.itemslot && piece.bIsEquipped)
            {
                if (item.bIsEquipped && item.ID == piece.ID) // If somehow you equip the same item twice
                {
                    Console.WriteLine(item.Name + " Is already Equipped.");
                    break;
                }
                //UnEquipItem()
                this.stats.Health -= piece.stats.Health;
                this.stats.Strength -= piece.stats.Strength;
                this.stats.Dexterity -= piece.stats.Dexterity;
                this.stats.Vitality -= piece.stats.Vitality;
                this.stats.Intelligence -= piece.stats.Intelligence;
                this.stats.Armor -= piece.stats.Armor;
                this.stats.AttackPower -= piece.stats.AttackPower;
                piece.bIsEquipped = false;

            }
        }



        if (item.bIsEquippable && !item.bIsEquipped) //Kinda works, probably should be improved on. 
        {
            this.stats.Health += item.stats.Health;
            this.stats.Strength += item.stats.Strength;
            this.stats.Dexterity += item.stats.Dexterity;
            this.stats.Vitality += item.stats.Vitality;
            this.stats.Intelligence += item.stats.Intelligence;
            this.stats.Armor += item.stats.Armor;
            this.stats.AttackPower += item.stats.AttackPower; // May be a better way to add in the stats, not sure.

            item.bIsEquipped = true;
        }

    }
    public void ViewInventory()
    {
        Console.Clear();


        Console.WriteLine("Current Stats: ");
        this.stats.OutputStats();
        Console.WriteLine("Experience: " + this.Experience + "/" + ExperienceRequired + "\n" + "Current Level: " + this.level + "\n\n");
        string choice;
        bool bIsDone = false;
        if(this.playerInventory.InventoryItems.Count == 0)
        {
            Console.WriteLine("You have no items currently!");
            System.Threading.Thread.Sleep(1500);
            Map.DrawMap(this.CurrentMap, this);
            return;
        }

            InventoryStart:
            int i = 1;
            Console.Clear();
            string equipItems = "Equip / View items = E Key.";
            string quitInv = "Leave Inventory = ESC key.";
            Console.SetCursorPosition((Console.WindowWidth - equipItems.Length), 0);
            Console.Write(equipItems);
            Console.SetCursorPosition((Console.WindowWidth - quitInv.Length), 1);
            Console.Write(quitInv);
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Current Stats: ");
            this.stats.OutputStats();
            Console.WriteLine("Experience: " + this.Experience + "/" + ExperienceRequired + "\n" + "Current Level: " + this.level + "\n\n");

            foreach (var piece in this.playerInventory.InventoryItems)
            {
                string isEquipped = " ";
                if (piece.bIsEquipped) isEquipped = "\t (Equipped)";
                Console.WriteLine(piece.Name + "\t " + i + isEquipped);
                piece.InventorySlot = i;
                i++;


            }

        while (!bIsDone)
        {
            if (Story.GetKey(0x45))
            {
                while (true)
                {

                    Console.WriteLine("Select an item you wish to view via the corresponding #. ");
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    choice = Console.ReadLine();
                    Console.SetCursorPosition(Console.CursorLeft, (Console.CursorTop - 1));

                    foreach (var piece in this.playerInventory.InventoryItems)
                    {
                        int x = 0;
                        if (Int32.TryParse(choice, out x) && Int32.Parse(choice) == piece.InventorySlot)
                        {
                            piece.stats.OutputStats();

                            if (piece.bIsEquippable && !piece.bIsEquipped)
                            {
                                Console.WriteLine("Would you like to equip this item? (Y/N)");
                                if (Console.ReadLine() == "Y" && this.level >= piece.stats.RequiredLevel) this.EquipItem(piece);
                                if (this.level < piece.stats.RequiredLevel) Console.WriteLine("This item cannot be equipped.");
                            }
                            else if (piece.bIsEquippable && piece.bIsEquippable) Console.WriteLine("Item is already equipped!");
                            else if (!piece.bIsEquippable) Console.WriteLine("You cannot equip this item.");
                            Console.WriteLine("\n\n" + "Would you like to view other items? (Y/N)");
                            if (Console.ReadLine() == "Y") goto InventoryStart;
                            bIsDone = true;
                            Map.DrawMap(this.CurrentMap, this);
                        }
                    }


                }
            }
            else if (Story.GetKey(0x1B))
            {
                Map.DrawMap(this.CurrentMap, this);
                return;
            }
        }
    }


}