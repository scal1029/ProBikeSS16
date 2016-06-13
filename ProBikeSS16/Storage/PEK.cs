using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProBikeSS16
{
    abstract class PEK
    {
        protected char prefix;
        int quantity = 0;
        uint id;
        double price = 0;
        double stockvalue = 0;


        public char Prefix
        {
            get
            {
                return prefix;
            }
        }

        public int Quantity
        {
            get
            {
                return quantity;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                quantity = value;
            }
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public double Price
        {
            get
            {
                return price;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                price = value;
            }
        }

        public double StockValue
        {
            get
            {
                return stockvalue;
            }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException();
                stockvalue = value;
            }
        }

        public PEK (uint id, int quantity = 0, int cost = 0)
        {
            if (id >= 60)
                throw new ArgumentOutOfRangeException();

            this.id = id;
            this.quantity = quantity;
            this.price = cost;
        }

        public override bool Equals(object obj)
        {
            PEK p = obj as PEK;

            return p != null && this.prefix == p.Prefix
                && this.id == p.Id && this.quantity == p.Quantity
                && this.price == p.Price && this.stockvalue == p.StockValue;
        }

        public override int GetHashCode()
        {
            return prefix.GetHashCode() ^ id.GetHashCode() 
                ^ quantity.GetHashCode() ^ price.GetHashCode()
                ^ stockvalue.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(Prefix);

            s.Append(prefix);
            s.AppendLine(id.ToString());
            s.Append(" Quantity: ");
            s.AppendLine(quantity.ToString());
            s.Append(" Price: ");
            s.AppendLine(price.ToString());
            s.Append(" StockValue: ");
            s.AppendLine(stockvalue.ToString());

            return s.ToString();
        }
    }
}
