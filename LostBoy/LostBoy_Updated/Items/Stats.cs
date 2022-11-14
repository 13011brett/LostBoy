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
        public float MovementModifier { get; set; } = 0;
        public int AttackPower { get; set; } = 0;
        public int Armor { get; set; } = 0;
        public int Experience { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Vitality { get; set; } = 0;

        public Stats()
        {


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

        public virtual StatsBuilder SetMovementModifier(float speed)
        {
            this.buildee.MovementModifier = speed;
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
