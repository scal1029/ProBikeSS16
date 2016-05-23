using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    sealed class Storage
    {
        static readonly Storage instance = new Storage();

        HashSet<PEK> content = new HashSet<PEK>();

        private Storage()
        {
            initStorageContent();
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
            for (uint i = 1; i <= 3; i++)
                content.Add(new Product(i));
            initFabricates();
            initPurchaseParts();
        }

        private void initFabricates()
        {
            for (uint i = 4; i <= 20; i++)
                content.Add(new Fabricate(i));

            content.Add(new Fabricate(26));
            content.Add(new Fabricate(29));
            content.Add(new Fabricate(30));
            content.Add(new Fabricate(31));
            content.Add(new Fabricate(49));
            content.Add(new Fabricate(50));
            content.Add(new Fabricate(51));
            content.Add(new Fabricate(54));
            content.Add(new Fabricate(55));
            content.Add(new Fabricate(56));

        }

        private void initPurchaseParts()
        {
            for (uint i = 21; i <= 25; i++)
                content.Add(new PurchasePart(i));

            content.Add(new PurchasePart(27));
            content.Add(new PurchasePart(28));

            for (uint i = 32; i <= 48; i++)
                content.Add(new PurchasePart(i));

            content.Add(new PurchasePart(52));
            content.Add(new PurchasePart(53));
            content.Add(new PurchasePart(57));
            content.Add(new PurchasePart(58));
            content.Add(new PurchasePart(59));
        }
    }
}
