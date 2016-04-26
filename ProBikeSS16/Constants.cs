using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    public static class Constants
    {
        public const uint Week_Days                             = 5;
        public const uint TIME_SHIFT_DAY                        = 480;                              //In Minutes
        public const uint WHOLE_SHIFT_TIME                      = Week_Days * TIME_SHIFT_DAY;       //In Minutes
        public const double MAX_OVERTIME_RATIO                  = 0.5;                              //Percentage Ratio

        public const uint STORAGE_THRESHOLD                     = 250000;                           //In Euro
        public const int ADDITIONAL_STORAGE_COSTS               = 5000;                             //In Euro
        public const double STOCKHOLDING_COSTS                  = 0.6;                              //Per Week

        //Euro per Minute
        public static readonly double[] WAGES_SHIFT             = { 0, 0.45, 0.55, 0.70 };
        public const double WAGE_OVERTIME                       = 0.90;
        public static readonly double[] VARIABLE_MACHINE_COSTS  = { 0.05, 0.05, 0.05, 0.05, 0, 0.3, 0.3, 0.3, 0.8, 0.3, 0.3, 0.3, 0.5 };
        public static readonly double[] FIX_MACHINE_COSTS       = { 0.01, 0.01, 0.01, 0.01, 0, 0.1, 0.1, 0.1, 0.25, 0.1, 0.1, 0.1, 0.15 };

        public const uint MAX_WORKPLACES                        = 15;
        public enum WORKPLACES                                  { A1=0, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15 };
    }
}
