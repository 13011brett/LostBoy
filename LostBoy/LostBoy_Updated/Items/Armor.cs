using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostBoy.Items
{
    public abstract class Armor : ObtainableItem
    {
        public int Defense { get; protected set;  }

        public Armor()
        {

        }
    }
}
