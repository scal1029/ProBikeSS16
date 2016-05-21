using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace ProBikeSS16
{
    public static class DataTableStuff
    {
        public static DataSet ReadXMLtoDataSet(string filepath)
        {

            DataSet ds = new DataSet();
            ds.ReadXml(GlobalVariables.InputXML.CreateReader());

            #region Crap
            //Console.WriteLine(ds.Tables[1].Rows[0].ToString());

            ////Stock
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("Table2: Stock");
            //Console.WriteLine();
            ////MessageBox.Show(ds.Tables[2].Columns.Count.ToString());
            //foreach (DataRow row in ds.Tables[2].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[2].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " what? ");
            //    }
            //}

            ////three
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("Table3 Nothing");
            //Console.WriteLine();
            //foreach (DataRow row in ds.Tables[3].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[3].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " ");
            //    }
            //}

            ////FutureInwardStockMovement
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("InwardStockMovement+FutureInwardStockMovement");
            //Console.WriteLine();
            //foreach (DataRow row in ds.Tables[4].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[4].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " ");
            //    }
            //}

            ////five
            //Console.WriteLine();
            //Console.WriteLine();
            //Console.WriteLine("Table5: Nothing");
            //Console.WriteLine();
            //foreach (DataRow row in ds.Tables[5].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[5].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " ");
            //    }
            //}
            #endregion Crap

            return ds;
        }

        public static void Programmplannung()
        {
            
        }
    }
}
