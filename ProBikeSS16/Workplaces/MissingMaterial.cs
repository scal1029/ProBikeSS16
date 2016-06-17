using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.Workplaces
{
    class MissingMaterial
    {
        int missingPart;
        int period;
        int productionOrder;
        int batch;
        int part;
        int amount;

        public MissingMaterial(int missingPart, int period, int productionOrder, int batch, int part, int amount)
        {
            this.missingPart = missingPart;
            this.period = period;
            this.productionOrder = productionOrder;
            this.batch = batch;
            this.part = part;
            this.amount = amount;
        }
    }
}
