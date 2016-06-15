using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    class XMLworkingtimelist
    {
        public int station;
        public int shift;
        public int overtime;

        public XMLworkingtimelist(int s, int sh, int o)
        {
            station = s;
            shift = sh;
            overtime = o;
        }

        public XMLworkingtimelist()
        {

        }
    }
}
