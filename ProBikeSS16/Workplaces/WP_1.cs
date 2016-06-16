namespace ProBikeSS16.Workplaces
{
    class WP_1 : Workplace
    {

        static int order_E49 = 0;
        static int order_E54 = 0;
        static int order_E29 = 0;

        #region Getter/Setter
        public int ProdTimeE49
        {
            get
            {
                return getApproxProdTimeE49(order_E49);
            }
        }

        public int ProdTimeE54
        {
            get
            {
                return getApproxProdTimeE54(order_E54);
            }
        }

        public int ProdTimeE29
        {
            get
            {
                return getApproxProdTimeE29(order_E29);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE49 + ProdTimeE54 + ProdTimeE29;
            }
        }

        public int NeedOfE13
        {
            get { return getNeedOfE13(order_E49); }
        }

        public int NeedOfE14
        {
            get { return getNeedOfE14(order_E54); }
        }

        public int NeedOfE15
        {
            get { return getNeedOfE15(order_E29); }
        }

        public int NeedOfE18
        {
            get { return getNeedOfE18(order_E49); }
        }

        public int NeedOfE19
        {
            get { return getNeedOfE19(order_E54); }
        }

        public int NeedOfE20
        {
            get { return getNeedOfE20(order_E29); }
        }

        public int NeedOfE7
        {
            get { return getNeedOfE7(order_E49); }
        }

        public int NeedOfE8
        {
            get { return getNeedOfE8(order_E54); }
        }

        public int NeedOfE9
        {
            get { return getNeedOfE9(order_E29); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK24(order_E49, order_E54, order_E29); }
        }

        public int NeedOfK25
        {
            get { return getNeedOfK25(order_E49, order_E54, order_E29); }
        }

        public int Order_E49
        {
            get
            {
                return order_E49;
            }

            set
            {
                order_E49 = value;
            }
        }

        public int Order_E54
        {
            get
            {
                return order_E54;
            }

            set
            {
                order_E54 = value;
            }
        }

        public int Order_E29
        {
            get
            {
                return order_E29;
            }

            set
            {
                order_E29 = value;
            }
        }
        #endregion

        public WP_1(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
            fillProductionOrders();
        }

        public override void fillProductionOrders()
        {
            Order_E49 = GlobalVariables.E49Produktionsauftrag;
            Order_E54 = GlobalVariables.E54Produktionsauftrag;
            Order_E29 = GlobalVariables.E54Produktionsauftrag;
        }

        #region Production E49
        public void produce_one_bath_e49()
        {
            if (order_E49 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E49 -= prod_batch;
                onMachine += prod_batch;
            }
            use_k24();
            use_k25();

            if (storage.Content[13].Quantity < prod_batch ||
                storage.Content[18].Quantity < prod_batch ||
                storage.Content[7].Quantity < prod_batch) 
                    return;

            storage.Content[13].Quantity -= (1 * prod_batch);
            storage.Content[18].Quantity -= (1 * prod_batch);
            storage.Content[7].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE49(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E54
        public void produce_one_bath_e54()
        {
            if (order_E54 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E54 -= prod_batch;
                onMachine += prod_batch;
            }
        
            use_k24();
            use_k25();

            if (storage.Content[14].Quantity<prod_batch ||
                storage.Content[19].Quantity<prod_batch ||
                storage.Content[8].Quantity<prod_batch) 
                    return;

            storage.Content[14].Quantity -= (1 * prod_batch);
            storage.Content[19].Quantity -= (1 * prod_batch);
            storage.Content[8].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE54(prod_batch);
            onMachine = 0;
        }
        #endregion
        
        #region Production E29
        public void produce_one_bath_e29()
        {
            if (order_E29 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E29 -= prod_batch;
                onMachine += prod_batch;

            }
            use_k24();
            use_k25();

            if (storage.Content[15].Quantity < prod_batch ||
                storage.Content[20].Quantity < prod_batch ||
                storage.Content[9].Quantity < prod_batch)
                return;

            storage.Content[15].Quantity -= (1 * prod_batch);
            storage.Content[20].Quantity -= (1 * prod_batch);
            storage.Content[9].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE29(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Common Use
        private bool use_k24()
        {
            if (storage.Content[24].Quantity < prod_batch)
                return false;
            storage.Content[24].Quantity -= (2 * prod_batch);
            return true;
        }

        private bool use_k25()
        {
            if (storage.Content[25].Quantity < prod_batch)
                return false;
            storage.Content[25].Quantity -= (2 * prod_batch);
            return true;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeE49(int e49Val)
        {
            return 6 * e49Val;
        }

        public int getApproxProdTimeE54(int e54Val)
        {
            return 6 * e54Val;
        }

        public int getApproxProdTimeE29(int e29Val)
        {
            return 6 * e29Val;
        }

        public int getNeedOfE13(int e13Val)
        {
            return 1 * e13Val;
        }

        public int getNeedOfE14(int e14Val)
        {
            return 1 * e14Val;
        }

        public int getNeedOfE15(int e15Val)
        {
            return 1 * e15Val;
        }

        public int getNeedOfE18(int e18Val)
        {
            return 1 * e18Val;
        }

        public int getNeedOfE19(int e19Val)
        {
            return 1 * e19Val;
        }

        public int getNeedOfE20(int e20Val)
        {
            return 1 * e20Val;
        }

        public int getNeedOfE7(int e7Val)
        {
            return 1 * e7Val;
        }

        public int getNeedOfE8(int e8Val)
        {
            return 1 * e8Val;
        }

        public int getNeedOfE9(int e9Val)
        {
            return 1 * e9Val;
        }

        public int getNeedOfK24(int e49Val, int e54Val, int e29Val)
        {
            return 2 * e49Val + 2 * e54Val + 2 * e29Val;
        }

        public int getNeedOfK25(int e49Val, int e54Val, int e29Val)
        {
            return 2 * e49Val + 2 * e54Val + 2 * e29Val;
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "\nOrder E49: " + Order_E49 
                + "\nOrder E54: " + Order_E54 
                + "\nOrder E29: " + Order_E29;
        }
    }
}
