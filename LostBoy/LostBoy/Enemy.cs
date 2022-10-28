using LostBoy;
using System.Runtime.InteropServices;
using System;

struct Vec3 { public float x; public float y; public float z; }; // Z May be used just to dictate the level we're on? Not quite sure. Going to be a 2d game currently.

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


    public Enemy(Map map)
    {
        this.level = (map.MapDifficulty); // Difficulty of map = monster level (? might not be best.).
        this.Health = ((this.level * RandomFloatNumber(1, 3)) + 100);
        this.monster = (Monster)RandomNumber(0, 2);


    }

}

