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
    public class ItemStatsBuilder : IStatsBuilder
    {
        private ItemStats buildee;

        public virtual IStatsBuilder SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
            return this;

        }
        public virtual IStatsBuilder SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
            return this;
        }

        public virtual IStatsBuilder SetMovementSpeed(float speed)
        {
            this.buildee.MovementSpeed = speed;
            return this;
        }



        public Stats? Build()
        {
            var shadow = this.buildee.Clone() as Stats;
            this.buildee = new ItemStats();
            return shadow;
        }
        public ItemStatsBuilder() => this.buildee = new ItemStats();
    }

    public class StatsBuilder : IStatsBuilder
    {
        private Stats buildee;

        public virtual void SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
        }
        public virtual void SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
        }

        public virtual IStatsBuilder SetMovementSpeed(float speed)
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
    public interface IStatsBuilder{
        void SetHealth(float newHP);
        void SetAPBase(int ap);
       // Stats Build();


    }
}
