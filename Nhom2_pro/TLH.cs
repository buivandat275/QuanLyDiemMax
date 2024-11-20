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
    public partial class TLH : Form
    {
        public TLH()
        {
            InitializeComponent();
            ketnoi(); // Gọi hàm kết nối
            btKhoiPhuc.Enabled = false;

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
        private void LoadComboBoxData()
        {
            ketnoi();

            // Load danh sách môn học
            SqlCommand cmdMonHoc = new SqlCommand("SELECT MaMonHoc, TenMon FROM MonHoc WHERE DaXoa = 0", conn);
            SqlDataAdapter daMonHoc = new SqlDataAdapter(cmdMonHoc);
            DataTable dtMonHoc = new DataTable();
            daMonHoc.Fill(dtMonHoc);
            clbMonHoc.DataSource = dtMonHoc;
            clbMonHoc.DisplayMember = "TenMon";
            clbMonHoc.ValueMember = "MaMonHoc";

            // Load danh sách học kỳ
            SqlCommand cmdHocKy = new SqlCommand("SELECT MaHocKy, TenHocKy FROM HocKy WHERE DaXoa = 0", conn);
            SqlDataAdapter daHocKy = new SqlDataAdapter(cmdHocKy);
            DataTable dtHocKy = new DataTable();
            daHocKy.Fill(dtHocKy);
            cbHocKy.DataSource = dtHocKy;
            cbHocKy.DisplayMember = "TenHocKy";
            cbHocKy.ValueMember = "MaHocKy";

            conn.Close();
        }
        private void LoadDanhSachLop()
        {
            ketnoi();
            SqlCommand command = new SqlCommand(
                "SELECT LopHoc.MaLop, LopHoc.TenLop, LopHoc.SoLuongSV, " +
                "STRING_AGG(MonHoc.TenMon, ', ') AS DanhSachMon, HocKy.TenHocKy " +
                "FROM LopHoc " +
                "JOIN LopHoc_MonHoc ON LopHoc.MaLop = LopHoc_MonHoc.MaLop " +
                "JOIN MonHoc ON LopHoc_MonHoc.MaMonHoc = MonHoc.MaMonHoc " +
                "JOIN HocKy ON LopHoc.MaHocKy = HocKy.MaHocKy " +
                "WHERE LopHoc.DaXoa = 0 " +
                "GROUP BY LopHoc.MaLop, LopHoc.TenLop, LopHoc.SoLuongSV, HocKy.TenHocKy", conn);

            sqlda = new SqlDataAdapter(command);
            ds = new DataSet();
            sqlda.Fill(ds, "LopHoc");

            dgvDanhSachLop.DataSource = ds.Tables["LopHoc"];
            conn.Close();
        }

        private void TLH_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadDanhSachLop();
        }
        private bool KiemTraTenLopTrung(string tenLop)
        {
            try
            {
                ketnoi();
                string query = "SELECT COUNT(*) FROM LopHoc WHERE TenLop = @TenLop AND DaXoa = 0";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TenLop", tenLop);
                int count = (int)cmd.ExecuteScalar();
                return count > 0; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kiểm tra trùng tên lớp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                conn.Close();
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            string tenLop = txt_TenLop.Text.Trim();
            if (!ValidateInput()) return;

            if (cbHocKy.SelectedValue == null || clbMonHoc.CheckedItems.Count == 0 ||
                string.IsNullOrWhiteSpace(txt_TenLop.Text) || string.IsNullOrWhiteSpace(txt_SoSV.Text))
            {
                MessageBox.Show("Vui lòng chọn học kỳ, ít nhất một môn học, nhập tên lớp và số sinh viên.");
                return;
            }
            // Kiểm tra tên lớp có bị trùng không
            if (KiemTraTenLopTrung(tenLop))
            {
                MessageBox.Show("Tên lớp học đã tồn tại, vui lòng chọn tên khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thêm lớp học này không?", "Xác nhận thêm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            ketnoi();

            // Kiểm tra mã lớp trùng
            SqlCommand cmdCheck = new SqlCommand("SELECT COUNT(*) FROM LopHoc WHERE MaLop = @MaLop", conn);
            cmdCheck.Parameters.AddWithValue("@MaLop", txtMaLH.Text);

            int count = (int)cmdCheck.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Mã lớp học này đã tồn tại. Vui lòng nhập mã khác.");
                conn.Close();
                return;
            }

            try
            {
                // Thêm vào bảng LopHoc
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO LopHoc (MaLop, MaHocKy, TenLop, SoLuongSV, DaXoa) " +
                    "VALUES (@MaLop, @MaHocKy, @TenLop, @SoLuongSV, 0)", conn);

                cmd.Parameters.AddWithValue("@MaLop", txtMaLH.Text);
                cmd.Parameters.AddWithValue("@MaHocKy", cbHocKy.SelectedValue);
                cmd.Parameters.AddWithValue("@TenLop", txt_TenLop.Text);
                cmd.Parameters.AddWithValue("@SoLuongSV", txt_SoSV.Text);
                cmd.ExecuteNonQuery();

                // Thêm vào bảng LopHoc_Monhoc
                foreach (DataRowView item in clbMonHoc.CheckedItems)
                {
                    SqlCommand cmdLopMon = new SqlCommand(
                        "INSERT INTO LopHoc_MonHoc (MaLop, MaMonHoc) VALUES (@MaLop, @MaMonHoc)", conn);

                    cmdLopMon.Parameters.AddWithValue("@MaLop", txtMaLH.Text);
                    cmdLopMon.Parameters.AddWithValue("@MaMonHoc", item["MaMonHoc"]);
                    cmdLopMon.ExecuteNonQuery();
                }

                MessageBox.Show("Thêm lớp học thành công!");
                LoadDanhSachLop();
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

        private void btThoay_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc muốn thoát không?", "Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No)
            {
                
                return; 
            }
            this.Close();
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLH.Text) || string.IsNullOrEmpty(txt_TenLop.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp học cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
//   đang loi         // Không cho phép sửa mlơp
           /* if (!txtMaLH.Enabled)
            {
                MessageBox.Show("Không được phép sửa mã lop!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }*/

            if (!ValidateInput()) return;

            // Lấy thông tin từ các TextBox
            string maLop = txtMaLH.Text.Trim();
            string tenLop = txt_TenLop.Text.Trim();
            int soSV = int.Parse(txt_SoSV.Text.Trim());

            // Lấy danh sách môn học được chọn trong CheckedListBox
            List<string> selectedMonHoc = new List<string>();
            foreach (DataRowView item in clbMonHoc.CheckedItems)
            {
                selectedMonHoc.Add(item["MaMonHoc"].ToString());
            }

            if (selectedMonHoc.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một môn học cho lớp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                ketnoi();

                // Cập nhật thông tin lớp học
                string updateLopQuery = "UPDATE LopHoc SET TenLop = @TenLop, SoLuongSV = @SoLuongSV WHERE MaLop = @MaLop";
                SqlCommand cmdUpdateLop = new SqlCommand(updateLopQuery, conn);
                cmdUpdateLop.Parameters.AddWithValue("@TenLop", tenLop);
                cmdUpdateLop.Parameters.AddWithValue("@SoLuongSV", soSV);
                cmdUpdateLop.Parameters.AddWithValue("@MaLop", maLop);
                cmdUpdateLop.ExecuteNonQuery();

                // Xóa các môn học cũ trong bảng LopHoc_MonHoc
                string deleteMonHocQuery = "DELETE FROM LopHoc_MonHoc WHERE MaLop = @MaLop";
                SqlCommand cmdDeleteMonHoc = new SqlCommand(deleteMonHocQuery, conn);
                cmdDeleteMonHoc.Parameters.AddWithValue("@MaLop", maLop);
                cmdDeleteMonHoc.ExecuteNonQuery();

                // Thêm các môn học mới được chọn vào bảng LopHoc_MonHoc
                foreach (string maMon in selectedMonHoc)
                {
                    string insertMonHocQuery = "INSERT INTO LopHoc_MonHoc (MaLop, MaMonHoc) VALUES (@MaLop, @MaMonHoc)";
                    SqlCommand cmdInsertMonHoc = new SqlCommand(insertMonHocQuery, conn);
                    cmdInsertMonHoc.Parameters.AddWithValue("@MaLop", maLop);
                    cmdInsertMonHoc.Parameters.AddWithValue("@MaMonHoc", maMon);
                    cmdInsertMonHoc.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật lại danh sách lớp học trên DataGridView
                LoadDanhSachLop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaLH.Text))
            {
                MessageBox.Show("Vui lòng chọn lớp học cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLop = txtMaLH.Text.Trim();

            try
            {
                ketnoi();

                // Xác nhận xóa
                DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn xóa lớp học này?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                // Cập nhật trạng thái DaXoa = 1 (đã xóa)
                string updateQuery = "UPDATE LopHoc SET DaXoa = 1 WHERE MaLop = @MaLop";
                SqlCommand cmdUpdate = new SqlCommand(updateQuery, conn);
                cmdUpdate.Parameters.AddWithValue("@MaLop", maLop);
                cmdUpdate.ExecuteNonQuery();

                MessageBox.Show("Xóa lớp học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Tải lại danh sách lớp học
                LoadDanhSachLop();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void dgvDanhSachLop_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dgvDanhSachLop_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvDanhSachLop.Rows[e.RowIndex];
                txtMaLH.Text = row.Cells["MaLop"].Value.ToString();
                txt_TenLop.Text = row.Cells["TenLop"].Value.ToString();
                txt_SoSV.Text = row.Cells["SoLuongSV"].Value.ToString();
                string maLop = row.Cells["MaLop"].Value.ToString();

                try
                {
                    ketnoi();

                    // Truy vấn danh sách các môn học thuộc lớp được chọn
                    SqlCommand cmd = new SqlCommand(
                        "SELECT MonHoc.MaMonHoc FROM MonHoc " +
                        "JOIN LopHoc_MonHoc ON LopHoc_MonHoc.MaMonHoc = MonHoc.MaMonHoc " +
                        "WHERE LopHoc_MonHoc.MaLop = @MaLop", conn);
                    cmd.Parameters.AddWithValue("@MaLop", maLop);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dtMonHoc = new DataTable();
                    da.Fill(dtMonHoc);

                    // Duyệt qua tất cả các môn học trong CheckedListBox và bỏ tick trước
                    for (int i = 0; i < clbMonHoc.Items.Count; i++)
                    {
                        clbMonHoc.SetItemChecked(i, false);
                    }

                    // Đánh dấu tick các môn học có trong danh sách
                    foreach (DataRow rowMonHoc in dtMonHoc.Rows)
                    {
                        string maMonHoc = rowMonHoc["MaMonHoc"].ToString();
                        for (int i = 0; i < clbMonHoc.Items.Count; i++)
                        {
                            DataRowView item = (DataRowView)clbMonHoc.Items[i];
                            if (item["MaMonHoc"].ToString() == maMonHoc)
                            {
                                clbMonHoc.SetItemChecked(i, true);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách môn học: " + ex.Message);
                }
                finally
                {
                    conn.Close();
                }
            }
        }
        private int GetSVMax()
        {
            ketnoi();
            SqlCommand cmd = new SqlCommand("SELECT SoSVToiDa FROM CauHinh", conn);
            return (int)cmd.ExecuteScalar();

        }
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txt_TenLop.Text))
            {
                MessageBox.Show("Tên lớp không được để trống.");
                txt_TenLop.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txt_SoSV.Text) || !int.TryParse(txt_SoSV.Text, out int soSinhVien))
            {
                MessageBox.Show("Số sinh viên phải là một số nguyên hợp lệ.");
                txt_SoSV.Focus();
                return false;
            }
            
            int soSVToiDa = GetSVMax();
            if (soSinhVien > soSVToiDa)
            {
                MessageBox.Show("Số sinh viên không được vượt quá 40.");
                txt_SoSV.Focus();
                return false;
            }
/*
            if (cbMonHoc.SelectedIndex == -1)
            {
                MessageBox.Show("Vui lòng chọn môn học.");
                cbMonHoc.Focus();
                return false;
            }*/

            return true;
        }

        private void dgvDanhSachLop_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void lbl_Tieude_Click(object sender, EventArgs e)
        {

        }

        private void dgvDanhSachLop_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btDaxoa_Click(object sender, EventArgs e)
        {
            LoadDanhSachSinhVienDaXoa();
        }
        private void LoadDanhSachSinhVienDaXoa()
        {
            try
            {
                ketnoi();

                // Lấy danh sách lớp học đã bị xóa
                string query = "SELECT * FROM LopHoc WHERE DaXoa = 1";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Hiển thị trong DataGridView
                dgvDanhSachLop.DataSource = dt;
                btThem.Enabled = false;
                btSua.Enabled = false;
                btXoa.Enabled = false;
                btHienTai.Enabled = true; // Mở lại nút Hiện tại
                btKhoiPhuc.Enabled = true;

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không có lớp học nào đã bị xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải danh sách lớp học đã xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                conn.Close();
            }
        }

        private void btHienTai_Click(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadDanhSachLop();
            btThem.Enabled = true;
            btSua.Enabled = true;
            btXoa.Enabled = true;
            btKhoiPhuc.Enabled = false;
        }

        private void btKhoiPhuc_Click(object sender, EventArgs e)
        {
            if (dgvDanhSachLop.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dgvDanhSachLop.SelectedRows[0];
                string maLop = row.Cells["MaLop"].Value.ToString();

                DialogResult result = MessageBox.Show("Bạn có chắc muốn khôi phục lớp học này không?", "Xác nhận khôi phục", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    return;
                }

                try
                {
                    ketnoi();

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE LopHoc SET DaXoa = 0 WHERE MaLop = @MaLop", conn);

                    cmd.Parameters.AddWithValue("@MaLop", maLop);
                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Khôi phục lớp học thành công!");

                    // Tải lại danh sách lớp đã xóa
                    btDaxoa_Click(sender, e);
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
                MessageBox.Show("Vui lòng chọn lớp học để khôi phục.");
            }
        }

        private void btNew_Click(object sender, EventArgs e)
        {
            HocKy hocKy = new HocKy();
            hocKy.Show();
        }

        private void txt_SoSV_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
