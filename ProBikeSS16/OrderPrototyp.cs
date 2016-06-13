using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    [Serializable]
    public class OrderPrototyp
    {
        public int Artikel;
        public int Menge;
        public TeilPrototyp TeilPrototyp;

        public OrderPrototyp(int A, int M, TeilPrototyp T)
        {
            Artikel = A;
            Menge = M;
            TeilPrototyp = T;
        }
    }
}
