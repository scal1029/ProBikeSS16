using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    [Serializable]
    public class TeilPrototyp
    {
        public int TeilID;
        public List<ArbeitsstationPrototyp> KetteStationen;

        public TeilPrototyp(int ID, List<ArbeitsstationPrototyp> Kette)
        {
            TeilID = ID;
            KetteStationen = Kette;
        }
    }
}
