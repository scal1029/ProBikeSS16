﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        public static readonly double[] VARIABLE_MACHINE_COSTS  = { 0, 0.05, 0.05, 0.05, 0.05, 0, 0.3, 0.3, 0.3, 0.8, 0.3, 0.3, 0.3, 0.5, 0.05, 0.05 };
        public static readonly double[] FIX_MACHINE_COSTS       = { 0, 0.01, 0.01, 0.01, 0.01, 0, 0.1, 0.1, 0.1, 0.25, 0.1, 0.1, 0.1, 0.15, 0.01, 0.01 };

        public const uint MAX_WORKPLACES                        = 15;
        public enum WORKPLACES                                  { A1=1, A2, A3, A4, A5, A6, A7, A8, A9, A10, A11, A12, A13, A14, A15 };
    }

    public static class GlobalVariables
    {
        public static int curPeriod = 0;

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

        //Fabrik for Simulation
        public static Factory factory = Factory.Instance;

        public static DataTable Kapaza;

        //Blocking Errors for Exporting XML
        public static bool blockingErrorKapaPlan;

        //SetUpLastPeriod
        public static int A1SetUpsLastPeriod = 0;
        public static int A2SetUpsLastPeriod = 0;
        public static int A3SetUpsLastPeriod = 0;
        public static int A4SetUpsLastPeriod = 0;
        public static int A6SetUpsLastPeriod = 0;
        public static int A7SetUpsLastPeriod = 0;
        public static int A8SetUpsLastPeriod = 0;
        public static int A9SetUpsLastPeriod = 0;
        public static int A10SetUpsLastPeriod = 0;
        public static int A11SetUpsLastPeriod = 0;
        public static int A12SetUpsLastPeriod = 0;
        public static int A13SetUpsLastPeriod = 0;
        public static int A14SetUpsLastPeriod = 0;
        public static int A15SetUpsLastPeriod = 0;

        //Produktionsaufträge
        public static int E26Produktionsauftrag;
        public static int P1E26Produktionsauftrag;
        public static int P2E26Produktionsauftrag;
        public static int P3E26Produktionsauftrag;
        public static int E16Produktionsauftrag;
        public static int P1E16Produktionsauftrag;
        public static int P2E16Produktionsauftrag;
        public static int P3E16Produktionsauftrag;
        public static int E17Produktionsauftrag;
        public static int P1E17Produktionsauftrag;
        public static int P2E17Produktionsauftrag;
        public static int P3E17Produktionsauftrag;

        public static int P1Produktionsauftrag;
        public static int E51Produktionsauftrag;
        public static int E50Produktionsauftrag;
        public static int E4Produktionsauftrag;
        public static int E10Produktionsauftrag;
        public static int E49Produktionsauftrag;
        public static int E7Produktionsauftrag;
        public static int E13Produktionsauftrag;
        public static int E18Produktionsauftrag;

        public static int P2Produktionsauftrag;
        public static int E56Produktionsauftrag;
        public static int E55Produktionsauftrag;
        public static int E5Produktionsauftrag;
        public static int E11Produktionsauftrag;
        public static int E54Produktionsauftrag;
        public static int E8Produktionsauftrag;
        public static int E14Produktionsauftrag;
        public static int E19Produktionsauftrag;


        public static int P3Produktionsauftrag;
        public static int E31Produktionsauftrag;
        public static int E30Produktionsauftrag;
        public static int E6Produktionsauftrag;
        public static int E12Produktionsauftrag;
        public static int E29Produktionsauftrag;
        public static int E9Produktionsauftrag;
        public static int E15Produktionsauftrag;
        public static int E20Produktionsauftrag;

        public static DataTable dtProdOrder = new DataTable();
        public static DataTable Aussortierung = new DataTable();
        public static DataTable Bestellungsspeicher = new DataTable();

        public static DataTable Prognose = new DataTable();

        public static List<OrderPrototyp> ProduktionsAufträgeAktuellePeriode = new List<OrderPrototyp>();

        public static List<TeilPrototyp> AlleTeile = new List<TeilPrototyp>();

        public static Dictionary<int, ArbeitsstationPrototyp> ZusammenTeilundProduktionskette =
            new Dictionary<int, ArbeitsstationPrototyp>();

        public static  Dictionary<int ,Arbeitsplatzprototyp> AlleArbeitsplätze = new Dictionary<int, Arbeitsplatzprototyp>();

        public static List<string> Wochentage = new List<string>
        {
            "Montag",
            "Dienstag",
            "Mittwoch",
            "Donnerstag",
            "Freitag"
        };

        public static Dictionary<int, int> Lagerstand = new Dictionary<int, int>();

        //SimuStartDaten(Stationen, Arbeitsplätze, Produktionsaufträge, Lagerstände)

        public static Dictionary<int, Arbeitsplatzprototyp> OriginalAlleArbeitsplätze = new Dictionary<int, Arbeitsplatzprototyp>();
        public static List<OrderPrototyp> OriginalProduktionsAufträgeAktuellePeriode = new List<OrderPrototyp>();
        public static Dictionary<int, int> OriginalLagerstand = new Dictionary<int, int>();

        //Ausgabe
        public static DataTable KPErg = new DataTable();
        internal static int groupNumber;
        internal static int capN1Period;
        internal static int capN2Period;
        internal static int capN3Period;
    }
}
