using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class XMLselldirect
    {
        public int article;
        public int quantity;
        public double price;
        public double penalty;

        public XMLselldirect(int a, int q, double p, double pe)
        {
            article = a;
            quantity = q;
            price = p;
            penalty = pe;
        }
    }
}
