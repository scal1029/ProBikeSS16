using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class Order
    {
        int Periode;
        int Auftragnummer;
        int ArtikelID;
        int Menge;

        public Order(int _Periode, int _Auftrag, int _ID, int _Menge)
        {
            Periode = _Periode;
            Auftragnummer = _Auftrag;
            ArtikelID = _ID;
            Menge = _Menge;
        }
    }
}
