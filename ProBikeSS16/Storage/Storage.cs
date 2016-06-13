using System;
using System.Collections.Generic;
using System.Data;

namespace ProBikeSS16
{
    sealed class Storage
    {
        static readonly Storage instance = new Storage();

        Dictionary<int, PEK> content = new Dictionary<int, PEK>();

        private Storage()
        {
            initStorageContent();
        }

        internal Dictionary<int, PEK> Content
        {
            get
            {
                return content;
            }
        }

        internal static Storage Instance
        {
            get
            {
                return instance;
            }
        }

        private void initStorageContent()
        {
            for (int i = 1; i <= 3; i++)
                content[i] = new Product((uint)i);
            initFabricates();
            initPurchaseParts();
        }

        internal void fillData(DataSet data)
        {
            foreach (DataRow row in data.Tables[2].Rows)
            {
                int id = Convert.ToInt32((string)row["id"]);
                int a = Convert.ToInt32((string)row["amount"]);
                double p = Convert.ToDouble((string)row["price"]);
                double sv = Convert.ToDouble((string)row["stockvalue"]);
                content[id].Quantity = a;
                content[id].Price = p;
                content[id].StockValue = sv;
            }
        }

        private void initFabricates()
        {
            for (int i = 4; i <= 20; i++)
                content[i] = new Fabricate((uint)i);

            content[26] = new Fabricate(26);
            content[29] = new Fabricate(29);
            content[30] = new Fabricate(30);
            content[31] = new Fabricate(31);
            content[49] = new Fabricate(49);
            content[50] = new Fabricate(50);
            content[51] = new Fabricate(51);
            content[54] = new Fabricate(54);
            content[55] = new Fabricate(55);
            content[56] = new Fabricate(56);

        }

        private void initPurchaseParts()
        {
            for (int i = 21; i <= 25; i++)
                content[i] = new PurchasePart((uint)i);

            content[27] = new PurchasePart(27);
            content[28] = new PurchasePart(28);

            for (int i = 32; i <= 48; i++)
                content[i] = new PurchasePart((uint)i);

            content[52] = new PurchasePart(52);
            content[53] = new PurchasePart(53);
            content[57] = new PurchasePart(57);
            content[58] = new PurchasePart(58);
            content[59] = new PurchasePart(59);
        }
    }
}
