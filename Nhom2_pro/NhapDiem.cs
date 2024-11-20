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
    public partial class NhapDiem : Form
    {
        private string maMonHoc;
        private string maLop;
        string ketnoi = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";

        public NhapDiem(string maLop, string maMonHoc)
        {
            InitializeComponent();
            this.maLop = maLop;
            this.maMonHoc = maMonHoc;
        }
        private void NhapDiem_Load(object sender, EventArgs e)
        {
            txtmamonhoc.Text = this.maMonHoc;
            txtMaLop.Text = this.maLop;
            LoadStudentData();
        }
        private void LoadStudentData()
        {
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                try
                {
                    string query = @"
            SELECT 
                sv.MaSV, sv.Ho, sv.Ten, sv.GioiTinh,
                lh.TenLop, gd.SoBuoiVang
            FROM SinhVien sv
            INNER JOIN GhiDanh gd ON sv.MaSV = gd.MaSV
                  JOIN LopHoc lh ON gd.MaLop = lh.MaLop
            WHERE gd.MaLop = @MaLop";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@MaLop", maLop);

                    connection.Open();

                    // Thực thi câu lệnh SQL
                    SqlDataReader reader = command.ExecuteReader();

                    // Xóa toàn bộ dữ liệu trong DataGridView trước khi thêm mới
                    dataGridView1.Rows.Clear();

                    // Đọc dữ liệu và thêm vào DataGridView
                    int stt = 1;
                    while (reader.Read())
                    {
                        int rowIndex = dataGridView1.Rows.Add();
                        DataGridViewRow row = dataGridView1.Rows[rowIndex];

                        row.Cells["STT"].Value = stt++; // Số thứ tự
                        row.Cells["masv"].Value = reader["MaSV"].ToString(); // Mã sinh viên
                        row.Cells["hodem"].Value = reader["Ho"].ToString(); // Họ
                        row.Cells["ten"].Value = reader["Ten"].ToString(); // Tên
                        row.Cells["gioitinh"].Value = reader["GioiTinh"].ToString(); // Giới tính
                        row.Cells["lophoc"].Value = reader["TenLop"].ToString();
                        
                        // Tính 'DiemChuyenCan' dựa trên 'SoBuoiVang'
                        int soBuoiVang = reader["SoBuoiVang"] != DBNull.Value ? Convert.ToInt32(reader["SoBuoiVang"]) : 0;
                        float diemChuyenCan = CalculateAttendanceScore(soBuoiVang);
                        row.Cells["chuyencan"].Value = diemChuyenCan;

                    }
                    // Cho phép chỉnh sửa các cột điểm
                    dataGridView1.Columns["diem1"].ReadOnly = false;
                    dataGridView1.Columns["diem2"].ReadOnly = false;
                    dataGridView1.Columns["diem3"].ReadOnly = false;
                    dataGridView1.Columns["diem4"].ReadOnly = false;
                    dataGridView1.Columns["diemthuchanh"].ReadOnly = false;
                    dataGridView1.Columns["diemthi"].ReadOnly = false;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi load dữ liệu: " + ex.Message);
                }
            }
        }
        // Hàm lấy danh sách sinh viên và loại điểm
        private void LoadDiemSinhVien(int maLop)
        {
            using (SqlConnection conn = new SqlConnection(ketnoi))
            {
                string query = @"
            SELECT sv.MaSV, sv.Ho + ' ' + sv.Ten AS TenSinhVien, ld.MaLoaiDiem, ld.TenLoai, ld.TiLePhanTram
            FROM SinhVien sv
            JOIN GhiDanh gd ON sv.MaSV = gd.MaSV
            JOIN LopHoc lh ON gd.MaLop = lh.MaLop
            JOIN LoaiDiem ld ON lh.MaMonHoc = ld.MaMonHoc
            WHERE lh.MaLop = @MaLop AND sv.DaXoa = 0";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@MaLop", maLop);

                DataTable dt = new DataTable();
                adapter.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }

        private float CalculateAttendanceScore(int soBuoiVang)
        {
            // Ví dụ tính điểm: giả sử điểm chuyên cần tối đa là 10
            // Mỗi buổi vắng giảm điểm chuyên cần đi 0.5
            float maxScore = 10;
            float deductionPerAbsence = 0.5f;
            return Math.Max(0, maxScore - (soBuoiVang * deductionPerAbsence));
        }
       /* private void CalculateScores()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                // Lấy điểm chuyên cần từ cột "diemchuyencan"
                float diemChuyenCan = Convert.ToSingle(row.Cells["chuyencan"].Value);

                // Lấy các điểm khác để tính TBTK
                float diem1He1 = Convert.ToSingle(row.Cells["diem1"].Value);
                float diem2He1 = Convert.ToSingle(row.Cells["diem2"].Value);
                float diem1He2 = Convert.ToSingle(row.Cells["diem3"].Value);
                float diem2He2 = Convert.ToSingle(row.Cells["diem4"].Value);
                float diemThi = Convert.ToSingle(row.Cells["diemthi"].Value);

                // Tính TBTK
                float tbtk = (diem1He1 + diem2He1 + diem1He2 + diem2He2) / 4;
                row.Cells["diemtbtk"].Value = tbtk;

                // Tính Tổng điểm với trọng số mới: 30% điểm chuyên cần, 70% cho TBTK và điểm thi
                float totalScore = diemChuyenCan * 0.3f + (tbtk * 0.35f + diemThi * 0.35f);
                row.Cells["diemtong"].Value = totalScore;
            }
            MessageBox.Show("Đã tính điểm chuyên cần và tổng điểm!", "Thông báo");
        }*/

        private void btntinhvaluu_Click(object sender, EventArgs e)
        {
            // CalculateScores();
            float totalPercentage = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["TiLePhanTram"].Value != null)
                {
                    totalPercentage += float.Parse(row.Cells["TiLePhanTram"].Value.ToString());
                }
            }

            if (totalPercentage != 100)
            {
                MessageBox.Show("Tổng tỷ lệ phần trăm các loại điểm phải bằng 100!", "Thông báo");
                return;
            }

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["DiemSo"].Value != null && row.Cells["TiLePhanTram"].Value != null)
                {
                    float diemSo = float.Parse(row.Cells["DiemSo"].Value.ToString());
                    float tiLe = float.Parse(row.Cells["TiLePhanTram"].Value.ToString());
                    float diemTinh = diemSo * (tiLe / 100);

                    row.Cells["DiemTinh"].Value = diemTinh; // Cột này cần thêm vào DataGridView
                }
            }

            MessageBox.Show("Tính toán hoàn tất!", "Thông báo");
        }

        private void btLuuDiem_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(ketnoi))
            {
                conn.Open();
                SqlTransaction transaction = conn.BeginTransaction();

                try
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.Cells["DiemSo"].Value != null)
                        {
                            int maSV = Convert.ToInt32(row.Cells["MaSV"].Value);
                            int maLoaiDiem = Convert.ToInt32(row.Cells["MaLoaiDiem"].Value);
                            float diemSo = float.Parse(row.Cells["DiemSo"].Value.ToString());

                            string query = @"
                        IF EXISTS (SELECT * FROM Diem WHERE MaSV = @MaSV AND MaLoaiDiem = @MaLoaiDiem)
                        BEGIN
                            UPDATE Diem
                            SET DiemSo = @DiemSo
                            WHERE MaSV = @MaSV AND MaLoaiDiem = @MaLoaiDiem
                        END
                        ELSE
                        BEGIN
                            INSERT INTO Diem (MaSV, MaLoaiDiem, DiemSo)
                            VALUES (@MaSV, @MaLoaiDiem, @DiemSo)
                        END";

                            SqlCommand cmd = new SqlCommand(query, conn, transaction);
                            cmd.Parameters.AddWithValue("@MaSV", maSV);
                            cmd.Parameters.AddWithValue("@MaLoaiDiem", maLoaiDiem);
                            cmd.Parameters.AddWithValue("@DiemSo", diemSo);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    MessageBox.Show("Lưu điểm thành công!", "Thông báo");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    MessageBox.Show($"Có lỗi xảy ra: {ex.Message}", "Thông báo");
                }
            }
        }
    }
}
