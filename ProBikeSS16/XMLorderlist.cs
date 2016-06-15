using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class XMLorderlist
    {
        public int article;
        public int quantity;
        public int modus;

        public XMLorderlist(int a, int q, int m)
        {
            article = a;
            quantity = q;
            modus = m;
        }
    }
}
