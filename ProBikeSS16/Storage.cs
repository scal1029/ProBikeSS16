using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    sealed class Storage
    {
        static readonly Storage instance = new Storage();



        private Storage() { }
    }
}
