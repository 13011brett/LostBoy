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
        private const int MAX_ITEM_SLOTS = 10;
        public List<ObtainableItem> obtainableItems { get; protected set; } = new List<ObtainableItem>();
        public void AddItem(ObtainableItem item, int quantity)
        {
            foreach(var piece in obtainableItems)
            {
                if(item.ID == piece.ID)
                {
                    if (quantity + piece.Quantity < piece.QuantityMax) piece.Quantity += quantity;
                }
            }
 


            if (obtainableItems.Count < MAX_ITEM_SLOTS)
            {
                if(quantity > item.QuantityMax) item.Quantity = item.QuantityMax;
                else item.Quantity = quantity;
                obtainableItems.Add(item);
            }
                
         }
            
        }





    
}
