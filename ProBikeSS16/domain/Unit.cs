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

        public Unit (int id, type t)
        {
            this.id = id;
            this.t = t;
        }

  }
}
