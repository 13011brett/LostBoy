using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Security.Cryptography;

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

    public static void EnemiesToScreen(char c, Map map)
    {
        foreach (var enemies in map.enemies)
        {

                        
            Console.SetCursorPosition((int)(enemies.Location.x*map.MapSize.x), (int)(enemies.Location.y));
            Console.Write(c);
        }
    }


}