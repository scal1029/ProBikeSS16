using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    public static class DataTableStuff
    {
        public static DataSet ReadXMLtoDataSet(string filepath)
        {
            //DataSet test = new DataSet();
            //StringReader sr = new StringReader(filepath);
            //test.ReadXml(sr);
            //Console.WriteLine(test.Tables[1].Rows[0]);
            //return test.Tables[0];

            DataSet ds = new DataSet();
            ds.ReadXml(GlobalVariables.InputXML.CreateReader());
            Console.WriteLine(ds.Tables[1].Rows[0].ToString());

            //foreach (DataRow row in ds.Tables[1].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[1].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " ");
            //    }
            //}

            foreach (DataRow row in ds.Tables[2].Rows)
            {
                Console.WriteLine();
                for (int x = 0; x < ds.Tables[2].Columns.Count; x++)
                {
                    Console.Write(row[x].ToString() + " ");
                }
            }

            //foreach (DataRow row in ds.Tables[3].Rows)
            //{
            //    Console.WriteLine();
            //    for (int x = 0; x < ds.Tables[3].Columns.Count; x++)
            //    {
            //        Console.Write(row[x].ToString() + " ");
            //    }
            //}

            return ds;
        }


    }
}
