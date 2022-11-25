using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class Armor : ObtainableItem
    {
        public int DefenseModifier { get; protected set;  }
        public static Guid ItemType = Guid.NewGuid();
        

        public Armor()
        {
            this.QuantityMax = 1;
        }

    }

    public class Chainmail : Armor
    {
        public Chainmail()
        {
            
            this.stats = new ItemStatsBuilder()
                .SetArmor(Player.RandomNumber(50, 255))
                .SetHealth(Player.RandomNumber(50, 255))
                .SetLevelReq(2)
                .Build();
            this.Name = "Chainmail" + this.stats.Affix;
            this.bIsEquippable = true;
            this.itemslot = ItemSlot.Chest;
        }
    }
}


