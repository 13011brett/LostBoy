using LostBoy;
using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using LostBoy.Items;

public enum Monster
{
    Troll,
    Ogre,
    Demon

}



public class Enemy : Player
{
    private Monster monster;

    public List<ObtainableItem> heldItems { get; protected set; } = new List<ObtainableItem>();
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
        this.stats.Health = ((this.level * RandomFloatNumber(1, map.MapDifficulty)) + 70);
        this.damage = 100;
        this.Armor = (.05f * map.MapDifficulty);
        this.monster = (Monster)RandomNumber(0, 2);
        this.icon = (char)this.monster;
        this.Experience = ((int)this.level *map.MapDifficulty);


    }
    public Enemy(Map map, Vec3 loc)
    {
        this.level = (map.MapLevel); // Difficulty of map = monster level (? might not be best.).
        this.stats = new StatsBuilder()
        .SetAPBase(50 * this.level)
        .Build();
            
        
        this.damage = (this.stats.AttackPower/15)+5;
        
        this.stats.Health = ((this.level * RandomFloatNumber(1, 10)) + 70);
        this.monster = (Monster)RandomNumber(0, 3);
        this.location = loc;
        this.Experience = (((int)this.level * map.MapDifficulty)+RandomNumber(0,map.MapDifficulty));
        this.heldItems.Add(new Chainmail());
        if (this.monster == Monster.Troll)
        {
            this.icon = 'T';
            this.color = ConsoleColor.Yellow;
        }
        else if (this.monster == Monster.Ogre)
        {
            this.icon = 'O';
            this.color = ConsoleColor.Magenta;
        }
        else if(this.monster == Monster.Demon)
        {
            this.icon = 'D';
            this.color = ConsoleColor.DarkRed;
        }
        else this.icon = 'o';



    }

    public static void Movement(Map map)
    {
        foreach(var enemy in map.enemies)
        {
            if (enemy.stats.Health > 0) {
                int randDirection = RandomNumber(0, 1000);
                ConsoleColor oldColor = Console.ForegroundColor;
                Console.ForegroundColor = enemy.color;



                if (randDirection == 0 && (int)enemy.location.y < (Console.WindowHeight-1))
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 1 && (int)enemy.location.y > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.y--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 2 && (int)enemy.location.x < (Console.WindowWidth-2))
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x++;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                if (randDirection == 3 && (int)enemy.location.x > 1)
                {
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(" ");
                    enemy.location.x--;
                    Console.SetCursorPosition((int)(enemy.location.x), ((int)enemy.location.y));
                    Console.Write(enemy.icon);
                }
                Console.ForegroundColor = oldColor;
            }
        }
    }

}

