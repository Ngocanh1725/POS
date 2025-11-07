using Microsoft.Data.SqlClient;
using POS.MODEL;
using POS.UTIL;

namespace POS.DAL
{
    public class UserRepo
    {
        public User GetUserByUserName(string username)
        {
            User user = null;
            string sql = @"SELECT u.UserID, u.Username, u.PasswordHash, u.FullName, u.RoleId, r.RoleName FROM Users u INNER JOIN Roles r ON u.RoleId = r.RoleId WHERE u.Username = @Username";

            using (SqlConnection connection = DBConnection.GetConnection())
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    try
                    {
                        connection.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                user = new User
                                {
                                    UserId = reader.GetInt32(0),
                                    Username = reader.GetString(1),
                                    PasswordHash = reader.GetString(2),
                                    FullName = reader.GetString(3),
                                    RoleId = reader.GetInt32(4),
                                    RoleName = reader.GetString(5),
                                };
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Lỗi truy vấn người dùng: \n" + ex.Message);
                    }
                }
            }
            return user;
        }
    }
}
