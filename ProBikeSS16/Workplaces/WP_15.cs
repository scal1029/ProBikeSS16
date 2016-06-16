namespace ProBikeSS16.Workplaces
{
    class WP_15 : Workplace
    {

        static int order_E17 = 0;
        static int order_E26 = 0;

        #region Getter/Setter
        public int ProdTimeE17
        {
            get
            {
                return getApproxProdTimeE17(order_E17);
            }
        }

        public int ProdTimeE26
        {
            get
            {
                return getApproxProdTimeE26(order_E26);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE17 + ProdTimeE26;
            }
        }

        public int NeedOf7DirectTo15_P1
        {
            get { return getNeedOf7DirectTo15(GlobalVariables.P1Produktionsauftrag); }
        }

        public int NeedOf7DirectTo15_P2
        {
            get { return getNeedOf7DirectTo15(GlobalVariables.P2Produktionsauftrag); }
        }

        public int NeedOf7DirectTo15_P3
        {
            get { return getNeedOf7DirectTo15(GlobalVariables.P3Produktionsauftrag); }
        }

        public int NeedOfK43
        {
            get { return getNeedOfK43(order_E17); }
        }

        public int NeedOfE17
        {
            get { return getNeedOfK44(order_E17); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK45(order_E17); }
        }

        public int NeedOfK46
        {
            get { return getNeedOfK46(order_E17); }
        }

        public int NeedOfK47
        {
            get { return getNeedOfK47(order_E26); }
        }

        public int Order_E17
        {
            get
            {
                return order_E17;
            }

            set
            {
                order_E17 = value;
            }
        }

        public int Order_E26
        {
            get
            {
                return order_E26;
            }

            set
            {
                order_E26 = value;
            }
        }
        #endregion

        public WP_15(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
            fillProductionOrders();
        }

        public override void fillProductionOrders()
        {
            Order_E17 = GlobalVariables.E17Produktionsauftrag;
            Order_E26 = GlobalVariables.E26Produktionsauftrag;
        }

        #region Production E17
        public void produce_one_bath_e17()
        {
            if (order_E17 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_E17 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[43].Quantity < prod_batch ||
                storage.Content[44].Quantity < prod_batch ||
                storage.Content[45].Quantity < prod_batch ||
                storage.Content[46].Quantity < prod_batch)
                return;

            storage.Content[43].Quantity -= (1 * prod_batch);
            storage.Content[44].Quantity -= (1 * prod_batch);
            storage.Content[45].Quantity -= (1 * prod_batch);
            storage.Content[46].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE17(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E26
        public void produce_one_bath_e56()
        {
            if (order_E26 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_E26 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[47].Quantity < prod_batch)
                return;

            storage.Content[47].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE26(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeE17(int e17Val)
        {
            return 3 * e17Val;
        }

        public int getApproxProdTimeE26(int e26Val)
        {
            return 3 * e26Val;
        }

        public int getNeedOf7DirectTo15(int val)
        {
            return 1 * val;
        }

        public int getNeedOfK43(int k43Val)
        {
            return 1 * k43Val;
        }

        public int getNeedOfK44(int k44Val)
        {
            return 1 * k44Val;
        }

        public int getNeedOfK45(int k45Val)
        {
            return 1 * k45Val;
        }

        public int getNeedOfK46(int k46Val)
        {
            return 1 * k46Val;
        }

        public int getNeedOfK47(int k47Val)
        {
            return 1 * k47Val;
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "\nOrder E17: " + Order_E17
                + "\nOrder E26: " + Order_E26;
        }
    }
}
