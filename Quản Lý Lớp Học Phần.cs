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
    public partial class frmLopHocPhan : Form
    {
        // Chuỗi kết nối CSDL
        private string connectionString = @"Data Source=DESKTOP-13A3T4U\SQLEXPRESS;Initial Catalog=QL_TinChi;Integrated Security=True;TrustServerCertificate=True";
        private bool isAdding = false;

        public frmLopHocPhan()
        {
            InitializeComponent();
        }

        private void frmLopHocPhan_Load(object sender, EventArgs e)
        {
            LoadComboBoxMonHoc();
            LoadComboBoxGiangVien(); // Đã bổ sung hàm nạp danh sách Giảng viên
            LoadComboBoxHocKy();
            LoadData();
            SetControlState(false);
        }

        #region HÀM BỔ TRỢ & TẢI DỮ LIỆU

        // Tải danh sách Giảng viên lên ComboBox
        private void LoadComboBoxGiangVien()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaGV, HoTen FROM GIANGVIEN";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboGiangVien.DataSource = dt;
                    cboGiangVien.DisplayMember = "HoTen"; // Hiển thị Họ Tên
                    cboGiangVien.ValueMember = "MaGV";     // Giá trị ẩn là Mã GV
                    cboGiangVien.SelectedIndex = -1;       // Mặc định chưa chọn
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách giảng viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Tải danh sách Môn học lên ComboBox
        private void LoadComboBoxMonHoc()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaMon, TenMon FROM MONHOC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboMonHoc.DataSource = dt;
                    cboMonHoc.DisplayMember = "TenMon"; // Hiển thị Tên môn
                    cboMonHoc.ValueMember = "MaMon";    // Giá trị ẩn là Mã môn
                    cboMonHoc.SelectedIndex = -1;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải danh sách môn học: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Khởi tạo danh sách Học kỳ
        private void LoadComboBoxHocKy()
        {
            cboHocKy.Items.Clear();
            cboHocKy.Items.Add("1");
            cboHocKy.Items.Add("2");
            cboHocKy.Items.Add("3");
            cboHocKy.SelectedIndex = 0;
        }

        // Tải danh sách Lớp học phần lên DataGridView
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT lhp.MaLHP AS [Mã LHP], 
                                           mh.TenMon AS [Môn Học], 
                                           lhp.HocKy AS [Học Kỳ], 
                                           lhp.NamHoc AS [Năm Học], 
                                           gv.HoTen AS [Giảng Viên],
                                           lhp.SoLuongMax AS [Sĩ Số Tối Đa],
                                           lhp.MaMon,
                                           lhp.MaGV
                                    FROM LOPHOCPHAN lhp
                                    INNER JOIN MONHOC mh ON lhp.MaMon = mh.MaMon
                                    LEFT JOIN GIANGVIEN gv ON lhp.MaGV = gv.MaGV";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvLopHocPhan.DataSource = dt;

                    // Ẩn các cột mã hệ thống
                    if (dgvLopHocPhan.Columns["MaMon"] != null) dgvLopHocPhan.Columns["MaMon"].Visible = false;
                    if (dgvLopHocPhan.Columns["MaGV"] != null) dgvLopHocPhan.Columns["MaGV"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi tải dữ liệu lớp học phần: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Bật/tắt trạng thái nhập liệu
        private void SetControlState(bool editing)
        {
            txtMaLHP.Enabled = editing && isAdding;
            cboGiangVien.Enabled = editing;
            cboMonHoc.Enabled = editing;
            cboHocKy.Enabled = editing;
            txtNamHoc.Enabled = editing;
            nudSiSoToiDa.Enabled = editing;

            btnThem.Enabled = !editing;
            btnSua.Enabled = !editing;
            btnXoa.Enabled = !editing;
            btnLuu.Enabled = editing;
            btnHuy.Enabled = editing;
        }

        private void ClearInput()
        {
            txtMaLHP.Clear();
            cboGiangVien.SelectedIndex = -1;
            cboMonHoc.SelectedIndex = -1;
            cboHocKy.SelectedIndex = 0;
            txtNamHoc.Clear();
            nudSiSoToiDa.Value = 40;
        }

        #endregion

        #region XỬ LÝ NÚT BẤM (CRUD)

        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true;
            ClearInput();
            SetControlState(true);
            txtMaLHP.Focus();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvLopHocPhan.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần cần sửa trên bảng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isAdding = false;
            SetControlState(true); // Bật các ô cho phép sửa
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLopHocPhan.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn lớp học phần cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string maLHP = txtMaLHP.Text;
            DialogResult dr = MessageBox.Show($"Bạn có chắc chắn muốn xóa lớp [{maLHP}] không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "DELETE FROM LOPHOCPHAN WHERE MaLHP = @MaLHP";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@MaLHP", maLHP);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show("Xóa lớp học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                        ClearInput();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Không thể xóa (có thể đã có sinh viên đăng ký lớp này): " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            // Kiểm tra rỗng dữ liệu trước khi lưu
            if (string.IsNullOrWhiteSpace(txtMaLHP.Text) || string.IsNullOrWhiteSpace(txtNamHoc.Text) || cboMonHoc.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Mã LHP, Năm học và chọn Môn học!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;

                    if (isAdding)
                    {
                        cmd.CommandText = @"INSERT INTO LOPHOCPHAN (MaLHP, MaMon, HocKy, NamHoc, MaGV, SoLuongMax) 
                                            VALUES (@MaLHP, @MaMon, @HocKy, @NamHoc, @MaGV, @SoLuongMax)";
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE LOPHOCPHAN 
                                            SET MaMon = @MaMon, HocKy = @HocKy, NamHoc = @NamHoc, MaGV = @MaGV, SoLuongMax = @SoLuongMax 
                                            WHERE MaLHP = @MaLHP";
                    }

                    cmd.Parameters.AddWithValue("@MaLHP", txtMaLHP.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaMon", cboMonHoc.SelectedValue.ToString());
                    cmd.Parameters.AddWithValue("@HocKy", cboHocKy.Text.Trim());
                    cmd.Parameters.AddWithValue("@NamHoc", txtNamHoc.Text.Trim());
                    cmd.Parameters.AddWithValue("@MaGV", cboGiangVien.SelectedValue != null ? cboGiangVien.SelectedValue.ToString() : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@SoLuongMax", nudSiSoToiDa.Value);

                    cmd.ExecuteNonQuery();

                    MessageBox.Show(isAdding ? "Thêm lớp học phần thành công!" : "Cập nhật lớp học phần thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    SetControlState(false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHuy_Click(object sender, EventArgs e)
        {
            ClearInput();
            SetControlState(false);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvLopHocPhan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvLopHocPhan.Rows[e.RowIndex];

                // 1. Điền Mã LHP và Năm Học
                txtMaLHP.Text = row.Cells["Mã LHP"].Value?.ToString();
                txtNamHoc.Text = row.Cells["Năm Học"].Value?.ToString();

                // 2. Chọn Học Kỳ
                if (row.Cells["Học Kỳ"].Value != null)
                    cboHocKy.Text = row.Cells["Học Kỳ"].Value.ToString();

                // 3. Chọn Môn Học (Gán theo Mã Môn ẩn)
                if (row.Cells["MaMon"].Value != null)
                    cboMonHoc.SelectedValue = row.Cells["MaMon"].Value.ToString();

                // 4. Chọn Giảng Viên (Gán theo Mã GV ẩn)
                if (row.Cells["MaGV"].Value != DBNull.Value && row.Cells["MaGV"].Value != null)
                {
                    cboGiangVien.SelectedValue = row.Cells["MaGV"].Value.ToString();
                }
                else
                {
                    cboGiangVien.SelectedIndex = -1;
                }

                // 5. Điền Sĩ Số Tối Đa
                if (decimal.TryParse(row.Cells["Sĩ Số Tối Đa"].Value?.ToString(), out decimal siSo))
                    nudSiSoToiDa.Value = siSo;
            }
        }

        #endregion
    }
}