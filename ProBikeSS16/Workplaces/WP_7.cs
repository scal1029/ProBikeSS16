namespace ProBikeSS16.Workplaces
{
    class WP_7 : Workplace
    {
        int order_d9_1_p1 = 0;
        int order_d9_2_p1 = 0;
        int order_d9_3_p1 = 0;
        int order_d9_1_p2 = 0;
        int order_d9_2_p2 = 0;
        int order_d9_3_p2 = 0;
        int order_d9_1_p3 = 0;
        int order_d9_2_p3 = 0;
        int order_d9_3_p3 = 0;
        int order_d15_p1 = 0;
        int order_d15_p2 = 0;
        int order_d15_p3 = 0;

        #region Getter/Setter
        public int ProdTimeD9_1_p1
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_1_p1);
            }
        }

        public int ProdTimeD9_2_p1
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_2_p1);
            }
        }

        public int ProdTimeD9_3_p1
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_3_p1);
            }
        }

        public int ProdTimeD9_1_p2
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_1_p2);
            }
        }

        public int ProdTimeD9_2_p2
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_2_p2);
            }
        }

        public int ProdTimeD9_3_p2
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_3_p2);
            }
        }

        public int ProdTimeD9_1_p3
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_1_p3);
            }
        }

        public int ProdTimeD9_2_p3
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_2_p3);
            }
        }

        public int ProdTimeD9_3_p3
        {
            get
            {
                return getApproxProdTimeOd9(order_d9_3_p3);
            }
        }
        public int ProdTimeD15_p1
        {
            get
            {
                return getApproxProdTimeOd9(order_d15_p1);
            }
        }

        public int ProdTimeD15_p2
        {
            get
            {
                return getApproxProdTimeOd9(order_d15_p2);
            }
        }

        public int ProdTimeD15_p3
        {
            get
            {
                return getApproxProdTimeOd9(order_d15_p3);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeD9_1_p1 + ProdTimeD9_2_p1 + ProdTimeD9_3_p1
                    + ProdTimeD9_1_p2 + ProdTimeD9_2_p2 + ProdTimeD9_3_p2
                    + ProdTimeD9_1_p3 + ProdTimeD9_2_p3 + ProdTimeD9_3_p3
                    + ProdTimeD15_p1 + ProdTimeD15_p2 + ProdTimeD15_p3;
            }
        }

        public int NeedOfD8_1_p1
        {
            get
            {
                return getNeedOfD8(order_d9_1_p1);
            }
        }

        public int NeedOfD8_2_p1
        {
            get
            {
                return getNeedOfD8(order_d9_2_p1);
            }
        }

        public int NeedOfD8_3_p1
        {
            get
            {
                return getNeedOfD8(order_d9_3_p1);
            }
        }

        public int NeedOfD8_1_p2
        {
            get
            {
                return getNeedOfD8(order_d9_1_p2);
            }
        }

        public int NeedOfD8_2_p2
        {
            get
            {
                return getNeedOfD8(order_d9_2_p2);
            }
        }

        public int NeedOfD8_3_p2
        {
            get
            {
                return getNeedOfD8(order_d9_3_p2);
            }
        }

        public int NeedOfD8_1_p3
        {
            get
            {
                return getNeedOfD8(order_d9_1_p3);
            }
        }

        public int NeedOfD8_2_p3
        {
            get
            {
                return getNeedOfD8(order_d9_2_p3);
            }
        }

        public int NeedOfD8_3_p3
        {
            get
            {
                return getNeedOfD8(order_d9_3_p3);
            }
        }

        public int NeedOfK44
        {
            get { return getNeedOfK44(order_d15_p1)
                    + getNeedOfK44(order_d15_p2)
                    + getNeedOfK44(order_d15_p3); }
        }

        public int NeedOfK48
        {
            get
            {
                return getNeedOfK44(order_d15_p1)
                  + getNeedOfK44(order_d15_p2)
                  + getNeedOfK44(order_d15_p3);
            }
        }

        public int NeedOfK59
        {
            get
            {
                return getNeedOfK59(order_d9_2_p1)
                  + getNeedOfK59(order_d9_2_p2)
                  + getNeedOfK59(order_d9_2_p3);
            }
        }

        public int Order_d9_1_p1
        {
            get
            {
                return order_d9_1_p1;
            }

            set
            {
                order_d9_1_p1 = value;
            }
        }

        public int Order_d9_2_p1
        {
            get
            {
                return order_d9_2_p1;
            }

            set
            {
                order_d9_2_p1 = value;
            }
        }

        public int Order_d9_3_p1
        {
            get
            {
                return order_d9_3_p1;
            }

            set
            {
                order_d9_3_p1 = value;
            }
        }

        public int Order_d9_1_p2
        {
            get
            {
                return order_d9_1_p2;
            }

            set
            {
                order_d9_1_p2 = value;
            }
        }

        public int Order_d9_2_p2
        {
            get
            {
                return order_d9_2_p2;
            }

            set
            {
                order_d9_2_p2 = value;
            }
        }

        public int Order_d9_3_p2
        {
            get
            {
                return order_d9_3_p2;
            }

            set
            {
                order_d9_3_p2 = value;
            }
        }

        public int Order_d9_1_p3
        {
            get
            {
                return order_d9_1_p3;
            }

            set
            {
                order_d9_1_p3 = value;
            }
        }

        public int Order_d9_2_p3
        {
            get
            {
                return order_d9_2_p3;
            }

            set
            {
                order_d9_2_p3 = value;
            }
        }

        public int Order_d9_3_p3
        {
            get
            {
                return order_d9_3_p3;
            }

            set
            {
                order_d9_3_p3 = value;
            }
        }

        public int Order_d15_p1
        {
            get
            {
                return order_d15_p1;
            }

            set
            {
                order_d15_p1 = value;
            }
        }

        public int Order_d15_p2
        {
            get
            {
                return order_d15_p2;
            }

            set
            {
                order_d15_p2 = value;
            }
        }

        public int Order_d15_p3
        {
            get
            {
                return order_d15_p3;
            }

            set
            {
                order_d15_p3 = value;
            }
        }
        #endregion

        public WP_7(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }
        
        #region Production d9_1_p1
        public void produce_one_batch_d9_1_p1()
        {
            if (order_d9_1_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_1_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch) 
                    return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_2_p1
        public void produce_one_batch_d9_2_p1()
        {
            if (order_d9_2_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_2_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[59].Quantity < prod_batch)
                return;

            storage.Content[59].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_3_p1
        public void produce_one_batch_d9_3_p1()
        {
            if (order_d9_3_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_3_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_1_p2
        public void produce_one_batch_d9_1_p2()
        {
            if (order_d9_1_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_1_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_2_p2
        public void produce_one_batch_d9_2_p2()
        {
            if (order_d9_2_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_2_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[59].Quantity < prod_batch)
                return;

            storage.Content[59].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_3_p2
        public void produce_one_batch_d9_3_p2()
        {
            if (order_d9_3_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_3_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_1_p3
        public void produce_one_batch_d9_1_p3()
        {
            if (order_d9_1_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 7)
            {
                cur_prod = 7;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_1_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_2_p3
        public void produce_one_batch_d9_2_p3()
        {
            if (order_d9_2_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 8)
            {
                cur_prod = 8;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_2_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[59].Quantity < prod_batch)
                return;

            storage.Content[59].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d9_3_p3
        public void produce_one_batch_d9_3_p3()
        {
            if (order_d9_3_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 9)
            {
                cur_prod = 9;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d9_3_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d15_p1
        public void produce_one_batch_d15_p1()
        {
            if (order_d15_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 10)
            {
                cur_prod = 10;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d15_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[44].Quantity < prod_batch ||
                storage.Content[48].Quantity < prod_batch)
                return;

            storage.Content[44].Quantity -= (2 * prod_batch);
            storage.Content[48].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d15_p2
        public void produce_one_batch_d15_p2()
        {
            if (order_d15_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 11)
            {
                cur_prod = 11;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d15_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            if (storage.Content[44].Quantity < prod_batch ||
                storage.Content[48].Quantity < prod_batch)
                return;

            storage.Content[44].Quantity -= (2 * prod_batch);
            storage.Content[48].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d15_p3
        public void produce_one_batch_d15_p3()
        {
            if (order_d15_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 12)
            {
                cur_prod = 12;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d15_p3 -= prod_batch;
                onMachine += prod_batch;
            }
            
            if (storage.Content[44].Quantity < prod_batch ||
                storage.Content[48].Quantity < prod_batch)
                return;

            storage.Content[44].Quantity -= (2 * prod_batch);
            storage.Content[48].Quantity -= (2 * prod_batch);

            currentWorkTime += getApproxProdTimeOd9(prod_batch);
            onMachine = 0;
        }
        #endregion


        #region Methods
        public int getApproxProdTimeOd9(int val)
        {
            return 2 * val;
        }

        public int getNeedOfD8(int val)
        {
            return 1 * val;
        }

        public int getNeedOfK44(int k44Val)
        {
            return 2 * k44Val;
        }

        public int getNeedOfK48(int k48Val)
        {
            return 2 * k48Val;
        }

        public int getNeedOfK59(int k59Val)
        {
            return 2 * k59Val;
        }
        #endregion
    }
}
