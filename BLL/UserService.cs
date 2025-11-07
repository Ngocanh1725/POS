using POS.DAL;
using POS.MODEL;
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
            if (password == user.PasswordHash)
            {
                return user;
            }
            else
            {
                return null;
            }
        }
    }
}
