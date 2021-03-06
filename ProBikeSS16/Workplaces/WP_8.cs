﻿using System.Text;

namespace ProBikeSS16.Workplaces
{
    class WP_8 : Workplace
    {
        static int order_d7_1_p1 = 0;
        static int order_d7_2_p1 = 0;
        static int order_d7_3_p1 = 0;
        static int order_d7_1_p2 = 0;
        static int order_d7_2_p2 = 0;
        static int order_d7_3_p2 = 0;
        static int order_d7_1_p3 = 0;
        static int order_d7_2_p3 = 0;
        static int order_d7_3_p3 = 0;

        #region Getter/Setter
        public int ProdTimeD7_1_p1
        {
            get
            {
                    return getApproxProdTimeOd7_1_p1(order_d7_1_p1);
            }
        }

        public int ProdTimeD7_2_p1
        {
            get
            {
                return getApproxProdTimeOd7_2_p1(order_d7_2_p1);
            }
        }

        public int ProdTimeD7_3_p1
        {
            get
            {
                return getApproxProdTimeOd7_3_p1(order_d7_3_p1);
            }
        }

        public int ProdTimeD7_1_p2
        {
            get
            {
                return getApproxProdTimeOd7_1_p2(order_d7_1_p2);
            }
        }

        public int ProdTimeD7_2_p2
        {
            get
            {
                return getApproxProdTimeOd7_2_p2(order_d7_2_p2);
            }
        }

        public int ProdTimeD7_3_p2
        {
            get
            {
                return getApproxProdTimeOd7_3_p2(order_d7_3_p2);
            }
        }

        public int ProdTimeD7_1_p3
        {
            get
            {
                return getApproxProdTimeOd7_1_p3(order_d7_1_p3);
            }
        }

        public int ProdTimeD7_2_p3
        {
            get
            {
                return getApproxProdTimeOd7_2_p3(order_d7_2_p3);
            }
        }

        public int ProdTimeD7_3_p3
        {
            get
            {
                return getApproxProdTimeOd7_3_p3(order_d7_3_p3);
            }
        }

        public int ProdTime
        {
            get
            {
                return ProdTimeD7_1_p1 + ProdTimeD7_2_p1 + ProdTimeD7_3_p1
                    + ProdTimeD7_1_p2 + ProdTimeD7_2_p2 + ProdTimeD7_3_p2
                    + ProdTimeD7_1_p3 + ProdTimeD7_2_p3 + ProdTimeD7_3_p3;
            }
        }

        public int NeedOfD12_1_p1
        {
            get
            {
                return NeedOfD12(order_d7_1_p1);
            }
        }

        public int NeedOfD12_2_p1
        {
            get
            {
                return NeedOfD12(order_d7_2_p1);
            }
        }

        public int NeedOfD6_p1
        {
            get
            {
                return NeedOfD6(order_d7_3_p1);
            }
        }

        public int NeedOfD12_1_p2
        {
            get
            {
                return NeedOfD12(order_d7_1_p2);
            }
        }

        public int NeedOfD12_2_p2
        {
            get
            {
                return NeedOfD12(order_d7_2_p2);
            }
        }

        public int NeedOfD6_p2
        {
            get
            {
                return NeedOfD6(order_d7_3_p2);
            }
        }

        public int NeedOfD12_1_p3
        {
            get
            {
                return NeedOfD12(order_d7_1_p3);
            }
        }

        public int NeedOfD12_2_p3
        {
            get
            {
                return NeedOfD12(order_d7_2_p3);
            }
        }

        public int NeedOfD6_p3
        {
            get
            {
                return NeedOfD6(order_d7_3_p3);
            }
        }

        public static int Order_d7_1_p1
        {
            get
            {
                return order_d7_1_p1;
            }

            set
            {
                order_d7_1_p1 = value;
            }
        }

        public static int Order_d7_2_p1
        {
            get
            {
                return order_d7_2_p1;
            }

            set
            {
                order_d7_2_p1 = value;
            }
        }

        public static int Order_d7_3_p1
        {
            get
            {
                return order_d7_3_p1;
            }

            set
            {
                order_d7_3_p1 = value;
            }
        }

        public static int Order_d7_1_p2
        {
            get
            {
                return order_d7_1_p2;
            }

            set
            {
                order_d7_1_p2 = value;
            }
        }

        public static int Order_d7_2_p2
        {
            get
            {
                return order_d7_2_p2;
            }

            set
            {
                order_d7_2_p2 = value;
            }
        }

        public static int Order_d7_3_p2
        {
            get
            {
                return order_d7_3_p2;
            }

            set
            {
                order_d7_3_p2 = value;
            }
        }

        public static int Order_d7_1_p3
        {
            get
            {
                return order_d7_1_p3;
            }

            set
            {
                order_d7_1_p3 = value;
            }
        }

        public static int Order_d7_2_p3
        {
            get
            {
                return order_d7_2_p3;
            }

            set
            {
                order_d7_2_p3 = value;
            }
        }

