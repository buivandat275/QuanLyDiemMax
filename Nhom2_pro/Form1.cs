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

namespace Nhom2_pro
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtpassword.PasswordChar = '*';
        }

        private void btndn_Click(object sender, EventArgs e)
        {
            //string connectionString = "Data Source=HOANG-QUOC-DUY;Initial Catalog=QuanLySinhVien;Integrated Security=True;"; // Chuỗi kết nối đến SQL Server
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";//Dat
            string query = "SELECT COUNT(*) FROM NguoiDung WHERE UserName = @username AND Password = @password";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", txtuser.Text);
                command.Parameters.AddWithValue("@password", txtpassword.Text);

                try
                {
                    connection.Open();
                    int result = (int)command.ExecuteScalar();

                    if (result > 0)
                    {
                        // Đăng nhập thành công, chuyển sang form FrmMenu
                        Menu menuForm = new Menu();
                        menuForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        // Đăng nhập thất bại
                        MessageBox.Show("Bạn đã nhập sai thông tin đăng nhập", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Có lỗi xảy ra: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtpassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtpassword.PasswordChar = '\0';
            }
            else
            {
                txtpassword.PasswordChar = '*';
            }
        
    }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
