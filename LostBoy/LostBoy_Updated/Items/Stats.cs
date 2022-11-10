using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Stats : ICloneable
    {
        public float Health { get; protected set; } = 0;
        public float MovementSpeed { get; protected set; } = 0;
        public int AttackPower { get; protected set; } = 0;
        public int Level { get; private set; } = 1;
        public int Experience { get; protected set; } = 0;

        public Stats(float health = 0, float movementSpeed = 1, int attackPower = 0)
        {
            Health = health;
            MovementSpeed = movementSpeed;
            AttackPower = attackPower;
        }

        public virtual void SetLevel( int theLevel ) 
        {
            if (this is ItemStats) return;

            this.AttackPower = 10 * theLevel;
        }
        public object Clone() => this.MemberwiseClone();

        
        

    }

    public sealed class ItemStats : Stats
    {

    }

    public class StatsBuilder : Stats
    {
        private Stats buildee;

        public virtual StatsBuilder SetHealth(float newHp)
        {
            this.Health = newHp;
            return this;

        }
        public virtual StatsBuilder SetAPBase(int ap)
        {
            this.AttackPower = ap;
            return this;
        }

        public virtual StatsBuilder SetMovementSpeed(float speed)
        {
            this.MovementSpeed = speed;
            return this;
        }

        public StatsBuilder()
        {
            this.buildee = new Stats();
        }

        public Stats? Build()
        {
            var shadow = this.buildee.Clone() as Stats;
            this.buildee = new Stats();
            return shadow;
        }
        
        
    }
}
