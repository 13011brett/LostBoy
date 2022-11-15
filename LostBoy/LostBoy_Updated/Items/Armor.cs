﻿using System;
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

    public class Chainmail : Armor
    {
        public Chainmail()
        {
            this.Name = "Chainmail";
            this.stats = new ItemStatsBuilder()
                .SetArmor(1000)
                .Build();
        }
    }
}


