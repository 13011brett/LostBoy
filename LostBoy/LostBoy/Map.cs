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

    public static void FillBorder(char c)
    {
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
        foreach (var enemies in map.enemies)
        {
            int toPosX = (int)enemies.Location.x;// * ((int)map.MapSize.x); 
            int toPosY = (int)enemies.Location.y;// * ((int)map.MapSize.y); Old WEIRD method, not sure what I thought this would accomplish before....

            //I have now made it relative to window height and width :))

            while (toPosX >= Console.WindowWidth)
            {
                toPosX -= Console.WindowWidth;
            }
            while (toPosY >= Console.WindowHeight) 
            {
                toPosY -= Console.WindowHeight;
            }
            Console.SetCursorPosition(toPosX, toPosY);
            Console.Write(c);
        }
    }

    public static void ScreenMovement(Player player, Map map)
    {
        Player.Vec3 vec3;
        vec3.x = (int)((map.MapSize.x));
        vec3.y = (int)(map.MapSize.y); 
        vec3.z = map.MapSize.z;
        player.Location = vec3;
        Console.SetCursorPosition(((int)vec3.x/2), ((int)Console.WindowHeight));
        //Simulating moving with W
        

    }






}