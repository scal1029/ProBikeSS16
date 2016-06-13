using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.domain
{
    public enum type { s, b }
    class Unit
    {
        public int id;
        public type t;
        public List<Unit> lUnits;

        public Unit (int id, type t, List<Unit> li)
        {
            this.id = id;
            this.t = t;
            
            foreach(Unit u in li)
            {
                lUnits.Add(u);
            }
          
        }

        public Unit(int id, type ti)
        {
            this.id = id;
            this.t = t;
            lUnits = null;

        }

    }
}
