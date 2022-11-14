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

        public Armor()
        {

        }
    }
}
