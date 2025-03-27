using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    public class Menu
    {
        private string name;
        private int count;
        private int price;
        private int total;
        private int totalPrice;

        public int Total
        {
            get { return total; }
            set { total = value; }
        }


        public int Price
        {
            get { return price; }
            set { price = value; }
        }


        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public Menu(string name, int count, int price, int total = 0)
        {
            this.Name = name;
            this.Count = count;
            this.Price = price;
            this.Total = total;
        }

        public Menu(DataRow row)
        {
            this.Name = row["name"].ToString();
            this.Price = (int)Convert.ToDouble(row["price"].ToString());
            this.Count = (int)row["count"];
            this.Total = (int)Convert.ToDouble(row["total"].ToString());
        }
    }
}
