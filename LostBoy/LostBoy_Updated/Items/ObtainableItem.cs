using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public abstract class ObtainableItem
    {
        public  Guid ID { get; private set; } = Guid.NewGuid();
        public ItemStats stats { get; set; }
        public int InventorySlot { get; set; }
        public int LevelRequirement { get; protected set; }
        public string Name { get; set; }
        public string Type { get; protected set; }
        public bool bIsEquippable { get; protected set; }
        public bool bIsEquipped { get; set; } = false;
        public bool bIsConsumable { get; protected set; }
        public ItemSlot itemslot { get; protected set; }
        public int Quality { get; protected set; }
        public int Quantity { get; set; } = 1;
        public int QuantityMax { get; protected set; } = 1;
            
        public ObtainableItem()
        {
            //this.stats.Health = 100;
            //this.ID = Guid.NewGuid();
        }
        public void ProperLevel(Player p)
        {
            if (p.level >= this.LevelRequirement)
            {
                this.bIsEquippable = true;
            }
        }
        public enum ItemSlot
        {
            Head,
            Shoulders,
            Chest,
            Gloves,
            Belt,
            Legs,
            Feet,
            Necklace,
            Ring,
            Hands

        }
    }
}
