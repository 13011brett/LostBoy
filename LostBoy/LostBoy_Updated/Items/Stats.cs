using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Stats : ICloneable
    {
        public float Health { get; set; } = 0;
        public float MovementSpeed { get; set; } = 0;
        public int AttackPower { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Level { get; set; } = 1;
        public int Experience { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Vitality { get; set; } = 0;

        public Stats(float health = 0, float movementSpeed = 1, int attackPower = 0)
        {
            Health = health;
            MovementSpeed = movementSpeed;
            AttackPower = attackPower;
        }

        public virtual void SetLevel(int theLevel)
        {
            if (this is ItemStats) return;

            this.AttackPower = 10 * theLevel;
        }
        public object Clone() => this.MemberwiseClone();




    }
    public sealed class EntityStats : Stats
    {


    }
    public sealed class ItemStats : Stats
    {




    }
    public class ItemStatsBuilder 
    {
        private ItemStats buildee;

        public virtual ItemStatsBuilder SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
            return this;

        }
        public virtual ItemStatsBuilder SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
            return this;
        }

        public virtual ItemStatsBuilder SetMovementSpeed(float speed)
        {
            this.buildee.MovementSpeed = speed;
            return this;
        }



        public ItemStats? Build()
        {
            var shadow = this.buildee.Clone() as ItemStats;
            this.buildee = new ItemStats();
            return shadow;
        }
        public ItemStatsBuilder() => this.buildee = new ItemStats();
    }

    public class StatsBuilder 
    {
        private Stats buildee;

        public virtual StatsBuilder SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
            return this;
        }
        public virtual StatsBuilder SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
            return this;
        }

        public virtual StatsBuilder SetMovementSpeed(float speed)
        {
            this.buildee.MovementSpeed = speed;
            return this;
        }
       


        public Stats? Build()
        {
            Stats result = this.buildee;
            this.buildee = new Stats();
            return result;
        }
        public StatsBuilder() => this.buildee = new Stats();


    }
    public interface IStatsBuilder{ // Could not get this to work with a fluent builder. Works without it 
        void SetHealth(float newHP);
        void SetAPBase(int ap);
       // Stats Build();


    }
}
