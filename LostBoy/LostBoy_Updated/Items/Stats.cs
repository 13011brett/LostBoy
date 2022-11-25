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
        public float Health { get; set; } = -1;
        public float MaxHealth { get; set; }
        public string Affix { get; protected set; }
        public float Mana { get; set; } = 0;
        public float MovementModifier { get; set; } = 0;
        public int AttackPower { get; set; } = 0;
        public float Armor { get; set; } = 0;
        public int Experience { get; set; } = 0;
        public int Intelligence { get; set; } = 0;
        public int Strength { get; set; } = 0;
        public int Dexterity { get; set; } = 0;
        public int Vitality { get; set; } = 0;

        public Stats()
        {


        }
        public object Clone() => this.MemberwiseClone();

        public virtual void OutputStats()
        {
            if(this is ItemStats && Health > -1) Console.WriteLine("Health = " + Health);
            if (!(this is ItemStats) && Health > -1)Console.Write("Health = " + Health);
            if (MaxHealth > 0) Console.Write("/" + MaxHealth + "\n");
            if (Mana != 0) Console.WriteLine("Mana = " + Mana);
            if (AttackPower != 0) Console.WriteLine("Attack Power Rating = " + AttackPower);
            if(Armor != 0) Console.WriteLine("Armor = " + Armor);
            if(Strength != 0) Console.WriteLine("Strength = " + Strength);
            if(Dexterity != 0) Console.WriteLine("Dexterity = " + Dexterity);
            if(Vitality != 0) Console.WriteLine("Vitality = " + Vitality);
            if (Intelligence != 0) Console.WriteLine("Intelligence = " + Intelligence);
        }
        public void GetAffix()
        {
            float BiggestNumber = 0;
            if (this.Health > BiggestNumber)
            {
                this.Affix = " of Health";
                BiggestNumber = this.Health;
            }
            if (this.Mana > BiggestNumber)
            {
                this.Affix = " of Mana";
                BiggestNumber = this.Mana;
            }
            if (this.Armor > BiggestNumber)
            {
                this.Affix = " of Defense";
                BiggestNumber = this.Armor;

            }
        }




    }
    public sealed class EntityStats : Stats
    {


    }
    public sealed class ItemStats : Stats
    {
        public int RequiredLevel { get; set; } = 1;
        public override void OutputStats()
        {
            Console.WriteLine("Required Level = " + RequiredLevel + "\n");
            base.OutputStats();
            Console.Write("\n\n");
        }

    }
    public sealed class ConsumableStats : Stats
    {
        public override void OutputStats()
        {
            if (this.Health > 0) Console.WriteLine("Health Modified: " + this.Health);
            if(this.Mana > 0) Console.WriteLine("Mana Modified: " + this.Mana);
            base.OutputStats();
        }

    }

    public class ConsumableStatsBuilder
    {
        private ConsumableStats buildee;

        public virtual ConsumableStatsBuilder SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
            return this;

        }
        public virtual ConsumableStatsBuilder SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
            return this;
        }


        public ConsumableStats? Build()
        {
            var shadow = this.buildee.Clone() as ConsumableStats;
            this.buildee = new ConsumableStats();
            return shadow;
        }
        public ConsumableStatsBuilder() => this.buildee = new ConsumableStats();
    }



    public class ItemStatsBuilder 
    {
        private ItemStats buildee;

        public virtual ItemStatsBuilder SetHealth(float newHp)
        {
            this.buildee.Health = newHp;
            return this;

        }
        public virtual ItemStatsBuilder SetLevelReq(int Level)
        {
            this.buildee.RequiredLevel = Level;
            return this;

        }
        public virtual ItemStatsBuilder SetAPBase(int ap)
        {
            this.buildee.AttackPower = ap;
            return this;
        }

        public virtual ItemStatsBuilder SetArmor(float InArmor)
        {
            this.buildee.Armor = (this.buildee.RequiredLevel * this.buildee.Strength * 1.3f + InArmor);
            return this;
        }


        public ItemStats? Build()
        {
            this.buildee.GetAffix();
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
            this.buildee.MaxHealth = newHp;
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

        public virtual StatsBuilder SetArmor(int armor)
        {
            this.buildee.Armor = armor;
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
