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
        uint id;
        double wage=0, var_machineCosts, fix_machineCosts;
        uint currentWorkTime=0, shiftsToDo;
        double overTimeToDo;

        #region Properties
        public uint Id
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

        public uint CurrentWorkTime
        {
            get
            {
                return currentWorkTime;
            }
        }

        public uint ShiftsToDo
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
        #endregion
        
        public Workplace(uint id, double var_machineCosts, double fix_machineCosts, uint shiftsToDo=1, double overTimeToDo=0)
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
    }
}
