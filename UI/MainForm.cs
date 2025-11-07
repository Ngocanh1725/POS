using POS.MODEL;
using System.Windows.Forms;

namespace POS.UI
{
    public partial class MainForm : Form
    {
        private User _currentUser; // Lưu thông tin người dùng đang đăng nhập

        public MainForm(User user)
        {
            InitializeComponent();
            _currentUser = user;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Hiển thị thông tin chào mừng
            statusLabelUser.Text = $"Chào, {_currentUser.FullName} ({_currentUser.RoleName})";

            // Phân quyền menu dựa trên RoleName
            AuthorizeMenu();
        }

        private void AuthorizeMenu()
        {
            // Mặc định ẩn tất cả các menu quản lý/báo cáo
            mnuManagement.Visible = false;
            mnuReports.Visible = false;

            // Phân quyền
            switch (_currentUser.RoleName)
            {
                case "Admin":
                    // Admin thấy hết
                    mnuManagement.Visible = true;
                    mnuReports.Visible = true;
                    // (Các menu con bên trong cũng cần được bật)
                    mnuProductManagement.Visible = true;
                    mnuCategoryManagement.Visible = true; // Bật danh mục
                    mnuUserManagement.Visible = true; // (Ví dụ)
                    mnuReportByStaff.Visible = true;
                    mnuReportOverall.Visible = true;
                    break;

                case "Manager":
                    // Manager thấy quản lý và báo cáo
                    mnuManagement.Visible = true;
                    mnuReports.Visible = true;
                    // Nhưng không thấy quản lý người dùng
                    mnuProductManagement.Visible = true;
                    mnuCategoryManagement.Visible = true; // Bật danh mục
                    mnuUserManagement.Visible = false; // (Ẩn ví dụ)
                    // Thấy báo cáo tổng thể
                    mnuReportByStaff.Visible = true;
                    mnuReportOverall.Visible = true;
                    break;

                case "Staff":
                    // Staff chỉ thấy bán hàng và báo cáo của cá nhân
                    mnuManagement.Visible = false; // Ẩn tab quản lý
                    mnuReports.Visible = true;
                    mnuReportByStaff.Visible = true; // Chỉ thấy báo cáo của mình
                    mnuReportOverall.Visible = false; // Ẩn báo cáo tổng
                    break;
            }
        }

        private void mnuLogout_Click(object sender, EventArgs e)
        {
            // Đóng form này
            this.Close();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            // Thoát hoàn toàn ứng dụng
            Application.Exit();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Khi form chính bị đóng (do đăng xuất hoặc thoát)
            // Cần tìm và hiển thị lại form Login
            // Cách đơn giản nhất là khởi động lại ứng dụng
            Application.Restart();
        }

        private void mnuProductManagement_Click(object sender, EventArgs e)
        {
            // Mở form quản lý sản phẩm
            // Kiểm tra xem form đã mở chưa
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(ProductForm))
                {
                    form.Activate(); // Nếu đã mở, mang lên phía trước
                    return;
                }
            }

            // Nếu chưa mở, tạo mới
            ProductForm productForm = new ProductForm();
            productForm.Show();
        }

        // CẬP NHẬT SỰ KIỆN CLICK
        private void mnuPOS_Click(object sender, EventArgs e)
        {
            // Mở form bán hàng và truyền vào user hiện tại
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(SalesForm))
                {
                    form.Activate();
                    return;
                }
            }
            SalesForm salesForm = new SalesForm(_currentUser);
            salesForm.Show();
        }

        // THÊM SỰ KIỆN CLICK MỚI
        private void mnuCategoryManagement_Click(object sender, EventArgs e)
        {
            // Mở form quản lý danh mục
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType() == typeof(CategoryForm))
                {
                    form.Activate();
                    return;
                }
            }
            CategoryForm categoryForm = new CategoryForm();
            categoryForm.Show();
        }
    }
}