using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace L4
{
    /// <summary>
    /// Order class
    /// </summary>
    public class Order
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Order(string pav, int kiek)
        {
            Name = pav;
            Quantity = kiek;
        }

        public override string ToString()
        {
            string line = string.Format("{0, -20} {1, -20}", Name, Quantity);
            return line;
        }
    }
}