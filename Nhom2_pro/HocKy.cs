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
    public partial class HocKy : Form
    {
        public HocKy()
        {
            InitializeComponent();
            ketnoi();
        }
        SqlConnection conn;
        DataSet ds;
        SqlDataAdapter sqlda;
        public void ketnoi()
        {
            string ketnoi;
            ketnoi = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
            conn = new SqlConnection(ketnoi);
            conn.Open();
        }

        private void bthientai_Click(object sender, EventArgs e)
        {
            dgvDanhSachHocKy.Enabled = true; //kich hoat lai datagrid
            LoadDanhSachHocKy();
            btnthem.Enabled = btnsua.Enabled = btnxoa.Enabled = true;
            btnkhoiphuc.Enabled = false;
        }

        private void btnthem_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm học kỳ này không?", "Xác nhận thêm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            ketnoi();
            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM HocKy WHERE MaHocKy = @MaHocKy", conn);
            cmdCheck.Parameters.AddWithValue("@MaHocKy", txtmahocky.Text);

            int count = (int)cmdCheck.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Mã học kỳ này đã tồn tại. Vui lòng nhập mã khác.");
                conn.Close();
                return;
            }
            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO HocKy (MaHocKy,TenHocKy, Nam, DaXoa) VALUES (@MaHocKy,@TenHocKy, @Nam, 0)", conn);
                cmd.Parameters.AddWithValue("@MaHocKy", txtmahocky.Text);
                cmd.Parameters.AddWithValue("@TenHocKy", txttenhocky.Text);
                cmd.Parameters.AddWithValue("@Nam", txtNam.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Thêm học kỳ thành công!");
                LoadDanhSachHocKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }
        private void LoadDanhSachHocKy()
        {
            ketnoi();
            SqlCommand command = new SqlCommand("SELECT MaHocKy, TenHocKy,Nam FROM HocKy WHERE DaXoa = 0", conn);
            sqlda = new SqlDataAdapter(command);
            ds = new DataSet();
            sqlda.Fill(ds, "HocKy");

            dgvDanhSachHocKy.DataSource = ds.Tables["HocKy"];
            conn.Close();
        }
        private void HocKy_Load(object sender, EventArgs e)
        {
            LoadDanhSachHocKy();
            LoadComboBoxHocKy();
            btnkhoiphuc.Enabled = false; 
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtmahocky.Text))
            {
                MessageBox.Show("Mã học kỳ không được để trống.");
                txttenhocky.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txttenhocky.Text))
            {
                MessageBox.Show("Tên học kỳ không được để trống.");
                txttenhocky.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNam.Text) || !int.TryParse(txtNam.Text, out int nam))
            {
                MessageBox.Show("Năm phải là một số nguyên hợp lệ.");
                txtNam.Focus();
                return false;
            }
            return true;
        }

        private void btnsua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtmahocky.Text))
            {
                MessageBox.Show("Vui lòng chọn học kỳ cần sửa.");
                return;
            }
            if (KiemTraKhoaNgoai(txtmahocky.Text))
            {
                MessageBox.Show("Không thể xóa học kỳ này vì có dữ liệu liên kết.");
                return;
            }
            if (!ValidateInput()) return;

            DialogResult result = MessageBox.Show("Bạn có chắc muốn sửa học kỳ này không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                ketnoi();
                SqlCommand cmd = new SqlCommand(
                    "UPDATE HocKy SET TenHocKy = @TenHocKy, Nam = @Nam WHERE MaHocKy = @MaHocKy AND DaXoa = 0", conn);
                cmd.Parameters.AddWithValue("@MaHocKy", txtmahocky.Text);
                cmd.Parameters.AddWithValue("@TenHocKy", txttenhocky.Text);
                cmd.Parameters.AddWithValue("@Nam", txtNam.Text);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Sửa thông tin học kỳ thành công!");
                LoadDanhSachHocKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void dgvDanhSachHocKy_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDanhSachHocKy.Rows[e.RowIndex];
                txtmahocky.Text = row.Cells["MaHocKy"].Value.ToString();
                txttenhocky.Text = row.Cells["TenHocKy"].Value.ToString();
                txtNam.Text = row.Cells["Nam"].Value.ToString();
            }
        }

        private void btnxoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSachHocKy.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn học kỳ cần xóa.");
                return;
            }
            if (KiemTraKhoaNgoai(txtmahocky.Text))
            {
                MessageBox.Show("Không thể xóa học kỳ này vì có dữ liệu liên kết.");
                return;
            }

            DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa học kỳ này không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            try
            {
                ketnoi();
                string maHocKy = dgvDanhSachHocKy.SelectedRows[0].Cells["MaHocKy"].Value.ToString();

                SqlCommand cmd = new SqlCommand("UPDATE HocKy SET DaXoa = 1 WHERE MaHocKy = @MaHocKy", conn);
                cmd.Parameters.AddWithValue("@MaHocKy", maHocKy);
                cmd.ExecuteNonQuery();

                MessageBox.Show("Xóa học kỳ thành công!");
                LoadDanhSachHocKy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btdaxoa_Click(object sender, EventArgs e)
        {
            ketnoi();
            SqlCommand command = new SqlCommand("SELECT MaHocKy, TenHocKy,Nam FROM HocKy WHERE DaXoa = 1", conn);
            sqlda = new SqlDataAdapter(command);
            ds = new DataSet();
            sqlda.Fill(ds, "HocKy_DaXoa");
            dgvDanhSachHocKy.DataSource = ds.Tables["HocKy_DaXoa"];

            btnthem.Enabled = btnsua.Enabled = btnxoa.Enabled = false;
            btnkhoiphuc.Enabled = true;
            conn.Close();
        }

        private void btnkhoiphuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhSachHocKy.SelectedRows.Count > 0)
            {
                string maHocKy = dgvDanhSachHocKy.SelectedRows[0].Cells["MaHocKy"].Value.ToString();

                DialogResult result = MessageBox.Show("Bạn có chắc muốn khôi phục học kỳ này không?", "Xác nhận khôi phục", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;

                try
                {
                    ketnoi();
                    SqlCommand cmd = new SqlCommand("UPDATE HocKy SET DaXoa = 0 WHERE MaHocKy = @MaHocKy", conn);
                    cmd.Parameters.AddWithValue("@MaHocKy", maHocKy);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Khôi phục học kỳ thành công!");
                    btdaxoa_Click(sender, e); // Refresh danh sách học kỳ đã xóa
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn học kỳ để khôi phục.");
            }
        }
        private void LoadComboBoxHocKy()
        {
            ketnoi();
            SqlCommand cmdHocKy = new SqlCommand("SELECT MaHocKy, TenHocKy FROM HocKy WHERE DaXoa = 0", conn);
            SqlDataAdapter daHocKy = new SqlDataAdapter(cmdHocKy);
            DataTable dtHocKy = new DataTable();
            daHocKy.Fill(dtHocKy);
            cbHocKy.DataSource = dtHocKy;
            cbHocKy.DisplayMember = "TenHocKy";
            cbHocKy.ValueMember = "MaHocKy";
            conn.Close();
        }


        private void btnd_Click(object sender, EventArgs e)
        {
            dgvDanhSachHocKy.Enabled = false;//dong datagrid
            ketnoi();
            SqlCommand command = new SqlCommand(
                "SELECT LopHoc.MaLop, MonHoc.TenMon, LopHoc.TenLop, LopHoc.SoLuongSV, HocKy.TenHocKy " +
                "FROM LopHoc " +
                "JOIN MonHoc ON LopHoc.MaMonHoc = MonHoc.MaMonHoc " +
                "JOIN HocKy ON LopHoc.MaHocKy = HocKy.MaHocKy " +
                "WHERE LopHoc.DaXoa = 0 AND HocKy.MaHocKy = @MaHocKy", conn);

            command.Parameters.AddWithValue("@MaHocKy", cbHocKy.SelectedValue);

            SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dgvDanhSachHocKy.DataSource = dt;
            conn.Close();
        }

        private void btnthoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool KiemTraKhoaNgoai(string maHocKy)
        {
            ketnoi();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LopHoc WHERE MaHocKy = @MaHocKy", conn);
            cmd.Parameters.AddWithValue("@MaHocKy", maHocKy);

            int count = (int)cmd.ExecuteScalar();
            conn.Close();

            return count > 0;
        }

    }
}
