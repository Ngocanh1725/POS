using POS.DAL;
using POS.MODEL;
using POS.UTIL; // Thêm namespace UTIL
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS.BLL
{
    public class UserService
    {
        private UserRepo _userRepo;
        public UserService()
        {
            _userRepo = new UserRepo();
        }

        public User AuthenticatedUser(string username, string password)
        {
            User user = _userRepo.GetUserByUserName(username);

            if (user == null)
            {
                return null; // Không tồn tại người dùng            
            }

            // **CẬP NHẬT QUAN TRỌNG VỀ BẢO MẬT**
            // Không bao giờ so sánh mật khẩu trực tiếp.
            // Sử dụng PasswordHasher để xác minh.
            if (PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return user; // Mật khẩu khớp
            }
            else
            {
                return null; // Mật khẩu sai
            }
        }
    }
}