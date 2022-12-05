using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

public class Map : Player
{
    private Player.Vec3 mapSize;
    private int mapDifficulty;
    public List<Enemy> enemies = new List<Enemy>();
    public static char Border { get; protected set; }
    


    public Player.Vec3 MapSize
    {
        get { return mapSize; }
    }
    public int MapDifficulty
    {
        get
        {
            return mapDifficulty;
        }
        set
        {
            mapDifficulty = value;
        }
    }
    public bool ResetLoc { get; protected set; } = true;

    public Map(float map_x, float map_y, int difficulty)
    {
        this.mapSize.x = map_x;
        this.mapSize.y = map_y;
        this.mapDifficulty = difficulty;
        if(this.mapSize.x > (Console.LargestWindowWidth-20)) this.mapSize.x = (Console.LargestWindowWidth-20);
        if (this.mapSize.y > (Console.LargestWindowHeight-10)) this.mapSize.y = (Console.LargestWindowHeight-10);
        EnemyCreation(this);

    }
    public void EnemyCreation(Map map) // All enemy creation will be done within the map, currently works off of map difficulty but can be modularized. 
    {

//        Enemy[] enemy = new Enemy[map.mapDifficulty];
        for (int i = 0; i < map.mapDifficulty; i++)
        {
            Player.Vec3 loc;
            loc.x = Player.RandomNumber(1, (int)map.MapSize.x);  // rand x for monster
            loc.y = Player.RandomNumber(1, (int)map.MapSize.y);  // rand y for monster
            loc.z = MapSize.z; // z can be initialized within the map.
            map.enemies.Add(new Enemy(map,loc)); // possibly need to do another for loop after this to detect for monsters on duplicate spaces

        }
    }

    public static void FillScreen(char c)
    { 
        for (int top = 0; top < Console.WindowHeight; top++)
        {
            string line = string.Empty;
            for (int left = 0; left < Console.WindowWidth; left++)
            {
                line += c;
            }

            Console.SetCursorPosition(0, top);
            Console.Write(line);
        }
    }
    public static void TimeClearScreen(int timer = 0)
    {
        for(int top = 0; top < Console.WindowHeight; top++)
        {
            string line = string.Empty;
            for(int left = 0; left < Console.WindowWidth; left++)
            {
                Console.SetCursorPosition(left, top);
                Console.Write('-');
                System.Threading.Thread.Sleep(timer);

            }
           
        }
        Console.Clear();
    }

    public static void FillBorder(char c = '-')
    {
        Map.Border = c;
        for(int i = 0; i < Console.WindowHeight; i++)
        {
            if (i == 0)
            {
                for(int line = 0; line < Console.WindowWidth; line++)
                {
                    Console.SetCursorPosition(line, 0);
                    Console.Write(c);
                }   
            }
            Console.SetCursorPosition(0, i);
            Console.Write('|');
            Console.SetCursorPosition(Console.WindowWidth - 1, i);
            Console.Write('|');
        }
    }

