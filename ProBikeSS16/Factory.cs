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

            workplaces.Add((int)Constants.WORKPLACES.A1, 
                new Workplaces.WP_1((int)Constants.WORKPLACES.A1, 
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A1], 
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A1]));

            workplaces.Add((int)Constants.WORKPLACES.A2,
                new Workplaces.WP_2((int)Constants.WORKPLACES.A2,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A2],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A2]));

            workplaces.Add((int)Constants.WORKPLACES.A3,
                new Workplaces.WP_3((int)Constants.WORKPLACES.A3,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A3],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A3]));

            workplaces.Add((int)Constants.WORKPLACES.A4,
                new Workplaces.WP_4((int)Constants.WORKPLACES.A4,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A4],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A4]));

            workplaces.Add((int)Constants.WORKPLACES.A6,
                new Workplaces.WP_6((int)Constants.WORKPLACES.A6,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A6],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A6]));

            workplaces.Add((int)Constants.WORKPLACES.A7,
                new Workplaces.WP_7((int)Constants.WORKPLACES.A7,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A7],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A7]));

            workplaces.Add((int)Constants.WORKPLACES.A8,
                new Workplaces.WP_8((int)Constants.WORKPLACES.A8,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A8],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A8]));

            workplaces.Add((int)Constants.WORKPLACES.A9,
                new Workplaces.WP_9((int)Constants.WORKPLACES.A9,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A9],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A9]));

            workplaces.Add((int)Constants.WORKPLACES.A10,
                new Workplaces.WP_10((int)Constants.WORKPLACES.A10,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A10],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A10]));

            workplaces.Add((int)Constants.WORKPLACES.A11,
                new Workplaces.WP_11((int)Constants.WORKPLACES.A11,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A11],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A11]));

            workplaces.Add((int)Constants.WORKPLACES.A12,
                new Workplaces.WP_12((int)Constants.WORKPLACES.A12,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A12],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A12]));

            workplaces.Add((int)Constants.WORKPLACES.A13,
                new Workplaces.WP_13((int)Constants.WORKPLACES.A13,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A13],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A13]));
                
            workplaces.Add((int)Constants.WORKPLACES.A14,
                new Workplaces.WP_14((int)Constants.WORKPLACES.A14,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A14],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A14]));
                /*
            workplaces.Add((int)Constants.WORKPLACES.A15,
                new Workplaces.WP_15((int)Constants.WORKPLACES.A15,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A15],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A15]));
                */
            foreach (Workplace w in workplaces.Values)
                Console.WriteLine(w);
        }

        internal void initStorage(DataSet inputDataSetWithoutOldBatchCalc)
        {
            storage = Storage.Instance;
            storage.fillData(inputDataSetWithoutOldBatchCalc);
        }
    }
}
