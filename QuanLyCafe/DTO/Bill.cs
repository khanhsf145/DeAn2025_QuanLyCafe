﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCafe.DTO
{
    class Bill
    {
        public Bill(int id,  DateTime? dateCheckIn, DateTime? dateCheckOut, int status, int idTable, int discount = 0)
        {
            this.ID = id;
            this.DateCheckIn = dateCheckIn;
            this.DateCheckOut = dateCheckOut;
            this.Status = status;
            this.IDTable = idTable;
            this.Discount = discount;
        }

        public Bill(DataRow row)
        {
            this.ID = (int)row["id"];
            this.IDTable = (int)row["id_table"];
            this.DateCheckIn = (DateTime?)row["date_checkin"];
            var dateCheckOutTemp = row["date_checkout"];
            if (dateCheckOutTemp.ToString() != "")
            {
                this.DateCheckOut = (DateTime?)dateCheckOutTemp;
            }
            this.Status = (int)row["status"];
            if (row["discount"].ToString() != "")
                this.Discount = (int)row["discount"];
        }

        private int discount;
        public int Discount
        {
            get { return discount; }
            set { discount = value; }
        }

        private DateTime? dateCheckOut;
        public DateTime? DateCheckOut
        {
            get { return dateCheckOut; }
            set { dateCheckOut = value; }
        }

        private int status;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        private DateTime? dateCheckIn;
        public DateTime? DateCheckIn
        {
            get { return dateCheckIn; }
            set { dateCheckIn = value; }
        }

        private int id;
        public int ID
        {
            get { return id; }
            set { id = value; }
        }

        private int idTable;
        public int IDTable
        {
            get { return idTable; }
            set { idTable = value; }
        }
    }
}