    public static void EnemiesToScreen(Map map) // Currently not working as expected; need to find a way to map it not go out of window bounds.
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkRed; // Could probably eventually make this the color of the monster :D
        foreach (var enemies in map.enemies)
        {
            //int toPosX = (int)enemies.Location.x;// * ((int)map.MapSize.x); 
            //int toPosY = (int)enemies.Location.y;// * ((int)map.MapSize.y); Old WEIRD method, not sure what I thought this would accomplish before....

            //I have now made it relative to window height and width :))
            if (enemies.stats.Health >= 0)
            {
                if (enemies.LocationX == 0) enemies.LocationX += 1;
                else if (enemies.LocationX == (Console.WindowWidth - 1)) enemies.LocationX -= 1;

                if (enemies.LocationY == 0) enemies.LocationY += 1;
                else if (enemies.LocationY == (Console.WindowHeight - 1)) enemies.LocationY -= 1;

                Console.SetCursorPosition((int)enemies.Location.x, (int)enemies.Location.y);
                Console.Write(enemies.Icon);
            }
        }
        Console.ForegroundColor = currentColor;
    }
    public static void DrawMap(Map map, Player player)
    {
        if (map.ResetLoc)
        {
            player.ResetLocation(ref map);
            map.ResetLoc = false;
        }
        Console.Clear();
        player.CurrentMap = map;
        Console.SetWindowSize(((int)map.mapSize.x), ((int)map.mapSize.y));
        Console.SetBufferSize((int)map.mapSize.x, (int)map.mapSize.y);
        Map.FillBorder();
        Map.EnemiesToScreen(map);
        Console.ForegroundColor = ConsoleColor.Green;
        Map.ScreenMovement(player, map);
        


    }
    public static Map endGame = new Map(100, 100, 0);

    public static void AttackSequence(Player player, Map map)
    {
        foreach (var ene in map.enemies)
        {
            if (ene.LocationX == player.Location.x && ene.LocationY == player.Location.y && ene.stats.Health > 0)
            {

                
                Map.TimeClearScreen(0);
                Console.SetCursorPosition(0, 0);
                Console.SetWindowSize(200, 60);
                Console.Write("Player Health is " + player.stats.Health + "\nEnemy health is " + ene.stats.Health + "\n\n\n" + "Press K to attack, R to run away!\n\n" + "Current Experience = " + player.Experience + "/" + player.ExperienceRequired + "\nCurrent Level = " + player.level);

                do
                {
                    if (player.stats.Health <= 0)
                    {
                        Console.Clear();
                        Console.WriteLine("You are too weak to fight!");
                        System.Threading.Thread.Sleep(2500);
                        player.stats.Health = 0;
                        player.ResetLocation(ref map);
                        break;

                    }

                    if ((Story.GetAsyncKeyState(0x4B) & 0x8000) == 0x8000)
                    {
                        Console.Clear();

                        Player.Damage(ref player, ene);
                        if (ene.stats.Health <= 0) break;
                        else if(player.stats.Health <= 0)
                        {

                            Console.Clear();
                            Console.WriteLine("You are too weak to fight!");
                            player.stats.Health = 0;
                            System.Threading.Thread.Sleep(2500);
                            player.ResetLocation(ref map);
                            break;

                            
                        }
                        Console.Write("Player Health is " + player.stats.Health + "\nEnemy health is " + ene.stats.Health + "\n\n\n" + "Press K to attack, R to run away!\n\n" + "Current Experience = " + player.Experience + "/" + player.ExperienceRequired + "\nCurrent Level = " + player.level);
                        System.Threading.Thread.Sleep(100);


                    }
                    if ((Story.GetAsyncKeyState(0x52) & 0x8000) == 0x8000)
                    {
                        player.ResetLocation(ref map);
                        break;
                    }
                } while (player.stats.Health >= 0);
                Console.WriteLine(ene.Name + " Vanquished!\n" + "Experience Gained: " + ene.Experience );
                GainExperience(player, ene);
                Map.DrawMap(map, player);
                
                //Console.Clear();
                //Console.SetWindowSize((int)map.mapSize.x, (int)map.mapSize.y);
                //Map.FillBorder(Map.Border);               
                //player.ResetLocation(ref map);
                //Map.EnemiesToScreen(map);
            }
        }

    }



    public static void ScreenMovement(Player player, Map map) // This is the main gameplay walking loop at this point. It holds the WASD movement and also monster movement.
    {
        Player.Vec3 vec3;
        if (player.Location.x == 0 && player.Location.y == 0 && player.Location.z == 0)
        {
            vec3.x = (int)((map.MapSize.x / 2));
            vec3.y = (int)(map.MapSize.y - 1);
            vec3.z = map.MapSize.z;
            player.Location = vec3;
            Console.CursorVisible = false;
            Console.SetCursorPosition(((int)player.Location.x / 2), ((int)(player.Location.y))); // Center screen to initialize, may want to write this into the constructor. BUT maybe not.
            Console.Write(player.icon);
        }
        else
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(((int)player.Location.x), ((int)(player.Location.y))); // Center screen to initialize, may want to write this into the constructor. BUT maybe not.
            Console.Write(player.icon);
        }
        do
        {
            AttackSequence(player, map);
            Enemy.Movement(map);
            Player.DoMovement(player, map, 1);
            if (Story.GetKey(0x49)) player.ViewInventory();
            if((Story.GetAsyncKeyState(0x1B) & 0x8000) == 0x8000) // Break out with esc
            {
                Console.Clear();
                int x = 0;
                Console.WriteLine("1: \t Save Game" + "\n2: \t Quit Game" + "\n3: \t Return to Main Menu" + "\n4: \t Continue Game");
                switch (Story.ChoiceInt())
                {
                    case 1:
                        
                        Console.WriteLine("Please Enter Your Desired Save File Name (same name to overwrite old)");
                        string fileName = Console.ReadLine();
                        if (fileName != null) player.ToXmlString(fileName);
                        else player.ToXmlString(player.Name); // Uses defaultfilename if not done properly.
                        Story.TimedText("Saving....", 200);
                        Console.WriteLine("Completed!");
                        System.Threading.Thread.Sleep(1000);
                        DrawMap(player.CurrentMap, player);
                        break;
                    case 2:
                        Environment.Exit(0);
                        return;
                    case 3:
                        //player = new Player();
                        Story.DoIntro(ref player);
                        return;
                    case 4:

                    default:
                        DrawMap(player.CurrentMap, player);
                        break;

                        
                        
                }
            }
        } while (true);
        //Simulating moving with W



    }






}