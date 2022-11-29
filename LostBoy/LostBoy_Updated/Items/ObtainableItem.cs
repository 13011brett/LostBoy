using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public class ObtainableItem
    {
        public  Guid ID { get; set; } = Guid.NewGuid();
        public virtual ItemStats stats { get; set; }
        public int InventorySlot { get; set; }
        public int LevelRequirement { get; protected set; }
        public string Name { get; set; }
        //public string Type { get; protected set; }
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

        public ObtainableItem(Guid id, int lvlreq, int quality, bool isEquipped, bool isEquipabble, bool isConsumable, ItemSlot itemslot,int quantity, int quantityMax, float hp, int armor)
        {
            this.ID = id;
            this.LevelRequirement = lvlreq;
            this.Quality = quality;
            this.bIsEquipped = isEquipped;
            this.bIsEquippable = isEquipabble;
            this.bIsConsumable = isConsumable;
            this.itemslot = itemslot;
            this.Quantity = quantity;
            this.QuantityMax = quantityMax;
            this.stats = new ItemStatsBuilder()
                .SetLevelReq(lvlreq)
                .SetHealth(hp)
                .SetArmor(armor)
                .Build();
        }

        public void ProperLevel(Player p)
        {
            if (p.level >= this.LevelRequirement)
            {
                this.bIsEquippable = true;
            }
        }
        public void UseItem(Player p)
        {

            if (p.stats.Health + this.stats.Health >= p.stats.MaxHealth) p.stats.Health = p.stats.MaxHealth;
            else p.stats.Health += this.stats.Health;


            this.Quantity--;
            Console.WriteLine("Quantity Left = " + this.Quantity);
            if (this.Quantity <= 0)
            {
                
                p.playerInventory.InventoryItems.Remove(this);
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
