using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16.domain
{
    class Global
    {
        /* Fertigteile anlegen
        public static Unit e04 = new Unit(4, type.s);
        public static Unit e05 = new Unit(5, type.s);
        public static Unit e06 = new Unit(6, type.s);
        public static Unit e07 = new Unit(7, type.s);
        public static Unit e08 = new Unit(8, type.s);
        public static Unit e09 = new Unit(9, type.s);
        public static Unit e10 = new Unit(10, type.s);
        public static Unit e11 = new Unit(11, type.s);
        public static Unit e12 = new Unit(12, type.s);
        public static Unit e13 = new Unit(13, type.s);
        public static Unit e14 = new Unit(14, type.s);
        public static Unit e15 = new Unit(15, type.s);
        public static Unit e16 = new Unit(16, type.s);
        public static Unit e17 = new Unit(17, type.s);
        public static Unit e18 = new Unit(18, type.s);
        public static Unit e19 = new Unit(19, type.s);
        public static Unit e20 = new Unit(20, type.s);
        public static Unit e26 = new Unit(26, type.s);
        public static Unit e29 = new Unit(29, type.s);
        public static Unit e30 = new Unit(30, type.s);
        public static Unit e31 = new Unit(31, type.s);
        public static Unit e49 = new Unit(49, type.s);
        public static Unit e50 = new Unit(50, type.s);
        public static Unit e51 = new Unit(51, type.s);
        public static Unit e54 = new Unit(54, type.s);
        public static Unit e55 = new Unit(55, type.s);
        public static Unit e56 = new Unit(56, type.s);
        */

        // Kaufeile anlegen
        public static Unit k21 = new Unit(21, type.b);
        public static Unit k22 = new Unit(21, type.b);
        public static Unit k23 = new Unit(21, type.b);
        public static Unit k24 = new Unit(21, type.b);
        public static Unit k25 = new Unit(21, type.b);
        public static Unit k27 = new Unit(21, type.b);
        public static Unit k28 = new Unit(21, type.b);
        public static Unit k32 = new Unit(21, type.b);
        public static Unit k33 = new Unit(21, type.b);
        public static Unit k34 = new Unit(21, type.b);
        public static Unit k35 = new Unit(21, type.b);
        public static Unit k36 = new Unit(21, type.b);
        public static Unit k37 = new Unit(21, type.b);
        public static Unit k38 = new Unit(21, type.b);
        public static Unit k39 = new Unit(21, type.b);
        public static Unit k40 = new Unit(21, type.b);
        public static Unit k41 = new Unit(21, type.b);
        public static Unit k42 = new Unit(21, type.b);
        public static Unit k43 = new Unit(21, type.b);
        public static Unit k44 = new Unit(21, type.b);
        public static Unit k45 = new Unit(21, type.b);
        public static Unit k46 = new Unit(21, type.b);
        public static Unit k47 = new Unit(21, type.b);
        public static Unit k48 = new Unit(21, type.b);
        public static Unit k52 = new Unit(21, type.b);
        public static Unit k53 = new Unit(21, type.b);
        public static Unit k57 = new Unit(21, type.b);
        public static Unit k58 = new Unit(21, type.b);
        public static Unit k59 = new Unit(21, type.b);

        public static Unit e18 = new Unit(18, type.s, le18);
        public static List<Unit> le18 = new List<Unit> { k59, k59, k32, k28, k28, k28 };
        public static List<Unit> e13 = new List<Unit> { k39, k32 };
        public static List<Unit> e07 = new List<Unit> { k53, k53, k53, k53, k53, k53,
                                                        k53, k53, k53, k53, k53, k53,
                                                        k53, k53, k53, k53, k53, k53,
                                                        k53, k53, k53, k53, k53, k53,
                                                        k53, k53, k53, k53, k53, k53,
                                                        k53, k53, k53, k53, k53, k53,
                                                        k52, k38, k37, k35, k35};

        public static List<Unit> le49 = new List<Unit> { e18 };
        public static Unit e49 = new Unit(49, type.s, le18);


        //addlist(36, k53, ref e07);


            //public List<Unit> addlist(int number, Unit t, ref List<Unit> l)
            //{
            //    for(int i = 0; i < number; i++)
            //    {
            //        l.Add(t);
            //    }
            //    return l;
            //}

        /// <summary>
        /// Summery bikes
        /// </summary>
        public static List<Unit> ChildBike = new List<Unit> { };
        public static List<Unit> FemaleBike = new List<Unit> { };
        public static List<Unit> MaleBike = new List<Unit> { };

    }
}
