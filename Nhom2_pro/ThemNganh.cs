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
    public partial class ThemNganh : Form
    {
        public ThemNganh()
        {
            InitializeComponent();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void txtmanganh_TextChanged(object sender, EventArgs e)
        {

        }

        private void txttennganh_TextChanged(object sender, EventArgs e)
        {

        }
        // Chức năng THÊM
        private void btnthem_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO NganhHoc (MaNganh, TenNganh, DaXoa) VALUES (@MaNganh, @TenNganh, 0)", conn);
                    cmd.Parameters.AddWithValue("@MaNganh", txtmanganh.Text);
                    cmd.Parameters.AddWithValue("@TenNganh", txttennganh.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Thêm ngành thành công!");
                    btnnganhhientai_Click(sender, e); // Refresh danh sách ngành hiện tại
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        // Chắc năng SỬA
        private void btnsua_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE NganhHoc SET TenNganh = @TenNganh WHERE MaNganh = @MaNganh", conn);
                    cmd.Parameters.AddWithValue("@MaNganh", txtmanganh.Text);
                    cmd.Parameters.AddWithValue("@TenNganh", txttennganh.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Sửa ngành thành công!");
                    btnnganhhientai_Click(sender, e); // Refresh danh sách ngành hiện tại
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";

            // Kiểm tra xem có hàng nào được chọn trong DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                // Lấy mã ngành của hàng được chọn
                string maNganh = dataGridView1.SelectedRows[0].Cells["MaNganh"].Value.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Lệnh SQL để cập nhật DaXoa = 1 cho ngành đã chọn
                        SqlCommand cmd = new SqlCommand("UPDATE NganhHoc SET DaXoa = 1 WHERE MaNganh = @MaNganh", conn);
                        cmd.Parameters.AddWithValue("@MaNganh", maNganh);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Xóa ngành thành công!");

                            // Tải lại chỉ danh sách ngành hiện tại
                            btnnganhhientai_Click(sender, e); // Cập nhật DataGridView của "Ngành hiện tại"
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy ngành để xóa.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngành để xóa.");
            }
        }



        private void btnnganhhientai_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT MaNganh, TenNganh FROM NganhHoc WHERE DaXoa = 0", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }
        //KHÔI PHỤC NGÀNH
        private void btnkhoiphucnganh_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";

            // Kiểm tra xem có hàng nào được chọn trong DataGridView
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maNganh = dataGridView1.SelectedRows[0].Cells["MaNganh"].Value.ToString();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        // Lệnh SQL cập nhật DaXoa = 0 để khôi phục ngành
                        SqlCommand cmd = new SqlCommand("UPDATE NganhHoc SET DaXoa = 0 WHERE MaNganh = @MaNganh", conn);
                        cmd.Parameters.AddWithValue("@MaNganh", maNganh);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Khôi phục ngành thành công!");

                            // Tải lại chỉ danh sách ngành đã xóa
                            btnnganhdaxoa_Click(sender, e); // Cập nhật DataGridView của "Ngành đã xóa"
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy ngành để khôi phục.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ngành để khôi phục.");
            }
        }



        private void btnnganhdaxoa_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT MaNganh, TenNganh FROM NganhHoc WHERE DaXoa = 1", conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            label1.Left = (panel1.Width - label1.Width) / 2;
            label1.Top = (panel1.Height - label1.Height) / 2;
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        // DANH SÁCH SINH VIÊN THEO NGANH
        private void btndssv_Click(object sender, EventArgs e)
        {
            string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Truy vấn SQL để lấy thông tin ngành và sinh viên trong ngành đó
                    SqlCommand cmd = new SqlCommand(
                        "SELECT nh.TenNganh AS 'Tên Ngành', " +
                        "COUNT(sv.MaSV) OVER(PARTITION BY nh.TenNganh) AS 'Số lượng sinh viên', " +
                        "sv.MaSV AS 'Mã Sinh Viên', sv.Ho AS 'Họ', sv.Ten AS 'Tên', sv.Email AS 'Email', sv.SoDienThoai AS 'Số Điện Thoại' " +
                        "FROM NganhHoc nh " +
                        "LEFT JOIN SinhVien sv ON nh.MaNganh = sv.MaNganh " +
                        "WHERE nh.DaXoa = 0 AND (sv.DaXoa = 0 OR sv.DaXoa IS NULL) " +
                        "ORDER BY nh.TenNganh", conn);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dataGridView1.DataSource = dt; // Hiển thị dữ liệu trong DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void ThemNganh_Load(object sender, EventArgs e)
        {

        }
    }
}
