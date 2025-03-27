using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    public class Food
    {
        private int id;
        private string name;
        private int cate_id;
        private int price;

        public int ID
        {
            get { return id; }
            private set { id = value; }
        }

        public string Name
        {
            get { return name; }
            private set { name = value; }
        }

        public int CateID
        {
            get { return cate_id; }
            private set { cate_id = value; }
        }

        public int Price
        {
            get { return price; }
            private set { price = value; }
        }

        public Food(int id, string name, int cate_id, int price)
        {
            this.ID = id;
            this.Name = name;
            this.CateID = cate_id;
            this.Price = price;
        }

        public Food(DataRow row)
        {
            this.ID = (int)row["id"];
            this.Name = row["name"].ToString();
            this.CateID = (int)row["id_cate"];
            this.Price = (int)Convert.ToDouble(row["price"].ToString());
        }
    }
}
