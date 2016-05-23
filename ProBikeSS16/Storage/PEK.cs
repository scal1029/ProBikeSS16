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
        private int quantity = 0;
        private uint id;
        int cost = 0;


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
        }

        public uint Id
        {
            get
            {
                return id;
            }
        }

        public int Cost
        {
            get
            {
                return cost;
            }
        }

        public PEK (uint id, int quantity = 0, int cost = 0)
        {
            if (id < 60)
                throw new ArgumentOutOfRangeException();

            this.id = id;
            this.quantity = quantity;
            this.cost = cost;
        }

        public override bool Equals(object obj)
        {
            PEK p = obj as PEK;

            return p != null && this.prefix == p.Prefix
                && this.id == p.Id && this.quantity == p.Quantity
                && this.cost == p.Cost;
        }

        public override int GetHashCode()
        {
            return prefix.GetHashCode() ^ id.GetHashCode() 
                ^ quantity.GetHashCode() ^ cost.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder(Prefix);
            
            s.AppendLine(id.ToString());
            s.Append("Quantity: ");
            s.AppendLine(quantity.ToString());
            s.Append("Costs: ");
            s.Append(cost);

            return s.ToString();
        }
    }
}
