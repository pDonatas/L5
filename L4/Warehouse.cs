using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L4
{
    /// <summary>
    /// Warehouse class
    /// </summary>
    public class Warehouse : IEquatable<Warehouse>, IComparable<Warehouse>
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }

        public Warehouse() { }

        public Warehouse(int nr, string name, int quant, double price)
        {
            Number = nr;
            Name = name;
            Quantity = quant;
            Price = price;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ Quantity.GetHashCode();
        }
        public bool Equals(Warehouse other)
        {
            if (other == null) return true;
            return Name.Equals(other.Name) && Quantity.Equals(other.Quantity);
        }

        public int CompareTo(Warehouse other)
        {
            if (other == null) return 1;
            if (Name.CompareTo(other.Name) != 0) return Name.CompareTo(other.Name);
            else return Quantity.CompareTo(other.Quantity);
        }

        public override string ToString()
        {
            string line = string.Format("{0, -20} {1, -20} {2, -20} {3, -20}", Number, Name, Quantity, Price);
            return line;
        }
    }
}