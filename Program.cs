using POS.UI;
using POS.UTIL;
using System.Windows.Forms;

namespace POS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            // 1. Kiểm tra kết nối CSDL trước khi chạy
            if (!DBConnection.CheckConnection())
            {
                // Thông báo lỗi nếu không kết nối được
                MessageBox.Show(
                    "Không thể kết nối đến cơ sở dữ liệu.\n" +
                    "Vui lòng kiểm tra chuỗi kết nối trong (UTIL/DBConnection.cs) hoặc đảm bảo máy chủ SQL đang chạy.",
                    "Lỗi Kết Nối CSDL",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return; // Dừng ứng dụng
            }

            ApplicationConfiguration.Initialize();

            // ===================================================================
            // ĐÃ DỌN DẸP:
            // Hoàn tác lại, chạy LoginForm (Form đăng nhập)
            // ===================================================================
            Application.Run(new LoginForm());

            // (Bạn có thể xóa dòng 'Application.Run(new HashToolForm());' cũ)
        }
    }
}