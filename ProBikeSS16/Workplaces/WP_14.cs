using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.Workplaces
{
    class WP_14 : Workplace
    {

        static int order_E16 = 0;

        #region Getter/Setter
        public int ProdTimeE16
        {
            get
            {
                return getApproxProdTimeE16(order_E16);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE16;
            }
        }

        public int NeedOf6DirectTo14_P1
        {
            get { return getNeedOf6DirectTo14(GlobalVariables.P1Produktionsauftrag); }
        }

        public int NeedOf6DirectTo14_P2
        {
            get { return getNeedOf6DirectTo14(GlobalVariables.P2Produktionsauftrag); }
        }

        public int NeedOf6DirectTo14_P3
        {
            get { return getNeedOf6DirectTo14(GlobalVariables.P3Produktionsauftrag); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK24(order_E16); }
        }

        public int NeedOfK40
        {
            get { return getNeedOfK40(order_E16); }
        }

        public int NeedOfK41
        {
            get { return getNeedOfK41(order_E16); }
        }

        public int NeedOfK42
        {
            get { return getNeedOfK42(order_E16); }
        }

        public int Order_E16
        {
            get
            {
                return order_E16;
            }

            set
            {
                order_E16 = value;
            }
        }
        #endregion

        public WP_14(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
            fillProductionOrders();
        }

        new public void fillProductionOrders()
        {
            Order_E16 = GlobalVariables.E16Produktionsauftrag;
        }

        #region Production E16
        public void produce_one_bath_e16()
        {
            if (order_E16 <= 0 && onMachine == 0)
                return;
            
            if (onMachine == 0)
            {
                order_E16 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[24].Quantity < prod_batch ||
                storage.Content[40].Quantity < prod_batch ||
                storage.Content[41].Quantity < prod_batch ||
                storage.Content[42].Quantity < (2 * prod_batch))
                return;

            storage.Content[24].Quantity -= (1 * prod_batch);
            storage.Content[40].Quantity -= (1 * prod_batch);
            storage.Content[41].Quantity -= (1 * prod_batch);
            storage.Content[42].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeE16(prod_batch);
            onMachine = 0;
        }
        #endregion
       
        #region Methods
        public int getApproxProdTimeE16(int e16Val)
        {
            return 3 * e16Val;
        }

        public int getNeedOf6DirectTo14(int val)
        {
            return 1 * val;
        }

        public int getNeedOfK24(int k24Val)
        {
            return 1 * k24Val;
        }

        public int getNeedOfK40(int k40Val)
        {
            return 1 * k40Val;
        }

        public int getNeedOfK41(int k41Val)
        {
            return 1 * k41Val;
        }

        public int getNeedOfK42(int k42Val)
        {
            return 2 * k42Val;
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "\nOrder E16: " + Order_E16;
        }
    }
}
