using LostBoy;
using System.Runtime.InteropServices;
using System;



public enum Monster
{
    Troll,
    Ogre,
    Demon

}



public class Enemy : Player
{
    private Monster monster;

    public Monster Monster
    {
        get
        {
            return monster;
        }
        set
        {
            monster = value;
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

    public Enemy(Map map)
    {

        this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
        this.Health = ((this.level * RandomFloatNumber(1, 3)) + 100);
        this.monster = (Monster)RandomNumber(0, 2);


    }
    public Enemy(Map map, Vec3 loc)
    {

        this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
        this.Health = ((this.level * RandomFloatNumber(1, 3)) + 100);
        this.monster = (Monster)RandomNumber(0, 2);
        this.location = loc;


    }

    public static void Movement(Map map)
    {
        foreach(var enemy in map.enemies)
        {
            if (enemy.Health > 0) {
                int randDirection = RandomNumber(0, 300);

                if (randDirection == 0 && (int)enemy.location.x < Console.WindowHeight)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write("o");
                }
                if (randDirection == 1 && (int)enemy.location.x > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write("o");
                }
                if (randDirection == 2 && (int)enemy.location.y < Console.WindowWidth)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write("o");
                }
                if (randDirection == 3 && (int)enemy.location.y > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write("o");
                }
            }
        }
    }

}

