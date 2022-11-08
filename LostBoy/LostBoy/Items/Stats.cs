using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Stats
    {
        public float Health { get; protected set; }
        public float MovementSpeed { get; protected set; }
        public int AttackPower { get; protected set; }

        public Stats(float health = 0, float movementSpeed = 1, int attackPower = 0)
        {
            Health = health;
            MovementSpeed = movementSpeed;
            AttackPower = attackPower;
        }
        
        public void SetHealth(float inHealth = 0)
        {
            this.Health = inHealth;
        }
    }
}
