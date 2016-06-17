using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.Workplaces
{
    class WaitList
    {
        int period;
        int productionOrder;
        int batch;
        int part;
        int amount;

        public WaitList(int period, int productionOrder, int batch, int part, int amount)
        {
            this.period = period;
            this.productionOrder = productionOrder;
            this.batch = batch;
            this.part = part;
            this.amount = amount;
        }
    }
}
