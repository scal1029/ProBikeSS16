namespace ProBikeSS16.Workplaces
{
    class WP_11 : Workplace
    {

        int order_e4 = 0;
        int order_e5 = 0;
        int order_e6 = 0;
        int order_e7 = 0;
        int order_e8 = 0;
        int order_e9 = 0;
        int cur_prod = 0;
        int onMachine = 0;

        #region Getter/Setter
        public int ProdTimeE4
        {
            get
            {
                return getApproxProdTimeE4(order_e4);
            }
        }

        public int ProdTimeE5
        {
            get
            {
                return getApproxProdTimeE5(order_e5);
            }
        }

        public int ProdTimeE6
        {
            get
            {
                return getApproxProdTimeE6(order_e6);
            }
        }

        public int ProdTimeE7
        {
            get
            {
                return getApproxProdTimeE7(order_e7);
            }
        }

        public int ProdTimeE8
        {
            get
            {
                return getApproxProdTimeE8(order_e8);
            }
        }

        public int ProdTimeE9
        {
            get
            {
                return getApproxProdTimeE9(order_e9);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE4 + ProdTimeE5 + ProdTimeE6 +
                    ProdTimeE7 + ProdTimeE8 + ProdTimeE9;
            }
        }

        public int NeedOfDirectFrom10
        {
            get { return getNeedOfDirectFrom10(order_e4 + order_e5 + order_e6 
                + order_e7 + order_e8 + order_e9); }
        }

        public int NeedOfK35
        {
            get { return getNeedOfK35(order_e4, order_e5, order_e6, order_e7, order_e8, order_e9); }
        }

        public int NeedofK36
        {
            get { return getNeedOfK36(order_e4, order_e5, order_e6); }
        }

        public int NeedofK37
        {
            get { return getNeedOfK37(order_e7, order_e8, order_e9); }
        }

        public int NeedofK38
        {
            get { return getNeedOfK38(order_e7, order_e8, order_e9); }
        }

        public int Order_e4
        {
            get
            {
                return order_e4;
            }

            set
            {
                order_e4 = value;
            }
        }

        public int Order_e5
        {
            get
            {
                return order_e5;
            }

            set
            {
                order_e5 = value;
            }
        }

        public int Order_e6
        {
            get
            {
                return order_e6;
            }

            set
            {
                order_e6 = value;
            }
        }

        public int Order_e7
        {
            get
            {
                return order_e7;
            }

            set
            {
                order_e7 = value;
            }
        }

        public int Order_e8
        {
            get
            {
                return order_e8;
            }

            set
            {
                order_e8 = value;
            }
        }

        public int Order_e9
        {
            get
            {
                return order_e9;
            }

            set
            {
                order_e9 = value;
            }
        }
        #endregion

        public WP_11(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {
        }
        
        #region Production E4
        public void produce_one_bath_E4()
        {
            if (order_e4 <= 0 && onMachine == 0)
                return;

            if(cur_prod != 1)
            {
                cur_prod = 1;
                currentWorkTime += 10;
            }
            
            if (onMachine == 0)
            {
                order_e4 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[36].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[36].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Production E5
        public void produce_one_bath_E5()
        {
            if (order_e5 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                currentWorkTime += 10;
            }

            if (onMachine == 0)
            {
                order_e5 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[36].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[36].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Production E6
        public void produce_one_bath_E6()
        {
            if (order_e6 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                currentWorkTime += 10;
            }

            if (onMachine == 0)
            {
                order_e6 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[36].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[36].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Production E7
        public void produce_one_bath_E7()
        {
            if (order_e7 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                currentWorkTime += 20;
            }

            if (onMachine == 0)
            {
                order_e7 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[37].Quantity < prod_batch ||
                storage.Content[38].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[37].Quantity -= (1 * prod_batch);
            storage.Content[38].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Production E8
        public void produce_one_bath_E8()
        {
            if (order_e8 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                currentWorkTime += 20;
            }

            if (onMachine == 0)
            {
                order_e8 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[37].Quantity < prod_batch ||
                storage.Content[38].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[37].Quantity -= (1 * prod_batch);
            storage.Content[38].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Production E9
        public void produce_one_bath_E9()
        {
            if (order_e9 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                currentWorkTime += 20;
            }

            if (onMachine == 0)
            {
                order_e9 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[35].Quantity < prod_batch ||
                storage.Content[37].Quantity < prod_batch ||
                storage.Content[38].Quantity < prod_batch)
                return;

            storage.Content[35].Quantity -= (2 * prod_batch);
            storage.Content[37].Quantity -= (1 * prod_batch);
            storage.Content[38].Quantity -= (1 * prod_batch);

            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeE4(int e4Val)
        {
            return 3 * e4Val;
        }

        public int getApproxProdTimeE5(int e5Val)
        {
            return 3 * e5Val;
        }

        public int getApproxProdTimeE6(int e6Val)
        {
            return 3 * e6Val;
        }

        public int getApproxProdTimeE7(int e7Val)
        {
            return 3 * e7Val;
        }

        public int getApproxProdTimeE8(int e8Val)
        {
            return 3 * e8Val;
        }

        public int getApproxProdTimeE9(int e9Val)
        {
            return 3 * e9Val;
        }

        public int getNeedOfDirectFrom10(int d10Val)
        {
            return 1 * d10Val;
        }

        public int getNeedOfK35(int e4Val, int e5Val, int e6Val, 
            int e7Val, int e8Val, int e9Val)
        {
            return 2 * (e4Val + e5Val + e6Val + e7Val + e8Val + e9Val);
        }

        public int getNeedOfK36(int e4Val, int e5Val, int e6Val)
        {
            return 1 * (e4Val + e5Val + e6Val);
        }

        public int getNeedOfK37(int e7Val, int e8Val, int e9Val)
        {
            return 1 * (e7Val + e8Val + e9Val);
        }

        public int getNeedOfK38(int e7Val, int e8Val, int e9Val)
        {
            return 1 * (e7Val + e8Val + e9Val);
        }
        #endregion
    }
}
