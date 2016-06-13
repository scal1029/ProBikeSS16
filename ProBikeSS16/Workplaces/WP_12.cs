using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.Workplaces
{
    class WP_12 : Workplace
    {

        int order_DirectTo8 = 0;
        int cur_prod = 0;
        int onMachine = 0;

        #region Getter/Setter
        public int ProdTimeDirectTo8
        {
            get
            {
                return getApproxProdTimeDirectTo8(order_DirectTo8);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeDirectTo8;
            }
        }

        public int NeedOfDirectFrom13
        {
            get { return getNeedOfDirectFrom13(order_DirectTo8); }
        }
        #endregion

        public WP_12(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
        }
        
        #region Production DirectTo8
        public void produce_one_bath_DirectTo8()
        {
            if (order_DirectTo8 <= 0 && onMachine == 0)
                return;
            
            if (onMachine == 0)
            {
                order_DirectTo8 -= prod_batch;
                onMachine += prod_batch;
            }

            if (Directs.From13DirectTo12_Stock < prod_batch)
                return;

            Directs.From13DirectTo12_Stock -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion
       
        #region Methods
        public int getApproxProdTimeDirectTo8(int directTo8)
        {
            return 3 * directTo8;
        }

        public int getNeedOfDirectFrom13(int d13Val)
        {
            return 1 * d13Val;
        }
        #endregion
    }
}
