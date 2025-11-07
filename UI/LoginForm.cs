using POS.BLL;
using POS.MODEL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS.UI
{
    public partial class LoginForm : Form
    {
        UserService _userService;
        public LoginForm()
        {
            InitializeComponent();
            _userService = new UserService();

        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();
            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Tên không được để trống");
                txtUsername.Focus();
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Mật khẩu không được để trống");
                txtPassword.Focus();
                return;
            }    
            try
            {
                User authenticatedUser = _userService.AuthenticatedUser(username, password);
                if (authenticatedUser != null)
                {
                    MessageBox.Show("Chào mừng " + authenticatedUser.FullName + "! đăng nhập thành công");
                    new Form1().Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Đăng nhập thất bại");
                    this.txtUsername.Focus();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Tên truy cập hoặc mật khẩu không đúng.");
            }

        }
    }
}
