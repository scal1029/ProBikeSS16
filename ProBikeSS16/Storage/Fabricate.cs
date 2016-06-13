using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class Fabricate : PEK
    {
        public Fabricate(uint id, int quantity = 0) : base(id, quantity)
        {
            prefix = 'E';
        }
    }
}
