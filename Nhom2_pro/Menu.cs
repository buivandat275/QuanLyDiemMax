using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Nhom2_pro
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }
        private Form currentFormChild;
        private void OpenFormInPanel(Form form)
        {

            if (currentFormChild != null)
            {
                currentFormChild.Close();
            }
            currentFormChild = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panel3.Controls.Add(form);
            panel3.Tag = form;
            form.BringToFront();
            form.Show();
        }

        private void Menu_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var qlsv = new QLSV();
            OpenFormInPanel(qlsv);
            label2.Text = "Quản lý sinh viên";
            //QLSV qLSV = new QLSV();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var tlh = new TLH();
            OpenFormInPanel(tlh);
            label2.Text = "Tạo lớp học";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var ghidanh = new GhiDanhSv();
            OpenFormInPanel(ghidanh);
            label2.Text = "Ghi danh";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var qlmh = new QLMH();
            OpenFormInPanel(qlmh);
            label2.Text = "Quản lý môn học";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var qld = new QLD();
            OpenFormInPanel(qld);
            label2.Text = "Quản lý điểm";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var bcd = new BaoCaoD();
            OpenFormInPanel(bcd);
            label2.Text = "Báo cáo";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //this.Close();
            Application.Exit(); 
        }
    }
}
