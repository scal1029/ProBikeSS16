using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

    public static class GlobalVariables
    {
        public static int? StockChildBike;
        public static int? StockMaleBike;
        public static int? StockFemaleBike;
        public static int? SaleChildBikeN;
        public static int? SaleChildBikeN1;
        public static int? SaleChildBikeN2;
        public static int? SaleChildBikeN3;
        public static int? SaleFemaleBikeN;
        public static int? SaleFemaleBikeN1;
        public static int? SaleFemaleBikeN2;
        public static int? SaleFemaleBikeN3;
        public static int? SaleMaleBikeN;
        public static int? SaleMaleBikeN1;
        public static int? SaleMaleBikeN2;
        public static int? SaleMaleBikeN3;

        public static XDocument InputXML = new XDocument();

        public static bool ForecastCorrect;
        public static bool XMLCorrect;

        public static DataSet InputDataSetWithoutOldBatchCalc;
    }
}
