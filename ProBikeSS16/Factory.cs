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

        Dictionary<int, Workplace> workplaces = new Dictionary<int, Workplace>();

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
            //workplaces.Add(i, (new WP_1(i, Constants.VARIABLE_MACHINE_COSTS[i], Constants.FIX_MACHINE_COSTS[i]));
            //Console.WriteLine(workplaces[i]);
            
        }

        internal void initStorage(DataSet inputDataSetWithoutOldBatchCalc)
        {
            storage = Storage.Instance;
            storage.fillData(inputDataSetWithoutOldBatchCalc);
        }
    }
}
