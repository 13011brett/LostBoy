using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Principal;

public class Map
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

    public Map(float map_x, float map_y, int difficulty)
    {
        this.mapSize.x = map_x;
        this.mapSize.y = map_y;
        this.mapDifficulty = difficulty;
        if(this.mapSize.x > (Console.LargestWindowWidth-20)) this.mapSize.x = (Console.LargestWindowWidth-20);
        if (this.mapSize.y > (Console.LargestWindowHeight-10)) this.mapSize.y = (Console.LargestWindowHeight-10);
        Console.SetWindowSize(((int)this.mapSize.x), ((int)this.mapSize.y));
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
                Console.Write(' ');
                System.Threading.Thread.Sleep(timer);

            }
        }
    }

    public static void FillBorder(char c)
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

    public static void EnemiesToScreen(char c, Map map) // Currently not working as expected; need to find a way to map it not go out of window bounds.
    {
        var currentColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkRed; // Could probably eventually make this the color of the monster :D
        foreach (var enemies in map.enemies)
        {
            //int toPosX = (int)enemies.Location.x;// * ((int)map.MapSize.x); 
            //int toPosY = (int)enemies.Location.y;// * ((int)map.MapSize.y); Old WEIRD method, not sure what I thought this would accomplish before....

            //I have now made it relative to window height and width :))
            if (enemies.Health >= 0)
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

    public static void AttackSequence(Player player, Map map)
    {
        foreach (var ene in map.enemies)
        {
            if (ene.LocationX == player.Location.x && ene.LocationY == player.Location.y && ene.Health > 0)
            {

                
                Map.TimeClearScreen(0);
                Console.SetCursorPosition(0, 0);
                Console.SetWindowSize(200, 60);
                Console.Write("Player Health is " + player.Health + "\nEnemy health is " + ene.Health + "\n\n\n" + "Press K to attack, R to run away!");

                do
                {

                    if ((Story.GetAsyncKeyState(0x4B) & 0x8000) == 0x8000)
                    {
                        Console.Clear();
                        if (player.Health <= 0) Console.Write("Game Over.");
                        player.Health -= ene.damage;
                        ene.Health -= 100;
                        if (ene.Health <= 0) break;
                        Console.Write("Player Health is " + player.Health + "\nEnemy health is " + ene.Health + "\n\n\n" + "Press K to attack, R to run away!");
                        //System.Threading.Thread.Sleep(10);


                    }
                    if ((Story.GetAsyncKeyState(0x52) & 0x8000) == 0x8000)
                    {
                        break;
                    }
                } while (player.Health >= 0);
                Console.Clear();
                Console.SetWindowSize((int)map.mapSize.x, (int)map.mapSize.y);
                Map.FillBorder(Map.Border);
                
                player.ResetLocation(ref map);
                
            }
        }

    }



    public static void ScreenMovement(Player player, Map map) // This is the main gameplay walking loop at this point. It holds the WASD movement and also monster movement.
    {
        Player.Vec3 vec3;
        vec3.x = (int)((map.MapSize.x/2));
        vec3.y = (int)(map.MapSize.y); 
        vec3.z = map.MapSize.z;
        player.Location = vec3;
        Console.CursorVisible = false;
        Console.SetCursorPosition(((int)vec3.x/2), ((int)vec3.y)); // Center screen to initialize, may want to write this into the constructor. BUT maybe not.
        do
        {
            AttackSequence(player, map);
            Enemy.Movement(map);
            Player.DoMovement(player, map, 0x57, 1);
            Player.DoMovement(player, map, 0x53, 1);
            Player.DoMovement(player, map, 0x41, 1);
            Player.DoMovement(player, map, 0x44, 1);
            if((Story.GetAsyncKeyState(0x1B) & 0x8000) == 0x8000) // Break out with esc
            {
                break;
            }
        } while (true);
        //Simulating moving with W



    }






}