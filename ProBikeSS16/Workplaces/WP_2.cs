namespace ProBikeSS16.Workplaces
{
    class WP_2 : Workplace
    {

        int order_E50 = 0;
        int order_E55 = 0;
        int order_E30 = 0;

        #region Getter/Setter
        public int ProdTimeE50
        {
            get
            {
                return getApproxProdTimeE50(order_E50);
            }
        }

        public int ProdTimeE55
        {
            get
            {
                return getApproxProdTimeE55(order_E55);
            }
        }

        public int ProdTimeE30
        {
            get
            {
                return getApproxProdTimeE30(order_E30);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE50 + ProdTimeE55 + ProdTimeE30;
            }
        }

        public int NeedOfE49
        {
            get { return getNeedOfE49(order_E50); }
        }

        public int NeedOfE54
        {
            get { return getNeedOfE54(order_E55); }
        }

        public int NeedOfE29
        {
            get { return getNeedOfE29(order_E30); }
        }

        public int NeedOfE10
        {
            get { return getNeedOfE10(order_E50); }
        }

        public int NeedOfE11
        {
            get { return getNeedOfE11(order_E55); }
        }

        public int NeedOfE12
        {
            get { return getNeedOfE12(order_E30); }
        }

        public int NeedOfE4
        {
            get { return getNeedOfE4(order_E50); }
        }

        public int NeedOfE5
        {
            get { return getNeedOfE5(order_E55); }
        }

        public int NeedOfE6
        {
            get { return getNeedOfE6(order_E30); }
        }

        public int NeedOfK24
        {
            get { return getNeedOfK24(order_E50, order_E55, order_E30); }
        }

        public int NeedOfK25
        {
            get { return getNeedOfK25(order_E50, order_E55, order_E30); }
        }

        public int Order_E50
        {
            get
            {
                return order_E50;
            }

            set
            {
                order_E50 = value;
            }
        }

        public int Order_E55
        {
            get
            {
                return order_E55;
            }

            set
            {
                order_E55 = value;
            }
        }

        public int Order_E30
        {
            get
            {
                return order_E30;
            }

            set
            {
                order_E30 = value;
            }
        }
        #endregion

        public WP_2(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production E50
        public void produce_one_bath_e50()
        {
            if (order_E50 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 30;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E50 -= prod_batch;
                onMachine += prod_batch;
            }
            use_k24();
            use_k25();

            if (storage.Content[49].Quantity < prod_batch ||
                storage.Content[10].Quantity < prod_batch ||
                storage.Content[4].Quantity < prod_batch) 
                    return;

            storage.Content[49].Quantity -= (1 * prod_batch);
            storage.Content[10].Quantity -= (1 * prod_batch);
            storage.Content[4].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE50(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E55
        public void produce_one_bath_e55()
        {
            if (order_E55 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 30;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E55 -= prod_batch;
                onMachine += prod_batch;
            }
        
            use_k24();
            use_k25();

            if (storage.Content[54].Quantity<prod_batch ||
                storage.Content[11].Quantity<prod_batch ||
                storage.Content[5].Quantity<prod_batch) 
                    return;

            storage.Content[54].Quantity -= (1 * prod_batch);
            storage.Content[11].Quantity -= (1 * prod_batch);
            storage.Content[5].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE55(prod_batch);
            onMachine = 0;
        }
        #endregion
        
        #region Production E30
        public void produce_one_bath_e30()
        {
            if (order_E30 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_E30 -= prod_batch;
                onMachine += prod_batch;

            }
            use_k24();
            use_k25();

            if (storage.Content[29].Quantity < prod_batch ||
                storage.Content[12].Quantity < prod_batch ||
                storage.Content[6].Quantity < prod_batch)
                return;

            storage.Content[29].Quantity -= (1 * prod_batch);
            storage.Content[12].Quantity -= (1 * prod_batch);
            storage.Content[6].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeE30(prod_batch);
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
        public int getApproxProdTimeE50(int e50Val)
        {
            return 5 * e50Val;
        }

        public int getApproxProdTimeE55(int e55Val)
        {
            return 5 * e55Val;
        }

        public int getApproxProdTimeE30(int e30Val)
        {
            return 5 * e30Val;
        }

        public int getNeedOfE49(int e49Val)
        {
            return 1 * e49Val;
        }

        public int getNeedOfE54(int e54Val)
        {
            return 1 * e54Val;
        }

        public int getNeedOfE29(int e29Val)
        {
            return 1 * e29Val;
        }

        public int getNeedOfE10(int e10Val)
        {
            return 1 * e10Val;
        }

        public int getNeedOfE11(int e11Val)
        {
            return 1 * e11Val;
        }

        public int getNeedOfE12(int e12Val)
        {
            return 1 * e12Val;
        }

        public int getNeedOfE4(int e4Val)
        {
            return 1 * e4Val;
        }

        public int getNeedOfE5(int e5Val)
        {
            return 1 * e5Val;
        }

        public int getNeedOfE6(int e6Val)
        {
            return 1 * e6Val;
        }

        public int getNeedOfK24(int e50Val, int e55Val, int e30Val)
        {
            return 2 * e50Val + 2 * e55Val + 2 * e30Val;
        }

        public int getNeedOfK25(int e50Val, int e55Val, int e30Val)
        {
            return 2 * e50Val + 2 * e55Val + 2 * e30Val;
        }
        #endregion
    }
}
