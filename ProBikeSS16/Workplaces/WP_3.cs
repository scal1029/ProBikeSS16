namespace ProBikeSS16.Workplaces
{
    class WP_3 : Workplace
    {

        int order_E51 = 0;
        int order_E56 = 0;
        int order_E31 = 0;

        #region Getter/Setter
        public int ProdTimeE51
        {
            get
            {
                return getApproxProdTimeE51(order_E51);
            }
        }

        public int ProdTimeE56
        {
            get
            {
                return getApproxProdTimeE56(order_E56);
            }
        }

        public int ProdTimeE31
        {
            get
            {
                return getApproxProdTimeE31(order_E31);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE51 + ProdTimeE56 + ProdTimeE31;
            }
        }

        public int NeedOfE50
        {
            get { return getNeedOfE50(order_E51); }
        }

        public int NeedOfE55
        {
            get { return getNeedOfE55(order_E56); }
        }

        public int NeedOfE30
        {
            get { return getNeedOfE30(order_E31); }
        }

        public int NeedOfE16
        {
            get { return getNeedOfE16(order_E51, order_E56, order_E31); }
        }

        public int NeedOfE17
        {
            get { return getNeedOfE17(order_E51, order_E56, order_E31); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK24(order_E51, order_E56, order_E31); }
        }

        public int NeedOfK27
        {
            get { return getNeedOfK27(order_E51, order_E56, order_E31); }
        }

        public int Order_E51
        {
            get
            {
                return order_E51;
            }

            set
            {
                order_E51 = value;
            }
        }

        public int Order_E56
        {
            get
            {
                return order_E56;
            }

            set
            {
                order_E56 = value;
            }
        }

        public int Order_E31
        {
            get
            {
                return order_E31;
            }

            set
            {
                order_E31 = value;
            }
        }
        #endregion

        public WP_3(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production E51
        public void produce_one_bath_e51()
        {
            if (order_E51 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E51 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e16();
            use_e17();
            use_k24();
            use_k27();

            if (storage.Content[50].Quantity < prod_batch)
                return;

            storage.Content[50].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE51(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E56
        public void produce_one_bath_e56()
        {
            if (order_E56 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                currentWorkTime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E56 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e16();
            use_e17();
            use_k24();
            use_k27();

            if (storage.Content[55].Quantity < prod_batch)
                return;

            storage.Content[55].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE56(prod_batch);
            onMachine = 0;
        }
        #endregion
        
        #region Production E31
        public void produce_one_bath_e31()
        {
            if (order_E31 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E31 -= prod_batch;
                onMachine += prod_batch;
            }

            use_e16();
            use_e17();
            use_k24();
            use_k27();

            if (storage.Content[30].Quantity < prod_batch)
                return;

            storage.Content[30].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE31(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Common Use
        private bool use_e16()
        {
            if (storage.Content[16].Quantity < prod_batch)
                return false;
            storage.Content[16].Quantity -= (1 * prod_batch);
            return true;
        }

        private bool use_e17()
        {
            if (storage.Content[17].Quantity < prod_batch)
                return false;
            storage.Content[17].Quantity -= (1 * prod_batch);
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
        public int getApproxProdTimeE51(int e51Val)
        {
            return 5 * e51Val;
        }

        public int getApproxProdTimeE56(int e56Val)
        {
            return 6 * e56Val;
        }

        public int getApproxProdTimeE31(int e31Val)
        {
            return 6 * e31Val;
        }

        public int getNeedOfE50(int e51Val)
        {
            return 1 * e51Val;
        }

        public int getNeedOfE55(int e56Val)
        {
            return 1 * e56Val;
        }

        public int getNeedOfE30(int e31Val)
        {
            return 1 * e31Val;
        }

        public int getNeedOfE16(int e51Val, int e56Val, int e31Val)
        {
            return 1 * e51Val + 1 * e56Val + 1 * e31Val;
        }

        public int getNeedOfE17(int e51Val, int e56Val, int e31Val)
        {
            return 1 * e51Val + 1 * e56Val + 1 * e31Val;
        }

        public int getNeedOfK24(int e51Val, int e56Val, int e31Val)
        {
            return 1 * e51Val + 1 * e56Val + 1 * e31Val;
        }

        public int getNeedOfK27(int e51Val, int e56Val, int e31Val)
        {
            return 1 * e51Val + 1 * e56Val + 1 * e31Val;
        }
        #endregion
    }
}
