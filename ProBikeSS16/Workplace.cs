using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using ProBikeSS16.Workplaces;

namespace ProBikeSS16
{
    abstract class Workplace
    {
        int id;
        double wage = 0, var_machineCosts, fix_machineCosts;
        protected int currentWorkTime=0, shiftsToDo;
        protected double overTimeToDo = 0;
        protected int setUptime = 0, setUps = 0;
        protected int[] workTimePerDay = new int[5];
        protected int cur_prod = 0;
        protected int onMachine = 0;
        public MissingMaterial missingMaterial;
        public WaitList waitList;
        public int curBatch;

        protected int prod_batch = 10;

        protected Storage storage = Storage.Instance;

        #region Properties
        public int Id
        {
            get
            {
                return id;
            }
        }

        public double Wage
        {
            get
            {
                return wage;
            }
        }

        public double Var_machineCosts
        {
            get
            {
                return var_machineCosts;
            }
        }

        public double Fix_machineCosts
        {
            get
            {
                return fix_machineCosts;
            }
        }

        public int CurrentWorkTime
        {
            get
            {
                return currentWorkTime;
            }
            set
            {
                currentWorkTime = value;
            }
        }

        public int ShiftsToDo
        {
            get
            {
                return shiftsToDo;
            }

            set
            {
                shiftsToDo = value < 0 ? 0 : value;
            }
        }

        public double OverTimeToDo
        {
            get
            {
                return overTimeToDo;
            }

            set
            {
                if (shiftsToDo == 3 || value < 0 || value > (Constants.WHOLE_SHIFT_TIME * Constants.MAX_OVERTIME_RATIO))
                    throw new ArgumentOutOfRangeException();
                overTimeToDo = value;
            }
        }

        public double CurrentProductionsCosts
        {
            get
            {
                double wholeCosts = 0;
                wholeCosts = currentWorkTime * (Fix_machineCosts + Var_machineCosts);
                for (int i = shiftsToDo; i > 0; i--)
                    wholeCosts += Constants.WAGES_SHIFT[i] * Constants.WHOLE_SHIFT_TIME;
                return wholeCosts += Constants.WAGE_OVERTIME * overTimeToDo;
            }
        }

        public double FreeTime
        {
            get
            {
                return Math.Abs(currentWorkTime - (shiftsToDo * Constants.WHOLE_SHIFT_TIME + overTimeToDo));
            }
        }

        public double Capacity
        {
            get
            {
                return Math.Round((currentWorkTime / (shiftsToDo * Constants.WHOLE_SHIFT_TIME + overTimeToDo)) * 100, 2);
            }
        }

        public int SetUpTime
        {
            get
            {
                return setUptime;
            }
        }

        public int[] WorkTimePerDay
        {
            get
            {
                return workTimePerDay;
            }
        }
        #endregion

        protected Workplace(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo=1, double overTimeToDo=0)
        {
            #region Guardians
            if (id > Constants.MAX_WORKPLACES)
                throw new ArgumentOutOfRangeException("Higher Workplace ID than available");
            
            if (var_machineCosts < 0 && var_machineCosts > 2)
                throw new ArgumentOutOfRangeException();

            if (fix_machineCosts < 0 && fix_machineCosts > 2)
                throw new ArgumentOutOfRangeException();
            #endregion

            this.id = id;
            this.var_machineCosts = var_machineCosts;
            this.fix_machineCosts = fix_machineCosts;
            ShiftsToDo = shiftsToDo;
            OverTimeToDo = overTimeToDo;

            Debug.WriteLine("Created Workplace: " + id);
        }

        public abstract void fillProductionOrders();

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();

            s.Append("WP ");
            s.AppendLine(id.ToString());
            s.Append("Wage ");
            s.AppendLine(wage.ToString());
            s.Append("Current Worktime ");
            s.AppendLine(currentWorkTime.ToString());
            s.Append("Variable Machine Costs ");
            s.AppendLine(var_machineCosts.ToString());
            s.Append("Fix Machine Costs ");
            s.AppendLine(fix_machineCosts.ToString());
            s.Append("Shifts to Do ");
            s.AppendLine(shiftsToDo.ToString());
            s.Append("Overtime to Do ");
            s.AppendLine(overTimeToDo.ToString());
            s.Append("Production Batch ");
            s.AppendLine(prod_batch.ToString());

            return s.ToString();
        }

        internal double getApproxWageCosts()
        {
            double sum = this.currentWorkTime;
            double ret = 0;

            if (currentWorkTime > 7200)
                return 0;
            if (sum > 6000 && sum <= 7200 && shiftsToDo == 3)
            {
                Console.WriteLine("Sum S3Ü b = " + sum);
                ret += (sum - 4800) * 0.7;
                sum = sum - (sum - 4800);
                Console.WriteLine("Sum S3 = " + sum);
            }
            if (sum > 4800 && sum <= 6000 && shiftsToDo == 2)
            {
                Console.WriteLine("Sum S2Ü b = " + sum);
                ret += (sum - 4800) * 0.9;
                sum = sum - (sum - 4800);
                Console.WriteLine("Sum S2Ü = " + sum);
            }
            if (sum > 3600 && sum <= 4800 && shiftsToDo == 2)
            {
                Console.WriteLine("Sum S2 b = " + sum);
                ret += (sum - 2400) * 0.55;
                sum = sum - (sum - 2400);
                Console.WriteLine("Sum S2 = " + sum);
            }
            if (sum > 2400 && sum <= 3600 && shiftsToDo == 1)
            {
                Console.WriteLine("Sum S1Ü b = " + sum);
                ret += (sum - 2400) * 0.9;
                sum = sum - (sum - 2400);
                Console.WriteLine("Sum S1Ü a = " + sum);
            }
            if (sum > 0 && sum <= 2400 && shiftsToDo == 1)
            {
                Console.WriteLine("Sum S1 b = " + sum);
                ret += sum * 0.45;
                sum -= sum;
                Console.WriteLine("Sum S1 = " + sum);
            }

            return Math.Round(ret, 2);   
        }

        internal double getApproxMaachineCosts()
        {
            double costs = fix_machineCosts + var_machineCosts;

            return Math.Round(currentWorkTime * costs, 2);
        }
    }
}
