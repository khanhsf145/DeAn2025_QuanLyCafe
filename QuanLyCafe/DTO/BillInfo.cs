using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    class BillInfo
    {
        private int id;
        private int idBill;
        private int idFood;
        private int count;
        public BillInfo(int id, int idBill, int idFood, int count)
        {
            this.ID = id;
            this.IDBill = idBill;
            this.IDFood = idFood;
            this.Count = count;
        }
        public int ID
        {
            get { return id; }
            set { id = value; }
        }
        public int IDBill
        {
            get { return idBill; }
            set { idBill = value; }
        }
        public int IDFood
        {
            get { return idFood; }
            set { idFood = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }

        public BillInfo(DataRow row)
        {
            this.ID = (int)row["id"];
            this.IDBill = (int)row["id_bill"];
            this.IDFood = (int)row["id_food"];
            this.Count = (int)row["count"];
        }
    }
}
