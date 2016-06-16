namespace ProBikeSS16.Workplaces
{
    class WP_4 : Workplace
    {

        static int order_p1 = 0;
        static int order_p2 = 0;
        static int order_p3 = 0;

        #region Getter/Setter
        public int ProdTimeP1
        {
            get
            {
                return getApproxProdTimeP1(order_p1);
            }
        }

        public int ProdTimeP2
        {
            get
            {
                return getApproxProdTimeP2(order_p2);
            }
        }

        public int ProdTimeP3
        {
            get
            {
                return getApproxProdTimeP3(order_p3);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeP1 + ProdTimeP2 + ProdTimeP3;
            }
        }

        public int NeedOfE51
        {
            get { return getNeedOfE51(order_p1); }
        }

        public int NeedOfE56
        {
            get { return getNeedOfE56(order_p2); }
        }

        public int NeedOfE31
        {
            get { return getNeedOfE31(order_p3); }
        }

        public int NeedOfE21
        {
            get { return getNeedOfK21(order_p1); }
        }

        public int NeedOfK22
        {
            get { return getNeedOfK22(order_p2); }
        }

        public int NeedOfK23
        {
            get { return getNeedOfK23(order_p3); }
        }

        public int NeedOfE26
        {
            get { return getNeedOfE26(order_p1, order_p2, order_p3); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK24(order_p1, order_p2, order_p3); }
        }

        public int NeedOfK27
        {
            get { return getNeedOfK27(order_p1, order_p2, order_p3); }
        }

        public int Order_p1
        {
            get
            {
                return order_p1;
            }

            set
            {
                order_p1 = value;
            }
        }

        public int Order_p2
        {
            get
            {
                return order_p2;
            }

            set
            {
                order_p2 = value;
            }
        }

        public int Order_p3
        {
            get
            {
                return order_p3;
            }

            set
            {
                order_p3 = value;
            }
        }
        #endregion

        public WP_4(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
            fillProductionOrders();
        }

        public override void fillProductionOrders()
        {
            Order_p1 = GlobalVariables.P1Produktionsauftrag;
            Order_p2 = GlobalVariables.P2Produktionsauftrag;
            Order_p3 = GlobalVariables.P3Produktionsauftrag;
        }

        #region Production P1
        public void produce_one_bath_p1()
        {
            if (order_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 30;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e26();
            use_k24();
            use_k27();

            if (storage.Content[51].Quantity < prod_batch ||
                storage.Content[21].Quantity < prod_batch)
                return;

            storage.Content[51].Quantity -= (1 * prod_batch);
            storage.Content[21].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeP1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production P2
        public void produce_one_bath_p2()
        {
            if (order_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 30;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e26();
            use_k24();
            use_k27();

            if (storage.Content[56].Quantity < prod_batch ||
                storage.Content[22].Quantity < prod_batch)
                return;

            storage.Content[56].Quantity -= (1 * prod_batch);
            storage.Content[22].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeP2(prod_batch);
            onMachine = 0;
        }
        #endregion
        
        #region Production P3
        public void produce_one_bath_p3()
        {
            if (order_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 30;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e26();
            use_k24();
            use_k27();

            if (storage.Content[31].Quantity < prod_batch ||
                storage.Content[23].Quantity < prod_batch)
                return;

            storage.Content[31].Quantity -= (1 * prod_batch);
            storage.Content[23].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeP3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Common Use
        private bool use_e26()
        {
            if (storage.Content[26].Quantity < prod_batch)
                return false;
            storage.Content[26].Quantity -= (1 * prod_batch);
            return true;
        }

        private bool use_k24()
        {
            if (storage.Content[24].Quantity < prod_batch)
                return false;
            storage.Content[24].Quantity -= (1 * prod_batch);
            return true;
        }

        private bool use_k27()
        {
            if (storage.Content[27].Quantity < prod_batch)
                return false;
            storage.Content[27].Quantity -= (1 * prod_batch);
            return true;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeP1(int p1Val)
        {
            return 6 * p1Val;
        }

        public int getApproxProdTimeP2(int p2Val)
        {
            return 7 * p2Val;
        }

        public int getApproxProdTimeP3(int p3Val)
        {
            return 7 * p3Val;
        }

        public int getNeedOfE51(int p1Val)
        {
            return 1 * p1Val;
        }

        public int getNeedOfE56(int p2Val)
        {
            return 1 * p2Val;
        }

        public int getNeedOfE31(int p3Val)
        {
            return 1 * p3Val;
        }

        public int getNeedOfK21(int p1Val)
        {
            return 1 * p1Val;
        }

        public int getNeedOfK22(int p2Val)
        {
            return 1 * p2Val;
        }

        public int getNeedOfK23(int p3Val)
        {
            return 1 * p3Val;
        }

        public int getNeedOfE26(int p1Val, int p2Val, int p3Val)
        {
            return 1 * p1Val + 1 * p2Val + 1 * p3Val;
        }

        public int getNeedOfK24(int p1Val, int p2Val, int p3Val)
        {
            return 1 * p1Val + 1 * p2Val + 1 * p3Val;
        }

        public int getNeedOfK27(int p1Val, int p2Val, int p3Val)
        {
            return 1 * p1Val + 1 * p2Val + 1 * p3Val;
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "\nOrder P1: " + Order_p1
                + "\nOrder P2: " + Order_p2
                + "\nOrder P3: " + Order_p3;
        }
    }
}
