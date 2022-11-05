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
    public abstract class Entity // This will be used when I refactor the project, not in use currently.
    {
        public Vec3 Location { get; protected set; }

        public float Experience { get; protected set; }

        public ConsoleColor colour { get; protected set; }
        public double Health { get; protected set; }
        public int Level { get; protected set; }
        public char Icon { get; protected set; }
        public double BaseAttackPower { get; protected set; }
        public double BaseArmorFactor { get; protected set; }
        public bool bIsAlive { get; protected set; }
        public double MovementSpeedFactor { get; protected set; }

        public string Name { get; protected set; }

        public Guid EntityID { get; protected set; }

        public abstract void DoMovement();

        public Entity()
        {
            this.colour = ConsoleColor.White;
            this.Name = "testing entity base class";
            this.EntityID = Guid.NewGuid(); // Not really my best stuff but eh it's a GUID for the monsters
            //this.BaseArmorFactor = (.03 * inLevel);
            //this.BaseAttackPower = (Player.RandomNumber(1, 10) * inLevel);
        }


    }
}
