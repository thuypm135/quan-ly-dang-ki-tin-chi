using Quản_Lý_Đăng_Ký_Tín_Chỉ;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quan_Ly_Dang_Ky_Tin_Chi
{
    public partial class frmMain : Form
    {
        private Form activeForm = null;

        public frmMain()
        {
            InitializeComponent();
            this.IsMdiContainer = true; // Bật chế độ MDI Container
        }

        #region SỰ KIỆN FORM LOAD & KHỞI TẠO GIAO DIỆN

        private void Main_Load(object sender, EventArgs e)
        {
            // Phóng to Form Main full màn hình
            this.WindowState = FormWindowState.Maximized;
            this.Text = "HỆ THỐNG QUẢN LÝ ĐĂNG KÝ TÍN CHỈ";

            // Định vị Header luôn nằm ở lớp ưu tiên trên cùng
            if (panelHeader != null)
            {
                panelHeader.Dock = DockStyle.Top;
                panelHeader.BringToFront();
            }

            // Cấu hình Sidebar cố định từ mép dưới Header trở xuống
            if (panelSidebar != null)
            {
                panelSidebar.Dock = DockStyle.Left;
                panelSidebar.Visible = false; // Mặc định ẩn Sidebar khi khởi chạy
            }

            // Mặc định ẩn toàn bộ các Menu con
            HideAllSubMenus();
        }

        /// <summary>
        /// Ẩn toàn bộ menu con và trả lại trạng thái mặc định cho các nút cha
        /// </summary>
        private void HideAllSubMenus()
        {
            // Nhóm 1: Hệ Thống
            if (btnDangXuat != null) btnDangXuat.Visible = false;
            if (btnThoat != null) btnThoat.Visible = false;

            // Nhóm 2: Danh Mục
            if (btnQLSinhVien != null) btnQLSinhVien.Visible = false;
            if (btnQLMonHoc != null) btnQLMonHoc.Visible = false;
            if (btnQLLopHocPhan != null) btnQLLopHocPhan.Visible = false;

            // Nhóm 3: Chức Năng
            if (btnDangKy != null) btnDangKy.Visible = false;
            if (btnNhapDiem != null) btnNhapDiem.Visible = false;

            // Nhóm 4: Thống Kê
            if (btnDSDangKy != null) btnDSDangKy.Visible = false;
            if (btnBangDiem != null) btnBangDiem.Visible = false;

            // Reset màu chữ và icon của các nút Cha về màu trắng
            ResetParentButtonState(btnHeThong, "Hệ Thống");
            ResetParentButtonState(btnDanhMuc, "Danh Mục");
            ResetParentButtonState(btnChucNang, "Chức Năng");
            ResetParentButtonState(btnThongKe, "Thống Kê");
        }

        private void ResetParentButtonState(Button btn, string originalText)
        {
            if (btn != null)
            {
                btn.ForeColor = Color.White;
                btn.Text = originalText + "         >";
            }
        }

        #endregion

        #region HÀM ĐIỀU KHIỂN SIDEBAR & HIỂN THỊ FORM CON

        // Nút góc trên bên trái Header (☰): Mở/Tắt Menu Sidebar
        private void btnMenuToggle_Click(object sender, EventArgs e)
        {
            if (panelSidebar != null)
            {
                panelSidebar.Visible = !panelSidebar.Visible;
                if (panelSidebar.Visible)
                {
                    // Cố định vị trí Y từ dưới Header xuống hết màn hình
                    int headerHeight = panelHeader != null ? panelHeader.Height : 50;
                    panelSidebar.Location = new Point(0, headerHeight);
                    panelSidebar.Height = this.ClientSize.Height - headerHeight;
                    panelSidebar.BringToFront(); // Đưa Sidebar đè lên trên vùng xám
                }
            }
        }

        /// <summary>
        /// Hàm mở Form con: Tự đóng Form cũ, lấp đầy vùng làm việc và thu gọn Sidebar
        /// </summary>
        private void OpenChildForm(Form childForm)
        {
            // 1. Đóng Form con cũ nếu đang mở
            if (activeForm != null)
            {
                activeForm.Close();
            }

            // 2. Cấu hình Form con mới
            activeForm = childForm;
            childForm.MdiParent = this;
            childForm.FormBorderStyle = FormBorderStyle.None; // Bỏ viền khung
            childForm.Dock = DockStyle.Fill;                 // Lấp đầy vùng xám

            childForm.Show();

            // 3. Tự động thu gọn Menu Sidebar sau khi chọn chức năng xong
            if (panelSidebar != null)
            {
                panelSidebar.Visible = false;
            }
        }

        #endregion

        #region XỬ LÝ ẨN/HIỆN MENU DẠNG ACCORDION (4 NÚT CHA)

        // 1. Click Nút CHA: "Hệ Thống"
        private void btnHeThong_Click(object sender, EventArgs e)
        {
            bool isOpening = (btnDangXuat != null && !btnDangXuat.Visible);
            HideAllSubMenus();

            if (isOpening)
            {
                if (btnDangXuat != null) btnDangXuat.Visible = true;
                if (btnThoat != null) btnThoat.Visible = true;
                btnHeThong.ForeColor = Color.Orange; // Đổi chữ màu cam
                btnHeThong.Text = "Hệ Thống         v"; // Ký tự xoay xuống
            }
        }

        // 2. Click Nút CHA: "Danh Mục"
        private void btnDanhMuc_Click(object sender, EventArgs e)
        {
            bool isOpening = (btnQLSinhVien != null && !btnQLSinhVien.Visible);
            HideAllSubMenus();

            if (isOpening)
            {
                if (btnQLSinhVien != null) btnQLSinhVien.Visible = true;
                if (btnQLMonHoc != null) btnQLMonHoc.Visible = true;
                if (btnQLLopHocPhan != null) btnQLLopHocPhan.Visible = true;
                btnDanhMuc.ForeColor = Color.Orange; // Đổi chữ màu cam
                btnDanhMuc.Text = "Danh Mục         v"; // Ký tự xoay xuống
            }
        }

        // 3. Click Nút CHA: "Chức Năng"
        private void btnChucNang_Click(object sender, EventArgs e)
        {
            bool isOpening = (btnDangKy != null && !btnDangKy.Visible);
            HideAllSubMenus();

            if (isOpening)
            {
                if (btnDangKy != null) btnDangKy.Visible = true;
                if (btnNhapDiem != null) btnNhapDiem.Visible = true;
                btnChucNang.ForeColor = Color.Orange; // Đổi chữ màu cam
                btnChucNang.Text = "Chức Năng         v"; // Ký tự xoay xuống
            }
        }

        // 4. Click Nút CHA: "Thống Kê"
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            bool isOpening = (btnDSDangKy != null && !btnDSDangKy.Visible);
            HideAllSubMenus();

            if (isOpening)
            {
                if (btnDSDangKy != null) btnDSDangKy.Visible = true;
                if (btnBangDiem != null) btnBangDiem.Visible = true;
                btnThongKe.ForeColor = Color.Orange; // Đổi chữ màu cam
                btnThongKe.Text = "Thống Kê         v"; // Ký tự xoay xuống
            }
        }

        #endregion

        #region XỬ LÝ SỰ KIỆN CLICK CÁC MENU CON (9 NÚT CON)

        // --- NHÓM HỆ THỐNG ---
        private void btnDangXuat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất khỏi hệ thống?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                foreach (Form child in this.MdiChildren)
                {
                    child.Close();
                }
                if (panelSidebar != null) panelSidebar.Visible = false;
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        // --- NHÓM DANH MỤC ---
        private void btnQLSinhVien_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmSinhVien());
        }

        private void btnQLMonHoc_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmMonHoc());
        }

        private void btnQLLopHocPhan_Click(object sender, EventArgs e)
        {
            OpenChildForm(new frmLopHocPhan());
        }

        // --- NHÓM CHỨC NĂNG ---
        private void btnDangKy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Đăng ký tín chỉ đang phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnNhapDiem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Nhập điểm sinh viên đang phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // --- NHÓM THỐNG KÊ ---
        private void btnDSDangKy_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Danh sách đăng ký đang phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnBangDiem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chức năng Bảng điểm sinh viên đang phát triển!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region BẮT SỰ KIỆN NÚT ĐÓNG CỦA FORM MAIN

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("Bạn có chắc chắn muốn thoát chương trình?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.No)
            {
                e.Cancel = true; // Hủy thao tác đóng form
            }
        }

        #endregion
    }
}