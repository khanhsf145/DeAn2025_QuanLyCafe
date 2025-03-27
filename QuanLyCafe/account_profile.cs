using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;

namespace QuanLyCafe
{
    public partial class account_profile: Form
    {
        private Account loginaccount;

        public Account LoginAccount
        {
            get { return loginaccount; }
            set { loginaccount = value; ChangeAccount(loginaccount); }
        }

        public account_profile(Account acc)
        {
            InitializeComponent();

            LoginAccount = acc;
        }

        void ChangeAccount(Account acc)
        {
            txtUsername.Text = LoginAccount.Username;
            txtName.Text = LoginAccount.DisplayName;
        }

        void UpdateAccountInfo()
        {
            string displayname = txtName.Text;
            string password = txtPassword.Text;
            string newpass = txtNewPass.Text;
            string reenterpass = txtNewPass2.Text;
            string username = txtUsername.Text;

            if (!newpass.Equals(reenterpass))
            {
                MessageBox.Show("Vui lòng nhập lại mật khẩu đúng với mật khẩu mới!");
            }
            else
            {
                if (AccountDAO.Instance.UpdateAccount(username, displayname, password, newpass))
                {
                    MessageBox.Show("Cập nhật thành công!");
                    if (updateaccount != null)
                    {
                        updateaccount(this, new AccountEvent(AccountDAO.Instance.GetAccountByUsername(username)));
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng điền đúng mật khẩu!");
                }
            }
        }

        private event EventHandler<AccountEvent> updateaccount;
        public event EventHandler<AccountEvent> UpdateAccount
        {
            add { updateaccount += value; }
            remove { updateaccount -= value; }
        }

        private void ExitBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateBtn_Click(object sender, EventArgs e)
        {
            UpdateAccountInfo();
        }
    }

    public class AccountEvent : EventArgs
    {
        private Account acc;
        public Account Acc
        {
            get { return acc; }
            set { acc = value; }
        }
        public AccountEvent(Account acc)
        {
            this.Acc = acc;
        }
    }
}
