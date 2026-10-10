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

namespace Quan_Ly_Dang_Ky_Tin_Chi
{
    public partial class frmMonHoc : Form
    {
        // Chuỗi kết nối CSDL (Thay đổi tên Server cho đúng với máy của bạn)
        private string connectionString = @"Data Source=DESKTOP-13A3T4U\SQLEXPRESS;Initial Catalog=QL_TinChi;Integrated Security=True;TrustServerCertificate=True";
        private bool isAdding = false; // Biến kiểm tra đang ở chế độ Thêm hay Sửa

        public frmMonHoc()
        {
            InitializeComponent();
        }

        private void frmMonHoc_Load(object sender, EventArgs e)
        {
            LoadData();
            SetControlState(false); // Ban đầu khóa các ô nhập liệu
        }

        #region HÀM BỔ TRỢ GIAO DIỆN & CSDL

        // Hàm tải dữ liệu từ bảng MONHOC lên DataGridView
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaMon AS [Mã Môn], TenMon AS [Tên Môn Học], SoTinChi AS [Số Tín Chỉ], SoTietLT AS [Số Tiết LT], SoTietTH AS [Số Tiết TH] FROM MONHOC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvMonHoc.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi kết nối cơ sở dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Hàm bật/tắt trạng thái các nút bấm và ô nhập liệu
        private void SetControlState(bool editing)
        {
            txtMaMon.Enabled = editing && isAdding; // Chỉ cho nhập Mã MH khi Thêm mới
            txtTenMon.Enabled = editing;
            nudSoTinChi.Enabled = editing;
            nudSotietLT.Enabled = editing;
            nudSotietTH.Enabled = editing;

            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;
            btnLuu.Enabled = editing;
            btnHuy.Enabled = editing;
        }

        // Hàm xóa trắng ô nhập liệu
        private void ClearInput()
        {
            txtMaMon.Clear();
            txtTenMon.Clear();
            nudSoTinChi.Value = 0;
            nudSotietLT.Value = 0;
            nudSotietTH.Value = 0;
        }

        #endregion

        #region XỬ LÝ SỰ KIỆN NÚT BẤM (CRUD)

        // 1. Click Nút THÊM
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            ClearInput();
            SetControlState(true);
            txtMaMon.Focus();
        }

        // 2. Click Nút SỬA
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn môn học cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isAdding = false;
            SetControlState(true);
        }

        // 3. Click Nút XÓA
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvMonHoc.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn môn học cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maMH = txtMaMon.Text;
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa môn học [{maMH}] không?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM MONHOC WHERE MaMH = @MaMH";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaMH", maMH);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xóa môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ClearInput();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể xóa môn học này (có thể đã có dữ liệu ràng buộc đăng ký): " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // 4. Click Nút LƯU
        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra ràng buộc dữ liệu
            if (string.IsNullOrWhiteSpace(txtMaMon.Text) || string.IsNullOrWhiteSpace(txtTenMon.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã môn học và Tên môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (isAdding) // Thêm mới
                    {
                        cmd.CommandText = "INSERT INTO MONHOC (MaMH, TenMH, SoTinChi, SoTietLT, SoTietTH) VALUES (@MaMH, @TenMH, @SoTinChi, @SoTietLT, @SoTietTH)";
                    }
                    else // Cập nhật (Sửa)
                    {
                        cmd.CommandText = "UPDATE MONHOC SET TenMH = @TenMH, SoTinChi = @SoTinChi, SoTietLT = @SoTietLT, SoTietTH = @SoTietTH WHERE MaMH = @MaMH";
                    }

                    cmd.Parameters.AddWithValue("@MaMH", txtMaMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@TenMH", txtTenMon.Text.Trim());
                    cmd.Parameters.AddWithValue("@SoTinChi", nudSoTinChi.Value);
                    cmd.Parameters.AddWithValue("@SoTietLT", nudSotietLT.Value);
                    cmd.Parameters.AddWithValue("@SoTietTH", nudSotietTH.Value);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(isAdding ? "Thêm mới môn học thành công!" : "Cập nhật môn học thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    SetControlState(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 5. Click Nút HỦY
        private void btnHuy_Click(object sender, EventArgs e)
        {
            ClearInput();
            SetControlState(false);
        }

        // 6. Click Nút THOÁT
        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sự kiện khi bấm vào 1 dòng trên DataGridView -> Đổ dữ liệu lên ô nhập
        private void dgvMonHoc_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvMonHoc.Rows[e.RowIndex];
                txtMaMon.Text = row.Cells["Mã Môn"].Value?.ToString();
                txtTenMon.Text = row.Cells["Tên Môn Học"].Value?.ToString();

                if (decimal.TryParse(row.Cells["Số Tín Chỉ"].Value?.ToString(), out decimal stc))
                    nudSoTinChi.Value = stc;
                if (decimal.TryParse(row.Cells["Số Tiết LT"].Value?.ToString(), out decimal lt))
                    nudSotietLT.Value = lt;
                if (decimal.TryParse(row.Cells["Số Tiết TH"].Value?.ToString(), out decimal th))
                    nudSotietTH.Value = th;
            }
        }

        #endregion
    }
}