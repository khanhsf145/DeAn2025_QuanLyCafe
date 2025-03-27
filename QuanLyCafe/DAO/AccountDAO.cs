using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;

namespace QuanLyCafe.DAO
{
    public class AccountDAO
    {
        private static AccountDAO instance;

        public static AccountDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new AccountDAO();
                return AccountDAO.instance;
            }
            private set
            {
                instance = value;
            }
        }
        private AccountDAO() { }

        public bool Login(string username, string password)
        {
            string query = "USP_Login @username , @password";
            DataTable result = DataProvider.Instance.ExecuteQuery(query, new object[]{username, password});
            return result.Rows.Count > 0;
        }

        public bool UpdateAccount(string username, string displayname, string password, string newpass)
        {
            int result = DataProvider.Instance.ExecuteNonQuery("exec USP_UpdateAccount @userName , @displayName , @password , @newpass", new object[] { username, displayname, password, newpass });
            return result > 0;
        }

        public DataTable GetListAccount()
        {
            return DataProvider.Instance.ExecuteQuery("select username, display_name, type from Account");
        }

        public Account GetAccountByUsername(string username)
        {
            string query = "select * from Account where username = '" + username + "'";
            DataTable data = DataProvider.Instance.ExecuteQuery(query);
            foreach (DataRow item in data.Rows)
            {
                return new Account(item);
            }
            return null;
        }

        public bool InsertAccount(string username, string display_name, int type)
        {
            string query = string.Format("insert into Account (username, display_name, type) values ('{0}', N'{1}', {2})", username, display_name, type);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool EditAccount(string username, string display_name, int type)
        {
            string query = string.Format("update Account set display_name = N'{0}', type = {1} where username = '{2}'", display_name, type, username);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }

        public bool DeleteAccount(string username)
        {
            string query = string.Format("delete Account where username = '{0}'", username);
            int result = DataProvider.Instance.ExecuteNonQuery(query);

            return result > 0;
        }
    }
}
