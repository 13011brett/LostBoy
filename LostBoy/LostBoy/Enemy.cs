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
    public char Icon
    {
        get
        {
            return icon;
        }
    }

    public Enemy(Map map)
    {

        this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
        this.Health = ((this.level * RandomFloatNumber(1, 3)) + 100);
        this.monster = (Monster)RandomNumber(0, 2);
        this.icon = (char)this.monster;


    }
    public Enemy(Map map, Vec3 loc)
    {

        this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
        this.Health = ((this.level * RandomFloatNumber(1, 3)) + 100);
        this.monster = (Monster)RandomNumber(0, 3);
        this.location = loc;
        if(this.monster == Monster.Troll)
        {
            this.icon = 'T';
        }
        else if (this.monster == Monster.Ogre)
        {
            this.icon = 'O';
        }
        else if(this.monster == Monster.Demon)
        {
            this.icon = 'D';
        }
        else this.icon = 'o';



    }

    public static void Movement(Map map)
    {
        foreach(var enemy in map.enemies)
        {
            if (enemy.Health > 0) {
                int randDirection = RandomNumber(0, 6);

                if (randDirection == 0 && (int)enemy.location.x < (Console.WindowHeight-1))
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 1 && (int)enemy.location.x > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 2 && (int)enemy.location.y < (Console.WindowWidth-1))
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 3 && (int)enemy.location.y > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
            }
        }
    }

}

