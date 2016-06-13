using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class PurchasePart : PEK
    {
        public PurchasePart(uint id, int quantity = 0) : base(id, quantity)
        {
            prefix = 'K';
        }
    }
}
