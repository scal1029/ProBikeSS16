using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    [Serializable]
    public class ArbeitsstationPrototyp
    {
        public string ID;
        public Arbeitsplatzprototyp Arbeitsplatz;
        public int Rüstzeit;
        public int Prodzeit;
        public int Warteschlange;
        //ID dann Menge
        public Dictionary<int, int> TeileProStation;
        public string BegründungStop; //keine Teile, keine Warteschlange
        public int Teil;
        public bool Produziert;
        public bool Done;

        public ArbeitsstationPrototyp(string id, Arbeitsplatzprototyp AP, int RüZe, int Püze,
            Dictionary<int, int> TpS, string Initial, int T)
        {
            ID = id;
            Arbeitsplatz = AP;
            Rüstzeit = RüZe;
            Prodzeit = Püze;
            Warteschlange = 0;
            TeileProStation = TpS;
            BegründungStop = Initial;
            Teil = T;
            Produziert = false;
            Done = false;
        }
    }
}
