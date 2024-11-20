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
    public partial class QLMH : Form
    {
        public QLMH()
        {
            InitializeComponent();
        }
        string connectionString = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void QLMH_Load(object sender, EventArgs e)
        {
            LoadKhoaHoc();
            LoadMonHoc(false);
        }
        private void LoadKhoaHoc()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MaLoai, TenLoai FROM LoaiKhoaHoc";
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                cbKhoahoc.Items.Clear();
                while (reader.Read())
                {
                    cbKhoahoc.Items.Add(new { MaLoai = reader["MaLoai"], TenLoai = reader["TenLoai"] });
                }
                cbKhoahoc.DisplayMember = "TenLoai";
                cbKhoahoc.ValueMember = "MaLoai";
            }
        }
        private void LoadMonHoc(bool hienDaXoa)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"SELECT MaMonHoc, TenMon, SoTinChi, SoBuoiHoc, SoVangToiDa, DiemQuaMon, MaLoai, DaXoa 
                                 FROM MonHoc WHERE DaXoa = @DaXoa";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@DaXoa", hienDaXoa ? 1 : 0);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO MonHoc (MaMonHoc,TenMon, SoTinChi, SoBuoiHoc, SoVangToiDa, DiemQuaMon, MaLoai, DaXoa) 
                                 VALUES (@MaMonhoc,@TenMon, @SoTinChi, @SoBuoiHoc, @SoVangToiDa, @DiemQuaMon, @MaLoai, 0)";
                SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@MaMonHoc", txtMaMonHoc.Text.Trim());
                command.Parameters.AddWithValue("@TenMon", txtTenMon.Text.Trim());
                command.Parameters.AddWithValue("@SoTinChi", nrSoTinChi.Value);
                command.Parameters.AddWithValue("@SoBuoiHoc", nrSoBuoi.Value);
                command.Parameters.AddWithValue("@SoVangToiDa", nrSoVangMax.Value);
                command.Parameters.AddWithValue("@DiemQuaMon", txtDiemQuaMon.Text.Trim());
                command.Parameters.AddWithValue("@MaLoai", ((dynamic)cbKhoahoc.SelectedItem).MaLoai);

                command.ExecuteNonQuery();
                MessageBox.Show("Thêm môn học thành công!", "Thông báo");
                LoadMonHoc(false);
            }
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maMonHoc = dataGridView1.SelectedRows[0].Cells["MaMonHoc"].Value.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"UPDATE MonHoc 
                                     SET TenMon = @TenMon, SoTinChi = @SoTinChi, SoBuoiHoc = @SoBuoiHoc, 
                                         SoVangToiDa = @SoVangToiDa, DiemQuaMon = @DiemQuaMon, MaLoai = @MaLoai
                                     WHERE MaMonHoc = @MaMonHoc";
                    SqlCommand command = new SqlCommand(query, connection);

                    command.Parameters.AddWithValue("@TenMon", txtTenMon.Text.Trim());
                    command.Parameters.AddWithValue("@SoTinChi", nrSoTinChi.Value);
                    command.Parameters.AddWithValue("@SoBuoiHoc", nrSoBuoi.Value);
                    command.Parameters.AddWithValue("@SoVangToiDa", nrSoVangMax.Value);
                    command.Parameters.AddWithValue("@DiemQuaMon", txtDiemQuaMon.Text.Trim());
                    command.Parameters.AddWithValue("@MaLoai", ((dynamic)cbKhoahoc.SelectedItem).MaLoai);
                    command.Parameters.AddWithValue("@MaMonHoc", maMonHoc);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Sửa môn học thành công!", "Thông báo");
                    LoadMonHoc(false);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn môn học để sửa!", "Thông báo");
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maMonHoc = dataGridView1.SelectedRows[0].Cells["MaMonHoc"].Value.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE MonHoc SET DaXoa = 1 WHERE MaMonHoc = @MaMonHoc";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaMonHoc", maMonHoc);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Xóa môn học thành công!", "Thông báo");
                    LoadMonHoc(false);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn môn học để xóa!", "Thông báo");
            }
        }

        private void btKhoiPhuc_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                string maMonHoc = dataGridView1.SelectedRows[0].Cells["MaMonHoc"].Value.ToString();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE MonHoc SET DaXoa = 0 WHERE MaMonHoc = @MaMonHoc";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaMonHoc", maMonHoc);

                    command.ExecuteNonQuery();
                    MessageBox.Show("Khôi phục môn học thành công!", "Thông báo");
                    LoadMonHoc(true);
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn môn học để khôi phục!", "Thông báo");
            }

        }

        private void btDaxoa_Click(object sender, EventArgs e)
        {
            LoadMonHoc(true);
        }

        private void btHienTai_Click(object sender, EventArgs e)
        {
            LoadMonHoc(false);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                txtMaMonHoc.Text = row.Cells["MaMonHoc"].Value.ToString();
                txtTenMon.Text = row.Cells["TenMon"].Value.ToString();
                nrSoTinChi.Value = Convert.ToDecimal(row.Cells["SoTinChi"].Value);
                nrSoBuoi.Value = Convert.ToDecimal(row.Cells["SoBuoiHoc"].Value);
                nrSoVangMax.Value = Convert.ToDecimal(row.Cells["SoVangToiDa"].Value);
                txtDiemQuaMon.Text = row.Cells["DiemQuaMon"].Value.ToString();

                foreach (var item in cbKhoahoc.Items)
                {
                    dynamic khoahoc = item;
                    if (khoahoc.MaLoai.ToString() == row.Cells["MaLoai"].Value.ToString())
                    {
                        cbKhoahoc.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private void btThoay_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
