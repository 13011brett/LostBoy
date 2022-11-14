using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Weapon : ObtainableItem
    {
        public int AttackDamage { get; protected set; }
        public double CriticalFactor { get; protected set; }  

        public Weapon()
        {
            this.bIsConsumable = false;
            this.ItemSlot = 0;
        }
        


    }
}
