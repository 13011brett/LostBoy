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

}

