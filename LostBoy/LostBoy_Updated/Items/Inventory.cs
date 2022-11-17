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
            bool MatchFound = false;
            foreach (var piece in obtainableItems)
            {
                
                if(item.ID == piece.ID)
                {
                    if (quantity + piece.Quantity < piece.QuantityMax) piece.Quantity += quantity;
                    else piece.Quantity = piece.QuantityMax;
                    MatchFound = true;
                    break;
                }
               
            }
 


            if (obtainableItems.Count < MAX_ITEM_SLOTS && !MatchFound) // Only  create an item if an item slot exists for it and a match is not found for the identifier. 
            {
                if(quantity > item.QuantityMax) item.Quantity = item.QuantityMax;
                else item.Quantity = quantity;
                obtainableItems.Add(item);
            }
                
         }

        public void ViewInventory()
        {
            Console.Clear();
            foreach (var piece in obtainableItems)
            {
                Console.WriteLine(piece.Name);
                piece.stats.OutputStats();

            }
        }
            
    }





    
}
