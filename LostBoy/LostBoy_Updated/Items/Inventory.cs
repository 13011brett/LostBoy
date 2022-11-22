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
        public List<ObtainableItem> InventoryItems { get; protected set; } = new List<ObtainableItem>();
        public int EquippedItems = 0;
        
        public void AddItem(ObtainableItem item, int quantity)
        {
            bool MatchFound = false;
            EquippedItems = 0;
            foreach (var piece in InventoryItems)
            {
                if (piece.bIsEquipped) EquippedItems++;
                if (item.ID == piece.ID)
                {
                    if (quantity + piece.Quantity < piece.QuantityMax) piece.Quantity += quantity;
                    else piece.Quantity = piece.QuantityMax;
                    MatchFound = true;
                    break;
                }

               
            }
 


            if ((InventoryItems.Count - EquippedItems)< MAX_ITEM_SLOTS && !MatchFound) // Only  create an item if an item slot exists for it and a match is not found for the identifier. 
            {
                if(quantity > item.QuantityMax) item.Quantity = item.QuantityMax;
                else item.Quantity = quantity;
                InventoryItems.Add(item);
            }
                
         }


    }





    
}
