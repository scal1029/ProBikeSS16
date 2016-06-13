using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    public class Order
    {
        public int Periode;
        public int Auftragnummer;
        public int ArtikelID;
        public int Menge;
        //public List<Arbeitsplatzprototyp> PassendeArbeitsplätze;
        public TeilPrototyp Teil;

        public Order(int _Periode, int _Auftrag, int _ID, int _Menge, TeilPrototyp ETeil)
        {
            Periode = _Periode;
            Auftragnummer = _Auftrag;
            ArtikelID = _ID;
            Menge = _Menge;
            //PassendeArbeitsplätze = Passend;
        }
    }
}
