using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.UTIL
{
    public static class DBConnection
    {
        // **QUAN TRỌNG: Sửa chuỗi kết nối này cho đúng với máy chủ SQL của bạn!**
        // Data Source=ADMIN-PC: Tên máy chủ SQL của bạn.
        // Initial Catalog=POS: Tên CSDL.
        // Integrated Security=True: Dùng tài khoản Windows.
        // (Nếu dùng tài khoản SQL, bạn cần thêm User ID và Password)
        public static string connectionString = @"Data Source=ADMIN-PC;Initial Catalog=POS;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=30"; // Chuỗi kết nối

        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối đến CSDL (DBConnection): \n" + ex.Message);
            }
        }

        /// <summary>
        /// Phương thức mới: Kiểm tra kết nối đến CSDL
        /// </summary>
        /// <returns>True nếu kết nối thành công, False nếu thất bại.</returns>
        public static bool CheckConnection()
        {
            using (SqlConnection connection = GetConnection())
            {
                try
                {
                    connection.Open();
                    return true; // Kết nối thành công
                }
                catch (Exception)
                {
                    return false; // Kết nối thất bại
                }
            }
        }
    }
}