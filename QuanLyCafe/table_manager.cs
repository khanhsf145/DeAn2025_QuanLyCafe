using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;

namespace QuanLyCafe
{
    public partial class table_manager: Form
    {
        private Account loginaccount;

        public Account LoginAccount
        {
            get { return loginaccount; }
            set { loginaccount = value; ChangeAccount(loginaccount.Type); }
        }
        public table_manager(Account acc)
        {
            InitializeComponent();

            this.LoginAccount = acc;

            LoadTable();
            LoadCategory();
            LoadCboTable(cboChooseTable);
        }

        #region Method

        void ChangeAccount(int type)
        {
            admin.Enabled = type == 1;
            thôngTinTàiKhoảnToolStripMenuItem.Text += " (" + LoginAccount.DisplayName + ")";
        }

        void LoadCategory()
        {
            List<Category> listCategory = CategoryDAO.Instance.GetListCategory();
            cboCategory.DataSource = listCategory;
            cboCategory.DisplayMember = "Name";
        }

        void LoadFoodListByCateID(int id)
        {
            List<Food> listFood = FoodDAO.Instance.GetFoodByCategoryID(id);
            cboFoods.DataSource = listFood;
            cboFoods.DisplayMember = "Name";
        }

        void LoadTable()
        {
            flpTable.Controls.Clear();
            List<Table> tableList = TableDAO.Instance.LoadTableList();

            foreach (Table item in tableList)
            {
                Button btn = new Button() { Width = TableDAO.TableWidth, Height = TableDAO.TableHeight };
                btn.Text = item.Name + Environment.NewLine + item.Status;
                btn.Click += btn_Click;
                btn.Tag = item;

                switch (item.Status)
                {
                    case "Trống":
                        btn.BackColor = Color.Azure;
                        btn.ForeColor = Color.DarkSlateGray;
                        break;
                    default:
                        btn.BackColor = Color.DarkSlateGray;
                        btn.ForeColor = Color.Azure;
                        break;
                }

                flpTable.Controls.Add(btn);
            }
        }

        void ShowBill(int id)
        {
            lsvBill.Items.Clear();
            List<QuanLyCafe.DTO.Menu> listBillInfo = MenuDAO.Instance.GetListMenuByTable(id);
            int totalPrice = 0;

            foreach (QuanLyCafe.DTO.Menu item in listBillInfo)
            {
                ListViewItem lsvItem = new ListViewItem(item.Name.ToString());
                lsvItem.SubItems.Add(item.Count.ToString());
                lsvItem.SubItems.Add(item.Price.ToString());
                lsvItem.SubItems.Add(item.Total.ToString());
                totalPrice += item.Total;

                lsvBill.Items.Add(lsvItem);
            }
            //CultureInfo culture = new CultureInfo("vi-VN");

            //Thread.CurrentThread.CurrentCulture = culture;
            //txtTotalPrice.Text = totalPrice.ToString("c", culture);
            txtTotalPrice.Text = totalPrice.ToString();
        }

        void LoadCboTable(ComboBox cb)
        {
            cb.DataSource = TableDAO.Instance.LoadTableList();
            cb.DisplayMember = "Name";
        }
        #endregion

        #region Events
        void btn_Click(object sender, EventArgs e)
        {
            int tableID = ((sender as Button).Tag as Table).ID;
            lsvBill.Tag = (sender as Button).Tag;
            ShowBill(tableID);
        }
        private void logout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có muốn đăng xuất không?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void myInfo_Click(object sender, EventArgs e)
        {
            account_profile frm = new account_profile(LoginAccount);
            frm.UpdateAccount += frm_UpdateAccount;
            frm.ShowDialog();
        }

        void frm_UpdateAccount(object sender, AccountEvent e)
        {
            thôngTinTàiKhoảnToolStripMenuItem.Text = "Thông tin tài khoản (" + e.Acc.DisplayName + ")";
        }

        private void admin_Click(object sender, EventArgs e)
        {
            admin frm = new admin();
            frm.loginAccount = LoginAccount;
            frm.InsertFood += frm_InsertFood;
            frm.DeleteFood += frm_DeleteFood;
            frm.UpdateFood += frm_UpdateFood;
            frm.ShowDialog();
        }

        void frm_UpdateFood(object sender, EventArgs e)
        {
            LoadFoodListByCateID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        void frm_DeleteFood(object sender, EventArgs e)
        {
            LoadFoodListByCateID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
            LoadTable();
        }

        void frm_InsertFood(object sender, EventArgs e)
        {
            LoadFoodListByCateID((cboCategory.SelectedItem as Category).ID);
            if (lsvBill.Tag != null)
                ShowBill((lsvBill.Tag as Table).ID);
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;

            ComboBox cb = sender as ComboBox;

            if (cb.SelectedItem == null)
                return;

            Category selected = cb.SelectedItem as Category;
            id = selected.ID;

            LoadFoodListByCateID(id);
        }
        private void AddFoodBtn_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;

            if (table == null)
            {
                MessageBox.Show("Hãy chọn bàn!");
                return;
            }

            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int foodID = (cboFoods.SelectedItem as Food).ID;
            int count = (int)FoodQty.Value;

            if (idBill == -1)
            {
                BillDAO.Instance.InsertBill(table.ID);
                BillInfoDAO.Instance.InsertBillInfo(BillDAO.Instance.GetMaxIDBill(), foodID, count);
            }
            else
            {
                BillInfoDAO.Instance.InsertBillInfo(idBill, foodID, count);
            }

            ShowBill(table.ID);

            LoadTable();
        }
        private void PayBtn_Click(object sender, EventArgs e)
        {
            Table table = lsvBill.Tag as Table;
            int idBill = BillDAO.Instance.GetUncheckBillIDByTableID(table.ID);
            int discount = (int)numDiscount.Value;
            double total = Convert.ToDouble(txtTotalPrice.Text.Split(',')[0]);
            double finalTotal = total - (total / 100) * discount;

            if (idBill != -1)
            {
                if (MessageBox.Show(string.Format("Bạn có muốn thanh toán hóa đơn cho bàn {0}\nTổng tiền - (Tổng tiền / 100) * Giảm giá\n= {1} - ({1} / 100) * {2} = {3}", table.Name, total, discount, finalTotal) + " không?", "Thông báo", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    BillDAO.Instance.CheckOut(idBill, discount, (int)finalTotal);
                    ShowBill(table.ID);

                    LoadTable();
                }
            }
        }

        private void SwitchBtn_Click(object sender, EventArgs e)
        {
            int id1 = (lsvBill.Tag as Table).ID;

            int id2 = (cboChooseTable.SelectedItem as Table).ID;

            if (MessageBox.Show(string.Format("Bạn có muốn chuyển từ {0} qua {1}", (lsvBill.Tag as Table).Name, (cboChooseTable.SelectedItem as Table).Name), "Thông báo", MessageBoxButtons.OKCancel) == System.Windows.Forms.DialogResult.OK)
            {
                TableDAO.Instance.SwitchTable(id1, id2);

                LoadTable();
            }
        }
        #endregion

        
    }
}
