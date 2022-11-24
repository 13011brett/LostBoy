using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Consumables : ObtainableItem
    {

        public Consumables()
        {
            
            this.bIsConsumable = true;
            this.bIsEquippable = false;
        }


    }
    

    public class Potion : Consumables
    {
           public Potion()
        {
            this.Name = "Potion";
            this.Quantity = 1;
            this.QuantityMax = 10;
            this.stats = new ItemStatsBuilder()
                .SetHealth(1000)
                .Build();
        }
    }
}
