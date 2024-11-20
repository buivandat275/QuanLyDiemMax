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
    public partial class QLD : Form
    {
        public QLD()
        {
            InitializeComponent();
        }

        string ketnoi = "Data Source=DESKTOP-49O1M0L\\SQLEXPRESS;Initial Catalog=QuanLySinhVien;Integrated Security=True;";
        private void LoadDataToListView()
        {
            // Xóa dữ liệu cũ trong ListView
            lvthongtindiem.Items.Clear();

            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                string query = "SELECT MaMonHoc, TenMon, SoTinChi FROM MonHoc";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Lấy dữ liệu từ các cột trong cơ sở dữ liệu
                            string maMonHoc = reader["MaMonHoc"].ToString();
                            string tenMonHoc = reader["TenMon"].ToString();
                            string soTinChi = reader["SoTinChi"].ToString();

                            // Tạo một ListViewItem với MaMonHoc làm giá trị chính
                            ListViewItem item = new ListViewItem(maMonHoc);

                            // Thêm các cột con cho ListViewItem
                            item.SubItems.Add(tenMonHoc);
                            item.SubItems.Add(soTinChi);

                            // Thêm ListViewItem vào ListView
                            lvthongtindiem.Items.Add(item);
                        }
                    }
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }
        }

        private void QLD_Load(object sender, EventArgs e)
        {
            //LoadDataToListView();
            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                string query = "SELECT TenLop FROM LopHoc WHERE DaXoa = 0";
                SqlCommand command = new SqlCommand(query, connection);

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    cbblophoc.Items.Clear();
                    while (reader.Read())
                    {
                        string displayText = $"{reader["TenLop"]}";
                        cbblophoc.Items.Add(displayText);
                    }
                }
            }
        }

       

        private void btnxemdiem_Click(object sender, EventArgs e)
        {

        }

        private void lvthongtindiem_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Kiểm tra xem người dùng đã chọn mục nào trong ListView hay chưa
            if (lvthongtindiem.SelectedItems.Count > 0)
            {
                // Lấy mã môn học từ mục được chọn
                maMonHocSelected = lvthongtindiem.SelectedItems[0].Text;

            }
        }

        private void btntimkiem_Click(object sender, EventArgs e)
        {

        }

        private void btntimkiem_Click_1(object sender, EventArgs e)
        {

            string maLop = txttkmalop.Text.Trim(); // Lấy mã lớp từ TextBox
            string tenLop = cbblophoc.SelectedItem?.ToString(); // Lấy tên lớp từ ComboBox

            // Kiểm tra trường hợp không nhập đủ thông tin
            if (string.IsNullOrEmpty(maLop) && string.IsNullOrEmpty(tenLop))
            {
                MessageBox.Show("Vui lòng nhập mã lớp hoặc chọn tên lớp để tìm kiếm.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection connection = new SqlConnection(ketnoi))
            {
                connection.Open();

                // Tạo câu truy vấn SQL
                string query = @"
                        SELECT mh.MaMonHoc, mh.TenMon, mh.SoTinChi, lh.SoLuongSV, hk.TenHocKy, lh.MaLop, lh.TenLop
                        FROM LopHoc lh
                        JOIN LopHoc_MonHoc lhmh ON lh.MaLop = lhmh.MaLop
                        JOIN MonHoc mh ON lhmh.MaMonHoc = mh.MaMonHoc
                        JOIN HocKy hk ON lh.MaHocKy = hk.MaHocKy
                        WHERE 1=1";

                // Thêm điều kiện tìm kiếm
                if (!string.IsNullOrEmpty(maLop))
                {
                    query += " AND lh.MaLop = @MaLop";
                }
                if (!string.IsNullOrEmpty(tenLop))
                {
                    query += " AND lh.TenLop = @TenLop";
                }

                SqlCommand command = new SqlCommand(query, connection);

                // Gắn tham số cho truy vấn
                if (!string.IsNullOrEmpty(maLop))
                {
                    command.Parameters.AddWithValue("@MaLop", maLop);
                }
                if (!string.IsNullOrEmpty(tenLop))
                {
                    command.Parameters.AddWithValue("@TenLop", tenLop);
                }

                try
                {
                    // Thực thi truy vấn và lấy dữ liệu
                    SqlDataReader reader = command.ExecuteReader();

                    // Xóa dữ liệu cũ trong ListView
                    lvthongtindiem.Items.Clear();

                    bool hasData = false; // Cờ để kiểm tra nếu có dữ liệu trả về

                    while (reader.Read())
                    {
                        hasData = true;

                        // Lấy thông tin môn học
                        string maMonHoc = reader["MaMonHoc"].ToString();
                        string tenMon = reader["TenMon"].ToString();
                        string soTinChi = reader["SoTinChi"].ToString();

                        // Lấy thông tin lớp học và học kỳ
                        txtsosv.Text = reader["SoLuongSV"].ToString();
                        txthocky.Text = reader["TenHocKy"].ToString();

                        // Hiển thị thông tin lớp còn thiếu
                        if (string.IsNullOrEmpty(maLop))
                        {
                            txttkmalop.Text = reader["MaLop"].ToString();
                        }
                        if (string.IsNullOrEmpty(tenLop))
                        {
                            cbblophoc.SelectedItem = reader["TenLop"].ToString();
                        }

                        // Tạo một ListViewItem
                        ListViewItem item = new ListViewItem(maMonHoc);
                        item.SubItems.Add(tenMon);
                        item.SubItems.Add(soTinChi);

                        // Thêm item vào ListView
                        lvthongtindiem.Items.Add(item);
                    }

                    if (!hasData)
                    {
                        MessageBox.Show("Không tìm thấy dữ liệu phù hợp.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtsosv.Clear();
                        txthocky.Clear();
                        txttkmalop.Clear();
                        cbblophoc.SelectedIndex = -1;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tìm kiếm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }


        }
        private string maMonHocSelected = null;
        private void btnnhapdiem_Click(object sender, EventArgs e)
        {
           if (lvthongtindiem.SelectedItems.Count > 0)
    {
        string maLop = txttkmalop.Text.Trim(); // Lấy mã lớp từ textbox
                string maMonHoc = lvthongtindiem.SelectedItems[0].SubItems[0].Text; // Lấy mã môn học được chọn

        if (!string.IsNullOrEmpty(maLop) && !string.IsNullOrEmpty(maMonHoc))
        {
            // Chuyển sang form Nhập điểm
            NhapDiem formNhapDiem = new NhapDiem(maLop, maMonHoc);
            formNhapDiem.ShowDialog();
        }
        else
        {
            MessageBox.Show("Vui lòng chọn đầy đủ lớp và môn học.", "Thông báo");
        }
    }
    else
    {
        MessageBox.Show("Vui lòng chọn một môn học trong danh sách.", "Thông báo");
    }
        }

        private void cbblophoc_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void txttkmalop_TextChanged(object sender, EventArgs e)
        {
            
        }
    }
}
