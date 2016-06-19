using ProBikeSS16.Workplaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace ProBikeSS16
{
    public sealed class Factory
    {
        static readonly Factory instance = new Factory();

        Dictionary<int, Workplace> workplaces = new Dictionary<int, Workplace>();
        WP_1 wp_1;
        WP_2 wp_2;
        WP_3 wp_3;
        WP_4 wp_4;
        WP_6 wp_6;
        WP_7 wp_7;
        WP_8 wp_8;
        WP_9 wp_9;
        WP_10 wp_10;
        WP_11 wp_11;
        WP_12 wp_12;
        WP_13 wp_13;
        WP_14 wp_14;
        WP_15 wp_15;


        Storage storage;

        public static Factory Instance
        {
            get
            {
                return instance;
            }
        }

        internal WP_1 Wp_1
        {
            get
            {
                return wp_1;
            }
        }

        internal WP_2 Wp_2
        {
            get
            {
                return wp_2;
            }
        }

        internal WP_3 Wp_3
        {
            get
            {
                return wp_3;
            }
        }

        internal WP_4 Wp_4
        {
            get
            {
                return wp_4;
            }
        }

        internal WP_6 Wp_6
        {
            get
            {
                return wp_6;
            }
        }

        internal WP_7 Wp_7
        {
            get
            {
                return wp_7;
            }
        }

        internal WP_8 Wp_8
        {
            get
            {
                return wp_8;
            }
        }

        internal WP_9 Wp_9
        {
            get
            {
                return wp_9;
            }
        }

        internal WP_10 Wp_10
        {
            get
            {
                return wp_10;
            }
        }

        internal WP_11 Wp_11
        {
            get
            {
                return wp_11;
            }
        }

        internal WP_12 Wp_12
        {
            get
            {
                return wp_12;
            }
        }

        internal WP_13 Wp_13
        {
            get
            {
                return wp_13;
            }
        }

        internal WP_14 Wp_14
        {
            get
            {
                return wp_14;
            }
        }

        internal WP_15 Wp_15
        {
            get
            {
                return wp_15;
            }
        }

        internal Dictionary<int, Workplace> Workplaces
        {
            get
            {
                return workplaces;
            }
        }

        private Factory()
        {
            initWorkplaces();
        }

        public void simulateWork(DataTable productionOrders)
        {
            //Beispiel Produktion P1 --> 100 Stück 
            wp_4.Order_p1 = 100;
            wp_3.Order_E51 = wp_4.NeedOfE51; //Automatisch berechnet aus Order_p1
            wp_15.Order_E17 += wp_3.NeedOfE17;
            wp_14.Order_E16 += wp_3.NeedOfE16;
            wp_2.Order_E50 += wp_3.NeedOfE50;
            wp_15.Order_E26 += wp_4.NeedOfE26;
            wp_7.Order_d15_p1 += wp_15.NeedOf7DirectTo15_P1;

            wp_4.produce_one_bath_p1(); //Einmal zehn Einheiten von P1 produzieren
        }

        //Alle Arbeitsplätze mit den gewünschten Produktionen aus GlobalVariables befüllen
        //Direkte Produktionen sind noch nicht implementiert 7 zu 15 z.B.
        public void fillProductionOrdersIntoWorkplaces()
        {
            foreach (Workplace w in workplaces.Values)
                w.fillProductionOrders();
        }

        private void initWorkplaces()
        {
            wp_1 = new WP_1((int)Constants.WORKPLACES.A1,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A1],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A1]);
            workplaces.Add((int)Constants.WORKPLACES.A1, wp_1);

            wp_2 = new WP_2((int)Constants.WORKPLACES.A2,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A2],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A2]);
            workplaces.Add((int)Constants.WORKPLACES.A2, wp_2);

            wp_3 = new WP_3((int)Constants.WORKPLACES.A3,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A3],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A3]);
            workplaces.Add((int)Constants.WORKPLACES.A3, wp_3);

            wp_4 = new WP_4((int)Constants.WORKPLACES.A4,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A4],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A4]);
            workplaces.Add((int)Constants.WORKPLACES.A4, wp_4);

            wp_6 = new WP_6((int)Constants.WORKPLACES.A6,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A6],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A6]);
            workplaces.Add((int)Constants.WORKPLACES.A6, wp_6);

            wp_7 = new WP_7((int)Constants.WORKPLACES.A7,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A7],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A7]);
            workplaces.Add((int)Constants.WORKPLACES.A7, wp_7);

            wp_8 = new WP_8((int)Constants.WORKPLACES.A8,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A8],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A8]);
            workplaces.Add((int)Constants.WORKPLACES.A8, wp_8);

            wp_9 = new WP_9((int)Constants.WORKPLACES.A9,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A9],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A9]);
            workplaces.Add((int)Constants.WORKPLACES.A9, wp_9);

            wp_10 = new WP_10((int)Constants.WORKPLACES.A10,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A10],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A10]);
            workplaces.Add((int)Constants.WORKPLACES.A10, wp_10);

            wp_11 = new WP_11((int)Constants.WORKPLACES.A11,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A11],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A11]);
            workplaces.Add((int)Constants.WORKPLACES.A11, wp_11);

            wp_12 = new WP_12((int)Constants.WORKPLACES.A12,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A12],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A12]);
            workplaces.Add((int)Constants.WORKPLACES.A12, wp_12);

            wp_13 = new WP_13((int)Constants.WORKPLACES.A13,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A13],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A13]);
            workplaces.Add((int)Constants.WORKPLACES.A13, wp_13);

            wp_14 = new WP_14((int)Constants.WORKPLACES.A14,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A14],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A14]);
            workplaces.Add((int)Constants.WORKPLACES.A14, wp_14);

            wp_15 = new WP_15((int)Constants.WORKPLACES.A15,
                Constants.VARIABLE_MACHINE_COSTS[(int)Constants.WORKPLACES.A15],
                Constants.FIX_MACHINE_COSTS[(int)Constants.WORKPLACES.A15]);
            workplaces.Add((int)Constants.WORKPLACES.A15, wp_15);

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
