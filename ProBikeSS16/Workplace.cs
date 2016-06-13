using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ProBikeSS16
{
    class Workplace
    {
        int id;
        double wage = 0, var_machineCosts, fix_machineCosts;
        protected int currentWorkTime=0, shiftsToDo;
        protected double overTimeToDo;

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
        }

        public int ShiftsToDo
        {
            get
            {
                return shiftsToDo;
            }

            set
            {

                if (value < 1 || value > 3)
                    throw new ArgumentOutOfRangeException();
                shiftsToDo = value;
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
        #endregion

        public Workplace(int id, double var_machineCosts, double fix_machineCosts, int shiftsToDo=1, double overTimeToDo=0)
        {
            #region Guardians
            if (id >= Constants.MAX_WORKPLACES)
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
    }
}
