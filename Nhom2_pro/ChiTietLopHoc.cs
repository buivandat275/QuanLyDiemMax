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
    public partial class ChiTietLopHoc : Form
    {
        public ChiTietLopHoc()
        {
            InitializeComponent();
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
        private void ChiTietLopHoc_Load(object sender, EventArgs e)
        {
            ketnoi();
            SqlCommand cmd = new SqlCommand("SELECT MaLop, TenLop FROM LopHoc WHERE DaXoa = 0", conn);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            cbLopHoc.DisplayMember = "TenLop";
            cbLopHoc.ValueMember = "MaLop";
            cbLopHoc.DataSource = dt;
        }
        private void HienThiDanhSachSinhVien(int maLop)
        {
            ketnoi();
            string query = @"
                SELECT 
                    sv.MaSV AS 'Mã Sinh Viên', 
                    sv.Ho AS 'Họ', 
                    sv.Ten AS 'Tên', 
                    sv.SoDienThoai AS 'Số Điện Thoại', 
                    sv.GioiTinh AS 'Giới Tính', 
                    sv.SoCMND AS 'CMND', 
                    sv.KhoaHoc AS 'Khóa Học'
                FROM SinhVien sv
                JOIN GhiDanh gd ON sv.MaSV = gd.MaSV
                WHERE gd.MaLop = @MaLop";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@MaLop", maLop);

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            // Gán dữ liệu vào DataGridView
            dgvSinhVien.DataSource = dt;
        }
        private void btChiTiet_Click(object sender, EventArgs e)
        {
            if (cbLopHoc.SelectedValue != null)
            {
                int maLop = (int)cbLopHoc.SelectedValue;
                HienThiDanhSachSinhVien(maLop);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một lớp học.");
            }
        }

        private void dgvSinhVien_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
