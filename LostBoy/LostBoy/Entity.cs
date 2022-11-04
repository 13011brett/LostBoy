using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class Vec3
{
    public double X { get; set; }
    public double Y { get; set; } 
    public double Z { get; set; } 

    public Vec3(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

namespace LostBoy
{
    public abstract class Entity
    {
        public Vec3 Location { get; protected set; }

        public float Experience { get; protected set; }

        public ConsoleColor colour { get; protected set; }
        public double Health { get; protected set; }
        public char Icon { get; protected set; }
        public double AttackPower { get; protected set; }
        public double ArmorFactor { get; protected set; }
        public bool bIsAlive { get; protected set; }
        public double MovementSpeedFactor { get; protected set; }

        public string Name { get; protected set; }

        public abstract void DoMovement();


    }
}
