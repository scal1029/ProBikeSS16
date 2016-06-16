namespace ProBikeSS16.Workplaces
{
    class WP_10 : Workplace
    {
        static int order_d11_1_p1;
        static int order_d11_2_p1;
        static int order_d11_1_p2;
        static int order_d11_2_p2;
        static int order_d11_1_p3;
        static int order_d11_2_p3;

        #region Getter/Setter
        public int ProdTimed11_1_p1
        {
            get
            {
                return getApproxProdTimed11(order_d11_1_p1);
            }
        }

        public int ProdTimed11_2_p1
        {
            get
            {
                return getApproxProdTimed11(order_d11_2_p1);
            }
        }

        public int ProdTimed11_1_p2
        {
            get
            {
                return getApproxProdTimed11(order_d11_1_p2);
            }
        }

        public int ProdTimed11_2_p2
        {
            get
            {
                return getApproxProdTimed11(order_d11_2_p2);
            }
        }

        public int ProdTimed11_1_p3
        {
            get
            {
                return getApproxProdTimed11(order_d11_1_p3);
            }
        }

        public int ProdTimed11_2_p3
        {
            get
            {
                return getApproxProdTimed11(order_d11_2_p3);
            }
        }
        public int ProdTime
        {
            get
            {
                return ProdTimed11_1_p1 + ProdTimed11_2_p1 + ProdTimed11_1_p2
                    + ProdTimed11_2_p2 + ProdTimed11_1_p3 + ProdTimed11_2_p3;
            }
        }

        public int NeedOfK52
        {
            get
            {
                return getNeedOfK525733(order_d11_1_p1 + order_d11_2_p1);
            }
        }

        public int NeedOfK57
        {
            get
            {
                return getNeedOfK525733(order_d11_1_p2 + order_d11_2_p2);
            }
        }

        public int NeedOfK33
        {
            get
            {
                return getNeedOfK525733(order_d11_1_p3 + order_d11_2_p3);
            }
        }

        public int NeedOfK53
        {
            get
            {
                return getNeedOfK535834(order_d11_1_p1 + order_d11_2_p1);
            }
        }

        public int NeedOfK58
        {
            get
            {
                return getNeedOfK535834(order_d11_1_p2 + order_d11_2_p2);
            }
        }

        public int NeedOfK34
        {
            get
            {
                return getNeedOfK535834(order_d11_1_p3 + order_d11_2_p3);
            }
        }
        #endregion

        public WP_10(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production D11 1 P1
        public void produce_one_batch_d11_1_p1()
        {
            if (order_d11_1_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_1_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[52].Quantity < prod_batch) 
                    return;

            storage.Content[52].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production D11 2 P1
        public void produce_one_batch_d11_2_p1()
        {
            if (order_d11_2_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_2_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[53].Quantity < (36 * prod_batch))
                return;

            storage.Content[53].Quantity -= (36 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production D11 1 P2
        public void produce_one_batch_d11_1_p2()
        {
            if (order_d11_1_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_1_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[57].Quantity < prod_batch)
                return;

            storage.Content[57].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production D11 2 P2
        public void produce_one_batch_d11_2_p2()
        {
            if (order_d11_2_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_2_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[58].Quantity < (36 * prod_batch))
                return;

            storage.Content[58].Quantity -= (36 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production D11 1 P3
        public void produce_one_batch_d11_1_p3()
        {
            if (order_d11_1_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_1_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[33].Quantity < prod_batch)
                return;

            storage.Content[33].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production D11 2 P3
        public void produce_one_batch_d11_2_p3()
        {
            if (order_d11_2_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_d11_2_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[34].Quantity < (36 * prod_batch))
                return;

            storage.Content[34].Quantity -= (36 * prod_batch);

            currentWorkTime += getApproxProdTimed11(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTimed11(int d11)
        {
            return 4 * d11;
        }

        public int getNeedOfK525733(int k525733)
        {
            return 1 * k525733;
        }

        public int getNeedOfK535834(int k535834)
        {
            return 36 * k535834;
        }
        #endregion

        public override string ToString()
        {
            return base.ToString() + "\nOrder D11_1_p1: " + order_d11_1_p1
                + "\nOrder D11_2_p1: " + order_d11_2_p1
                + "\nOrder D11_1_p2: " + order_d11_1_p2
                + "\nOrder D11_2_p2: " + order_d11_2_p2
                + "\nOrder D11_1_p3: " + order_d11_1_p3
                + "\nOrder D11_2_p3: " + order_d11_2_p3;
        }
    }
}