        public static int Order_d7_3_p3
        {
            get
            {
                return order_d7_3_p3;
            }

            set
            {
                order_d7_3_p3 = value;
            }
        }
        #endregion

        public WP_8(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo = 1, double overTimeToDo = 0) 
            : base(id, var_machineCosts, fix_machineCosts, shiftsToDo, overTimeToDo)
        {

        }

        public override void fillProductionOrders() { }

        #region Production d7_1_p1
        public void produce_one_batch_d7_1_p1()
        {
            if (order_d7_1_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 1)
            {
                cur_prod = 1;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_1_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < (3 * prod_batch)) 
                    return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_1_p1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_2_p1
        public void produce_one_batch_d7_2_p1()
        {
            if (order_d7_2_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 2)
            {
                cur_prod = 2;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_2_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < (3 * prod_batch))
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_2_p1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_3_p1
        public void produce_one_batch_d7_3_p1()
        {
            if (order_d7_3_p1 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 3)
            {
                cur_prod = 3;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_3_p1 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_3_p1(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_1_p2
        public void produce_one_batch_d7_1_p2()
        {
            if (order_d7_1_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 4)
            {
                cur_prod = 4;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_1_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_1_p2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_2_p2
        public void produce_one_batch_d7_2_p2()
        {
            if (order_d7_2_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 5)
            {
                cur_prod = 5;
                setUptime += 25;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_2_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_2_p2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_3_p2
        public void produce_one_batch_d7_3_p2()
        {
            if (order_d7_3_p2 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 6)
            {
                cur_prod = 6;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_3_p2 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_3_p2(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_1_p3
        public void produce_one_batch_d7_1_p3()
        {
            if (order_d7_1_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 7)
            {
                cur_prod = 7;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_1_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < (3 * prod_batch))
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_1_p3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_2_p3
        public void produce_one_batch_d7_2_p3()
        {
            if (order_d7_2_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 8)
            {
                cur_prod = 8;
                setUptime += 20;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_2_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < (3 * prod_batch))
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_2_p3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Production d7_3_p3
        public void produce_one_batch_d7_3_p3()
        {
            if (order_d7_3_p3 <= 0 && onMachine == 0)
                return;

            if (cur_prod != 9)
            {
                cur_prod = 9;
                setUptime += 15;
                setUps++;
            }

            if (onMachine == 0)
            {
                order_d7_3_p3 -= prod_batch;
                onMachine += prod_batch;
            }

            /** TODO DIRECTS **/
            if (storage.Content[28].Quantity < prod_batch)
                return;

            storage.Content[28].Quantity -= (3 * prod_batch);

            currentWorkTime += getApproxProdTimeOd7_3_p3(prod_batch);
            onMachine = 0;
        }
        #endregion

        #region Methods
        public int getApproxProdTimeOd7_1_p1(int d7Val_p1)
        {
            return 1 * d7Val_p1;
        }

        public int getApproxProdTimeOd7_2_p1(int d7Val_p1)
        {
            return 3 * d7Val_p1;
        }

        public int getApproxProdTimeOd7_3_p1(int d7Val_p1)
        {
            return 1 * d7Val_p1;
        }

        public int getApproxProdTimeOd7_1_p2(int d7Val_p2)
        {
            return 2 * d7Val_p2;
        }

        public int getApproxProdTimeOd7_2_p2(int d7Val_p2)
        {
            return 3 * d7Val_p2;
        }

        public int getApproxProdTimeOd7_3_p2(int d7Val_p2)
        {
            return 2 * d7Val_p2;
        }

        public int getApproxProdTimeOd7_1_p3(int d7Val_p3)
        {
            return 2 * d7Val_p3;
        }

        public int getApproxProdTimeOd7_2_p3(int d7Val_p3)
        {
            return 3 * d7Val_p3;
        }

        public int getApproxProdTimeOd7_3_p3(int d7Val_p3)
        {
            return 2 * d7Val_p3;
        }

        public int NeedOfD12(int d7Val)
        {
            return 1 * d7Val;
        }

        public int NeedOfD6(int d7Val)
        {
            return 1 * d7Val;
        }
        #endregion

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.Append(base.ToString());
            s.AppendLine("\nOrder d7_1_p1: " + Order_d7_1_p1);
            s.AppendLine("\nOrder d7_2_p1: " + Order_d7_2_p1);
            s.AppendLine("\nOrder d7_3_p1: " + Order_d7_3_p1);
            s.AppendLine("\nOrder d7_1_p2: " + Order_d7_1_p2);
            s.AppendLine("\nOrder d7_2_p2: " + Order_d7_2_p2);
            s.AppendLine("\nOrder d7_3_p2: " + Order_d7_3_p2);
            s.AppendLine("\nOrder d7_1_p3: " + Order_d7_1_p3);
            s.AppendLine("\nOrder d7_2_p3: " + Order_d7_2_p3);
            s.AppendLine("\nOrder d7_3_p3: " + Order_d7_3_p3);

            return s.ToString(); ;
        }
    }
}
