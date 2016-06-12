using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.Workplaces
{
    class WP_13 : Workplace
    {

        int order_DirectTo12 = 0;
        int cur_prod = 0;
        int onMachine = 0;

        #region Getter/Setter
        public int ProdTimeDirectTo12
        {
            get
            {
                return getApproxProdTimeDirectTo12(order_DirectTo12);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeDirectTo12;
            }
        }

        public int NeedOfK39
        {
            get { return getNeedOfK39(order_DirectTo12); }
        }
        #endregion

        public WP_13(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
        }
        
        #region Production DirectTo12
        public void produce_one_bath_DirectTo12()
        {
            if (order_DirectTo12 <= 0 && onMachine == 0)
                return;
            
            if (onMachine == 0)
            {
                order_DirectTo12 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[39].Quantity < prod_batch)
                return;

            storage.Content[39].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion
       
        #region Methods
        public int getApproxProdTimeDirectTo12(int directTo12)
        {
            return 2 * directTo12;
        }

        public int getNeedOfK39(int k39Val)
        {
            return 1 * k39Val;
        }
        #endregion
    }
}
