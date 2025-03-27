using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCafe.DAO;
using QuanLyCafe.DTO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace QuanLyCafe
{
    public partial class admin : Form
    {
        BindingSource foodList = new BindingSource();

        BindingSource accList = new BindingSource();
        public Account loginAccount;

        public admin()
        {
            InitializeComponent();

            LoadData();
        }

        #region methods

        void LoadData()
        {
            dataFoods.DataSource = foodList;
            dataAccounts.DataSource = accList;

            LoadDateTimePickerBill();
            LoadListBillByDate(fromDate.Value, toDate.Value);
            LoadListFood();
            LoadCategoryIntoCombobox(cboFoodCate);
            AddFoodBinding();
            LoadListCategory();
            LoadListTable();
            AddCategoryBinding();
            AddTableBinding();
            AddAccountBinding();
            LoadAccount();
        }

        void LoadDateTimePickerBill()
        {
            DateTime today = DateTime.Now;
            fromDate.Value = new DateTime(today.Year, today.Month, 1);
            toDate.Value = fromDate.Value.AddMonths(1).AddDays(-1);
        }

        void LoadListBillByDate(DateTime checkIn, DateTime checkOut)
        {
            dataBills.DataSource = BillDAO.Instance.GetBillListByDate(checkIn, checkOut);
        }

        void AddFoodBinding()
        {
            txtFoodName.DataBindings.Add(new Binding("Text", dataFoods.DataSource, "name", true, DataSourceUpdateMode.Never));
            txtFoodID.DataBindings.Add(new Binding("Text", dataFoods.DataSource, "id", true, DataSourceUpdateMode.Never));
            numPrice.DataBindings.Add(new Binding("Value", dataFoods.DataSource, "price", true, DataSourceUpdateMode.Never));
        }

        void AddCategoryBinding()
        {
            txtCateName.DataBindings.Add(new Binding("Text", dataCategory.DataSource, "name", true, DataSourceUpdateMode.Never));
            txtCateID.DataBindings.Add(new Binding("Text", dataCategory.DataSource, "id", true, DataSourceUpdateMode.Never));
        }

        void AddTableBinding()
        {
            txtTableName.DataBindings.Add(new Binding("Text", dataTables.DataSource, "name", true, DataSourceUpdateMode.Never));
            txtTableID.DataBindings.Add(new Binding("Text", dataTables.DataSource, "id", true, DataSourceUpdateMode.Never));
            cboTableStatus.DataBindings.Add(new Binding("Text", dataTables.DataSource, "status", true, DataSourceUpdateMode.Never));
        }

        void AddAccountBinding()
        {
            txtAccUsername.DataBindings.Add(new Binding("Text", dataAccounts.DataSource, "username", true, DataSourceUpdateMode.Never));
            txtAccName.DataBindings.Add(new Binding("Text", dataAccounts.DataSource, "display_name", true, DataSourceUpdateMode.Never));
            cboAccType.DataBindings.Add(new Binding("Text", dataAccounts.DataSource, "type", true, DataSourceUpdateMode.Never));
        }

        void LoadAccount()
        {
            accList.DataSource = AccountDAO.Instance.GetListAccount();
        }

        void LoadCategoryIntoCombobox(ComboBox cb)
        {
            cb.DataSource = CategoryDAO.Instance.GetListCategory();
            cb.DisplayMember = "name";
        }

        void LoadListFood()
        {
            foodList.DataSource = FoodDAO.Instance.GetListFood();
        }

        void LoadListCategory()
        {
            dataCategory.DataSource = CategoryDAO.Instance.GetListCategory();
        }

        void LoadListTable()
        {
            dataTables.DataSource = TableDAO.Instance.LoadTableList();
        }

        List<Food> SearchFoodByName(string name)
        {
            List<Food> listFood = FoodDAO.Instance.SearchFoodByName(name);

            return listFood;
        }

        void AddAccount(string username, string display_name, int type)
        {
            if (AccountDAO.Instance.InsertAccount(username, display_name, type))
            {
                MessageBox.Show("Thêm tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }

            LoadAccount();
        }

        void EditAccount(string username, string display_name, int type)
        {
            if (AccountDAO.Instance.EditAccount(username, display_name, type))
            {
                MessageBox.Show("Cập nhật thông tin tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }

            LoadAccount();
        }

        void DeleteAccount(string username)
        {
            if (loginAccount.Username.Equals(username))
            {
                MessageBox.Show("Không thể xóa tài khoản hiện thời!");
                return;
            }
            if (AccountDAO.Instance.DeleteAccount(username))
            {
                MessageBox.Show("Xóa tài khoản thành công!");
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }

            LoadAccount();
        }

        #endregion

        #region events
        private void BillsViewBtn_Click(object sender, EventArgs e)
        {
            LoadListBillByDate(fromDate.Value, toDate.Value);
        }



        // MÓN
        private void ViewFoodBtn_Click(object sender, EventArgs e)
        {
            LoadListFood();
        }
        private void txtFoodID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataFoods.SelectedCells.Count > 0)
                {
                    int id = (int)dataFoods.SelectedCells[0].OwningRow.Cells["CateID"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    cboFoodCate.SelectedItem = category;

                    int index = -1;
                    int i = 0;
                    foreach (Category item in cboFoodCate.Items)
                    {
                        if (item.ID == category.ID)
                        {
                            index = i;
                            break;
                        }
                        i++;
                    }

                    cboFoodCate.SelectedIndex = index;
                }
            }
            catch { }
        }
        private void AddFoodBtn_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int id_cate = (cboFoodCate.SelectedItem as Category).ID;
            int price = (int)numPrice.Value;

            if (FoodDAO.Instance.InsertFood(name, id_cate, price))
            {
                MessageBox.Show("Thêm món thành công!");
                LoadListFood();

                if (insert_food != null)
                    insert_food(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi thêm món vào thực đơn!");
            }
        }
        private void UpdateFoodBtn_Click(object sender, EventArgs e)
        {
            string name = txtFoodName.Text;
            int id_cate = (cboFoodCate.SelectedItem as Category).ID;
            int price = (int)numPrice.Value;
            int id = Convert.ToInt32(txtFoodID.Text);

            if (FoodDAO.Instance.UpdateFood(id, name, id_cate, price))
            {
                MessageBox.Show("Sửa thông tin món thành công!");
                LoadListFood();

                if (update_food != null)
                    update_food(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi sửa thông tin món!");
            }
        }
        private void DeleteFoodBtn_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtFoodID.Text);

            if (FoodDAO.Instance.DeleteFood(id))
            {
                MessageBox.Show("Xóa món thành công!");
                LoadListFood();

                if (delete_food != null)
                    delete_food(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra khi xóa món!");
            }
        }
        private event EventHandler insert_food;
        public event EventHandler InsertFood
        {
            add { insert_food += value; }
            remove { insert_food -= value; }
        }
        private event EventHandler delete_food;
        public event EventHandler DeleteFood
        {
            add { delete_food += value; }
            remove { delete_food -= value; }
        }
        private event EventHandler update_food;
        public event EventHandler UpdateFood
        {
            add { update_food += value; }
            remove { update_food -= value; }
        }
        private void SearchFoodBtn_Click(object sender, EventArgs e)
        {
            foodList.DataSource = SearchFoodByName(txtFoodName.Text);
        }



        // DANH MỤC
        private void txtCateID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataCategory.SelectedCells.Count > 0)
                {
                    int id = (int)dataCategory.SelectedCells[0].OwningRow.Cells["id"].Value;

                    Category category = CategoryDAO.Instance.GetCategoryByID(id);

                    txtCateName.Text = category.Name;
                }
            }
            catch { }
        }
        private void ViewCateBtn_Click(object sender, EventArgs e)
        {
            LoadListCategory();
        }
        private void AddCateBtn_Click(object sender, EventArgs e)
        {
            string name = txtCateName.Text;

            if (CategoryDAO.Instance.InsertCate(name))
            {
                MessageBox.Show("Thêm danh mục thành công!");
                LoadListCategory();

                if (insert_cate != null)
                    insert_cate(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private void UpdateCateBtn_Click(object sender, EventArgs e)
        {
            string name = txtCateName.Text;
            int id = Convert.ToInt32(txtCateID.Text);

            if (CategoryDAO.Instance.UpdateCate(id, name))
            {
                MessageBox.Show("Sửa thông tin danh mục thành công!");
                LoadListCategory();

                if (update_cate != null)
                    update_cate(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private void DeleteCateBtn_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtCateID.Text);

            if (CategoryDAO.Instance.DeleteCate(id))
            {
                MessageBox.Show("Xóa danh mục thành công!");
                LoadListCategory();

                if (delete_cate != null)
                    delete_cate(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private event EventHandler insert_cate;
        public event EventHandler InsertCategory
        {
            add { insert_cate += value; }
            remove { insert_cate -= value; }
        }
        private event EventHandler update_cate;
        public event EventHandler UpdateCategory
        {
            add { update_cate += value; }
            remove { update_cate -= value; }
        }
        private event EventHandler delete_cate;
        public event EventHandler DeleteCategory
        {
            add { delete_cate += value; }
            remove { delete_cate -= value; }
        }



        // BÀN
        private void txtTableID_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (dataTables.SelectedCells.Count > 0)
                {
                    int id = (int)dataTables.SelectedCells[0].OwningRow.Cells["id"].Value;

                    Table table = TableDAO.Instance.GetTableByID(id);

                    txtTableName.Text = table.Name;
                    cboTableStatus.SelectedItem = table.Status;
                }
            }
            catch { }
        }
        private void ViewTableBtn_Click(object sender, EventArgs e)
        {
            LoadListTable();
        }
        private void AddTableBtn_Click(object sender, EventArgs e)
        {
            string name = txtTableName.Text;

            if (TableDAO.Instance.InsertTable(name))
            {
                MessageBox.Show("Thêm bàn thành công!");
                LoadListTable();

                if (insert_table != null)
                    insert_table(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private void UpdateTableBtn_Click(object sender, EventArgs e)
        {
            string name = txtTableName.Text;
            int id = Convert.ToInt32(txtTableID.Text);

            if (TableDAO.Instance.UpdateTable(id, name))
            {
                MessageBox.Show("Sửa thông tin bàn thành công!");
                LoadListTable();

                if (update_table != null)
                    update_table(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private void DeleteTableBtn_Click(object sender, EventArgs e)
        {
            int id = Convert.ToInt32(txtTableID.Text);

            if (TableDAO.Instance.DeleteTable(id))
            {
                MessageBox.Show("Xóa bàn thành công!");
                LoadListTable();

                if (delete_table != null)
                    delete_table(this, new EventArgs());
            }
            else
            {
                MessageBox.Show("Có lỗi xảy ra!");
            }
        }
        private event EventHandler insert_table;
        public event EventHandler InsertTable
        {
            add { insert_table += value; }
            remove { insert_table -= value; }
        }
        private event EventHandler update_table;
        public event EventHandler UpdateTable
        {
            add { update_table += value; }
            remove { update_table -= value; }
        }
        private event EventHandler delete_table;
        public event EventHandler DeleteTable
        {
            add { delete_table += value; }
            remove { delete_table -= value; }
        }

        private void ViewAccBtn_Click(object sender, EventArgs e)
        {
            LoadAccount();
        }

        private void AddAccBtn_Click(object sender, EventArgs e)
        {
            string username = txtAccUsername.Text;
            string display_name = txtAccName.Text;
            int type = cboAccType.SelectedIndex;

            AddAccount(username, display_name, type);
        }

        private void UpdateAcBtn_Click(object sender, EventArgs e)
        {
            string username = txtAccUsername.Text;
            string display_name = txtAccName.Text;
            int type = cboAccType.SelectedIndex;

            EditAccount(username, display_name, type);
        }

        private void DeleteAccBtn_Click(object sender, EventArgs e)
        {
            string username = txtAccUsername.Text;

            DeleteAccount(username);
        }



        #endregion


        //void LoadAccountList()
        //{
        //    string query = "exec USP_GetAccountByUsername @username";

        //    dataAccounts.DataSource = DataProvider.Instance.ExecuteQuery(query, new object[] { "staff1" });
        //}
    }
}