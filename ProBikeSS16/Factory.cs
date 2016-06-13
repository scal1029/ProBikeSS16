using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    public sealed class Factory
    {
        static readonly Factory instance = new Factory();

        List<Workplace> workplaces = new List<Workplace>();

        Storage storage;

        public static Factory Instance
        {
            get
            {
                return instance;
            }
        }

        private Factory()
        {
            initWorkplaces();
        }

        private void initWorkplaces()
        {
            for (int i = 0; i < Constants.MAX_WORKPLACES; i++)
            { 
                workplaces.Add(new Workplace(i, Constants.VARIABLE_MACHINE_COSTS[i], Constants.FIX_MACHINE_COSTS[i]));
                Console.WriteLine(workplaces[i]);
            }
        }

        internal void initStorage(DataSet inputDataSetWithoutOldBatchCalc)
        {
            storage = Storage.Instance;
            storage.fillData(inputDataSetWithoutOldBatchCalc);
        }
    }
}
