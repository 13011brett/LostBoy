using LostBoy.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy
{
    public class Inventory
    {
        private const int MAX_ITEM_SLOT = 10;
        public List<ObtainableItem> obtainableItems { get; protected set; } = new List<ObtainableItem>();
        public void AddItem(ObtainableItem item, int quantity)
        {
            while(quantity> 0 && obtainableItems.Count < MAX_ITEM_SLOT)
            {
                if(item is Armor)
                {
                    for(int i = 0; i < quantity; i++)
                    {
                        obtainableItems.Add(item);
                    }
                }
            }
        }





    }
}
