namespace ProBikeSS16.Workplaces
{
    class WP_9 : Workplace
    {
        int order_e10 = 0;
        int order_e11 = 0;
        int order_e12 = 0;
        int order_e13 = 0;
        int order_e14 = 0;
        int order_e15 = 0;
        int order_e18 = 0;
        int order_e19 = 0;
        int order_e20 = 0;

        #region Getter/Setter
        public int ProdTimeE10
        {
            get
            {
                return getApproxProdTime3(order_e10);
            }
        }

        public int ProdTimeE11
        {
            get
            {
                return getApproxProdTime3(order_e11);
            }
        }

        public int ProdTimeE12
        {
            get
            {
                return getApproxProdTime3(order_e12);
            }
        }

        public int ProdTimeE13
        {
            get
            {
                return getApproxProdTime3(order_e13);
            }
        }

        public int ProdTimeE14
        {
            get
            {
                return getApproxProdTime3(order_e14);
            }
        }

        public int ProdTimeE15
        {
            get
            {
                return getApproxProdTime3(order_e15);
            }
        }

        public int ProdTimeE18
        {
            get
            {
                return getApproxProdTime2(order_e18);
            }
        }

        public int ProdTimeE19
        {
            get
            {
                return getApproxProdTime2(order_e19);
            }
        }

        public int ProdTimeE20
        {
            get
            {
                return getApproxProdTime2(order_e20);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeE10 + ProdTimeE11 + ProdTimeE12 
                    + ProdTimeE13 + ProdTimeE14 + ProdTimeE15
                    + ProdTimeE18 + ProdTimeE19 + ProdTimeE20;
            }
        }

        public int NeedOfD7_1_p1
        {
            get
            {
                return getNeedOfD7(order_e10);
            }
        }

        public int NeedOfD7_2_p1
        {
            get
            {
                return getNeedOfD7(order_e13);
            }
        }

        public int NeedOfD7_3_p1
        {
            get
            {
                return getNeedOfD7(order_e18);
            }
        }

        public int NeedOfD7_1_p2
        {
            get
            {
                return getNeedOfD7(order_e11);
            }
        }

        public int NeedOfD7_2_p2
        {
            get
            {
                return getNeedOfD7(order_e14);
            }
        }

        public int NeedOfD7_3_p2
        {
            get
            {
                return getNeedOfD7(order_e19);
            }
        }

        public int NeedOfD7_1_p3
        {
            get
            {
                return getNeedOfD7(order_e13);
            }
        }

        public int NeedOfD7_2_p3
        {
            get
            {
                return getNeedOfD7(order_e15);
            }
        }

        public int NeedOfD7_3_p3
        {
            get
            {
                return getNeedOfD7(order_e20);
            }
        }

        public int NeedOfK32
        {
            get
            {
                return getNeedOfK32(order_e10) + getNeedOfK32(order_e11) + getNeedOfK32(order_e12)
                    + getNeedOfK32(order_e13) + getNeedOfK32(order_e14) + getNeedOfK32(order_e15)
                    + getNeedOfK32(order_e18) + getNeedOfK32(order_e19) + getNeedOfK32(order_e20);
            }
        }
        #endregion

        public WP_9(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production E10
        public void produce_one_batch_e10()
        {
            if (order_e10 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_e10 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch) 
                    return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E11
        public void produce_one_batch_e11()
        {
            if (order_e11 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e11 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E12
        public void produce_one_batch_e12()
        {
            if (order_e12 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e12 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E13
        public void produce_one_batch_e13()
        {
            if (order_e13 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e13 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E14
        public void produce_one_batch_e14()
        {
            if (order_e14 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e14 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E15
        public void produce_one_batch_e15()
        {
            if (order_e15 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e15 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E18
        public void produce_one_batch_e18()
        {
            if (order_e18 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 7)
            {
                cur_prod = 7;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e18 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E19
        public void produce_one_batch_e19()
        {
            if (order_e19 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 8)
            {
                cur_prod = 8;
                setUptime += 20;
            }

            if (onMachine == 0)
            {
                order_e19 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production E20
        public void produce_one_batch_e20()
        {
            if (order_e20 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 9)
            {
                cur_prod = 9;
                setUptime += 15;
            }

            if (onMachine == 0)
            {
                order_e20 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[32].Quantity < prod_batch)
                return;

            storage.Content[32].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTime2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTime3(int e1015)
        {
            return 3 * e1015;
        }

        public int getApproxProdTime2(int e1820)
        {
            return 2 * e1820;
        }

        public int getNeedOfD7(int d7Val)
        {
            return 1 * d7Val;
        }

        public int getNeedOfK32(int k32Val)
        {
            return 1 * k32Val;
        }
        #endregion
    }
}
