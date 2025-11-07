using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace POS.UTIL
{
    public static class DBConnection
    {
        public static string connectionString = @"Data Source=ADMIN-PC;Initial Catalog=POS;Integrated Security=True;TrustServerCertificate=True;Connect Timeout = 30"; // Chuỗi kết nối
        public static SqlConnection GetConnection()
        {
            try
            {
                SqlConnection connection = new SqlConnection(connectionString);
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi kết nối đến csdl: \n" +  ex.Message);
            }
        }
    }
}
