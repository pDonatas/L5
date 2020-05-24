using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace L4
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label2.Text = "";
            Label6.Text = "";
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Session["price"] = double.Parse(TextBox1.Text);
        }
        /// <summary>
        /// Method for price counting
        /// </summary>
        /// <param name="Orders">List of orders</param>
        /// <param name="Warehouses">List of warehouses</param>
        /// <returns>Price in double</returns>
        double Price(List<Order> Orders, List<Warehouse> Warehouses)
        {
            double price = 0;
            foreach(var order in Orders)
            {
                var query = (from ware in Warehouses where ware.Name.Equals(order.Name) select ware.Price);
                double lowest = 0;
                foreach(var q in query)
                {
                    if(q*order.Quantity < lowest || lowest == 0)
                    {
                        lowest = q*order.Quantity;
                    }
                }
                price += lowest;
            }
            return price;
        }
        /// <summary>
        /// Method for removing cheapest item from list
        /// </summary>
        /// <param name="Orders">List of orders</param>
        /// <param name="Warehouses">List of warehouses</param>
        /// <param name="price">Items price</param>
        void RemoveCheapest(List<Order> Orders, List<Warehouse> Warehouses, double price)
        {
            var cheapest = FindCheapest(Warehouses, Orders);
            foreach(var order in Orders.ToList())
            {
                if (order.Name.Equals(cheapest.Name)) order.Quantity--;
                if (order.Quantity <= 0) Orders.Remove(order);
            }
            GenerateOrder(Orders, Warehouses, price);
        }
        /// <summary>
        /// Method for order generation
        /// </summary>
        /// <param name="Orders">List of orders</param>
        /// <param name="Warehouses">List of warehouses</param>
        /// <param name="price">Items price</param>
        void GenerateOrder(List<Order> Orders, List<Warehouse> Warehouses, double price)
        {
            if (price <= 0) throw new Exception("Sąrašas negali būti sudarytas su tokia kaina");
            if(Price(Orders, Warehouses) == 0) throw new Exception("Sąrašas negali būti sudarytas su tokia kaina");
            if (Price(Orders, Warehouses) > price) RemoveCheapest(Orders, Warehouses, price);
        }
        /// <summary>
        /// Method for finding cheapest thing in list
        /// </summary>
        /// <param name="warehouses">List of warehouses</param>
        /// <param name="orders">List of orders</param>
        /// <returns>Cheapest item in warehouse</returns>
        Warehouse FindCheapest(List<Warehouse> warehouses, List<Order> orders)
        {
            double min = 0;
            Warehouse cheapest = new Warehouse();
            foreach(var order in orders)
            {
                var query = (from ware in warehouses where ware.Name.Equals(order.Name) select ware);
                foreach(var war in query)
                {
                    if(war.Price < min || min == 0)
                    {
                        min = war.Price;
                        cheapest = war;
                    }
                }
            }
            if (cheapest.Price == 0) throw new Exception("Pigiausia prekė nerasta");
            return cheapest;
        }
        /// <summary>
        /// Method for printing warehouses
        /// </summary>
        /// <param name="table">Table object</param>
        /// <param name="warehouses">List of warehouses</param>
        /// <param name="sw">StreamWriter object</param>
        void Print(Table table, List<Warehouse> warehouses, StreamWriter sw)
        {
            sw.WriteLine("{0,-20} {1,-20}", "Name", "Quantity");
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = "Number";
            row.Cells.Add(cell);
            TableCell cell2 = new TableCell();
            cell2.Text = "Name";
            row.Cells.Add(cell2);
            TableCell cell3 = new TableCell();
            cell3.Text = "Quantity";
            row.Cells.Add(cell3);
            TableCell cell4 = new TableCell();
            cell4.Text = "Price";
            row.Cells.Add(cell4);
            table.Rows.Add(row);
            var nr = 1;
            foreach (var ware in warehouses)
            {
                if (nr != ware.Number)
                {
                    sw.WriteLine("-----------------------------------------------------------------------------");
                    nr = ware.Number;
                }
                sw.WriteLine(ware);
                TableRow row1 = new TableRow();
                TableCell cell11 = new TableCell();
                cell11.Text = ware.Number.ToString();
                row1.Cells.Add(cell11);
                TableCell cell22 = new TableCell();
                cell22.Text = ware.Name;
                row1.Cells.Add(cell22);
                TableCell cell33 = new TableCell();
                cell33.Text = ware.Quantity.ToString();
                row1.Cells.Add(cell33);
                TableCell cell44 = new TableCell();
                cell44.Text = ware.Price.ToString();
                row1.Cells.Add(cell44);
                table.Rows.Add(row1);
            }
            sw.WriteLine("-----------------------------------------------------------------------------");
        }
        /// <summary>
        /// Method for printing Orders
        /// </summary>
        /// <param name="table">Table object</param>
        /// <param name="Orders">Orders object</param>
        /// <param name="sw">StreamWriter object</param>
        void Print(Table table, List<Order> Orders, StreamWriter sw)
        {
            sw.WriteLine("{0,-20} {1,-20}", "Name", "Quantity");
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            cell.Text = "Name";
            row.Cells.Add(cell);
            TableCell cell2 = new TableCell();
            cell2.Text = "Quantity";
            row.Cells.Add(cell2);
            table.Rows.Add(row);
            foreach (var order in Orders)
            {
                sw.WriteLine(order);
                TableRow row1 = new TableRow();
                TableCell cell11 = new TableCell();
                cell11.Text = order.Name;
                row1.Cells.Add(cell11);
                TableCell cell22 = new TableCell();
                cell22.Text = order.Quantity.ToString();
                row1.Cells.Add(cell22);
                table.Rows.Add(row1);
            }
            sw.WriteLine("----------------------------------------------------");
        }
        /// <summary>
        /// Method for reading file information
        /// </summary>
        /// <param name="Warehouses">List of warehouses</param>
        /// <param name="Orders">List of orders</param>
        public void ReadFile(List<Warehouse> Warehouses, List<Order> Orders)
        {
            string path = Server.MapPath("~/App_Data/");
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(file))
                    {
                        if (!Path.GetFileName(file).Equals("Uzsakymas.txt"))
                        {
                            string line = null;
                            int nr = int.Parse(reader.ReadLine());
                            while (null != (line = reader.ReadLine()))
                            {
                                try
                                {
                                    string[] sl = line.Split(';');

                                    Warehouse warehouse = new Warehouse(nr, sl[0], int.Parse(sl[1]), double.Parse(sl[2]));

                                    Warehouses.Add(warehouse);
                                }
                                catch (Exception ex)
                                {
                                    Label2.Text = "Klaida: " + ex;
                                    return;
                                }
                            }
                        }
                        else
                        {
                            string line = null;
                            while (null != (line = reader.ReadLine()))
                            {
                                try
                                {
                                    string[] sl = line.Split(';');
                                    Order order = new Order(sl[0], int.Parse(sl[1]));
                                    Orders.Add(order);
                                }
                                catch (Exception ex)
                                {
                                    Label2.Text = "Klaida: " + ex;
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Label2.Text = "Klaida: " + ex;
                    return;
                }
            }
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            List<Warehouse> Warehouses = new List<Warehouse>();
            List<Order> Orders = new List<Order>();
            try
            {
                StreamWriter sw = new StreamWriter(Server.MapPath("rezultatai.txt"));
                double price = (double)Session["price"];
                ReadFile(Warehouses, Orders);
                Label3.Text = "Pradiniai duomenys";
                sw.WriteLine(Label3.Text);
                Label6.Text = "Užsakymas";
                sw.WriteLine(Label6.Text);
                Print(Table2, Orders, sw);
                GenerateOrder(Orders, Warehouses, price);

                Label4.Text = "Prekės:";
                sw.WriteLine(Label4.Text);
                Print(Table1, Warehouses, sw);
                Label5.Text = "Kaina: " + Price(Orders, Warehouses);
                List<Order> Sort = Orders
                .OrderBy(nn => nn.Name)
                .ThenBy(nn => nn.Quantity)
                .ToList<Order>();                Label7.Text = "Rezultatai";
                sw.WriteLine(Label7.Text);
                Print(Table3, Sort, sw);

                sw.Close();
            }
            catch (Exception ex)
            {
                Label2.Text = "Klaida: " + ex.Message;
                return;
            }
        }
    }
}