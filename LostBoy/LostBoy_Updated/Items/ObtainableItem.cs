using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public abstract class ObtainableItem
    {
        public Guid ID { get;  private set; }
        public ItemStats stats { get; set; }
        public int LevelRequirement { get; protected set; }
        public string Name { get; set; }
        public string Type { get; protected set; }
        public bool bIsEquippable { get; protected set; }
        public bool bIsConsumable { get; protected set; }
        public int ItemSlot { get; protected set; }
        public int Quality { get; protected set; }
            
        public ObtainableItem()
        {
            //this.stats.Health = 100;
            this.ID = new Guid();
        }
        public void ProperLevel(Player p)
        {
            if (p.level >= this.LevelRequirement)
            {
                this.bIsEquippable = true;
            }
        }
    }
}
