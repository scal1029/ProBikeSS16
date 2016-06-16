namespace ProBikeSS16.Workplaces
{
    class WP_6 : Workplace
    {
        int order_d8_p1 = 0;
        int order_d14_p1 = 0;
        int order_d8_p2 = 0;
        int order_d14_p2 = 0;
        int order_d8_p3 = 0;
        int order_d14_p3 = 0;

        #region Getter/Setter
        public int ProdTimeD8_p1
        {
            get
            {
                return getApproxProdTimeOd8_p1(order_d8_p1);
            }
        }

        public int ProdTimeD14_p1
        {
            get
            {
                return getApproxProdTimeOd14_p1(order_d14_p1);
            }
        }

        public int ProdTimeD8_p2
        {
            get
            {
                return getApproxProdTimeOd8_p2(order_d8_p2);
            }
        }

        public int ProdTimeD14_p2
        {
            get
            {
                return getApproxProdTimeOd14_p2(order_d14_p2);
            }
        }
        public int ProdTimeD8_p3
        {
            get
            {
                return getApproxProdTimeOd8_p3(order_d8_p3);
            }
        }

        public int ProdTimeD14_p3
        {
            get
            {
                return getApproxProdTimeOd14_p3(order_d14_p3);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeD8_p1 + ProdTimeD14_p1 + ProdTimeD8_p2 + ProdTimeD14_p2
                    + ProdTimeD8_p3 + ProdTimeD14_p3;
            }
        }

        public int NeedOfK28_d8_p1
        {
            get { return getNeedOfK28_d8_p1(order_d8_p1); }
        }

        public int NeedOfK28_d14_p1
        {
            get
            {
                return getNeedOfK28_d14_p1(order_d8_p1);
            }
        }

        public int NeedOfK28_d8_p2
        {
            get { return getNeedOfK28_d8_p2(order_d8_p2); }
        }

        public int NeedOfK28_d14_p2
        {
            get
            {
                return getNeedOfK28_d14_p2(order_d8_p2);
            }
        }

        public int NeedOfK28_d8_p3
        {
            get { return getNeedOfK28_d8_p3(order_d8_p3); }
        }

        public int NeedOfK28_d14_p3
        {
            get
            {
                return getNeedOfK28_d14_p3(order_d8_p3);
            }
        }

        public int NeedOfK28
        {
            get {
                return NeedOfK28_d8_p1 + NeedOfK28_d8_p2 + NeedOfK28_d8_p3
                  + NeedOfK28_d14_p1 + NeedOfK28_d14_p2 + NeedOfK28_d14_p3; }
        }

        public int Order_d8_p1
        {
            get
            {
                return order_d8_p1;
            }

            set
            {
                order_d8_p1 = value;
            }
        }

        public int Order_d14_p1
        {
            get
            {
                return order_d14_p1;
            }

            set
            {
                order_d14_p1 = value;
            }
        }

        public int Order_d8_p2
        {
            get
            {
                return order_d8_p2;
            }

            set
            {
                order_d8_p2 = value;
            }
        }

        public int Order_d14_p2
        {
            get
            {
                return order_d14_p2;
            }

            set
            {
                order_d14_p2 = value;
            }
        }

        public int Order_d8_p3
        {
            get
            {
                return order_d8_p3;
            }

            set
            {
                order_d8_p3 = value;
            }
        }

        public int Order_d14_p3
        {
            get
            {
                return order_d14_p3;
            }

            set
            {
                order_d14_p3 = value;
            }
        }
        #endregion

        public WP_6(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production d8_p1
        public void produce_one_batch_d8_p1()
        {
            if (order_d8_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d8_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch) 
                    return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd8_p1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d14_p1
        public void produce_one_batch_d14_p1()
        {
            if (order_d14_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d14_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeOd14_p1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d8_p2
        public void produce_one_batch_d8_p2()
        {
            if (order_d8_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                currentWorkTime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d8_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (4 * prod_batch);

            currentWorkTime += getApproxProdTimeOd8_p2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d14_p2
        public void produce_one_batch_d14_p2()
        {
            if (order_d14_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d14_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeOd14_p2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d8_p3
        public void produce_one_batch_d8_p3()
        {
            if (order_d8_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                currentWorkTime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d8_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (5 * prod_batch);

            currentWorkTime += getApproxProdTimeOd8_p3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d14_p3
        public void produce_one_batch_d14_p3()
        {
            if (order_d14_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d14_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (1 * prod_batch);

            currentWorkTime += getApproxProdTimeOd14_p3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeOd8_p1(int d8Val_p1)
        {
            return 3 * d8Val_p1;
        }

        public int getApproxProdTimeOd14_p1(int d14Val_p1)
        {
            return 2 * d14Val_p1;
        }

        public int getApproxProdTimeOd8_p2(int d8Val_p2)
        {
            return 3 * d8Val_p2;
        }

        public int getApproxProdTimeOd14_p2(int d14Val_p2)
        {
            return 2 * d14Val_p2;
        }

        public int getApproxProdTimeOd8_p3(int d8Val_p3)
        {
            return 3 * d8Val_p3;
        }

        public int getApproxProdTimeOd14_p3(int d14Val_p3)
        {
            return 2 * d14Val_p3;
        }

        public int getNeedOfK28_d8_p1(int d8Val_p1)
        {
            return 3 * d8Val_p1;
        }

        public int getNeedOfK28_d14_p1(int d14Val_p1)
        {
            return 1 * d14Val_p1;
        }

        public int getNeedOfK28_d8_p2(int d8Val_p2)
        {
            return 4 * d8Val_p2;
        }

        public int getNeedOfK28_d14_p2(int d14Val_p2)
        {
            return 1 * d14Val_p2;
        }

        public int getNeedOfK28_d8_p3(int d8Val_p3)
        {
            return 5 * d8Val_p3;
        }

        public int getNeedOfK28_d14_p3(int d14Val_p3)
        {
            return 1 * d14Val_p3;
        }
        #endregion
    }
}
