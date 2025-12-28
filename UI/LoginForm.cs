using CommunityServices.Domain;
using CommunityServices.Services;
using System;
using System.Windows.Forms;

namespace CommunityServices.UI
{
    public class LoginForm : Form
    {
        private readonly AuthService _auth;
        private readonly AdminService _adminService;
        private readonly ManagerService _managerService;
        private readonly ResidentService _residentService;

        private TextBox txtUser = new() { Left = 20, Top = 20, Width = 240 };
        private TextBox txtPass = new() { Left = 20, Top = 60, Width = 240, UseSystemPasswordChar = true };
        private Button btnLogin = new() { Left = 20, Top = 100, Width = 240, Text = "Prisijungti" };
        private Label lblInfo = new() { Left = 20, Top = 140, Width = 360, Height = 60 };

        public LoginForm(AuthService auth, AdminService adminService, ManagerService managerService, ResidentService residentService)
        {
            _auth = auth;
            _adminService = adminService;
            _managerService = managerService;
            _residentService = residentService;

            Text = "Prisijungimas";
            Width = 420;
            Height = 260;
            Controls.AddRange(new Control[] { txtUser, txtPass, btnLogin, lblInfo });

            lblInfo.Text = "Default admin: admin/Admin123";
            btnLogin.Click += (_, __) => DoLogin();

            AcceptButton = btnLogin;
        }

        private void DoLogin()
        {
            try
            {
                var user = _auth.Login(txtUser.Text.Trim(), txtPass.Text.Trim());

                // Paslepiam login, bet programos neuždarom
                Hide();

                var main = new MainForm(user, _adminService, _managerService, _residentService);

                // Kai MainForm užsidaro (Logout arba X), grįžtam į LoginForm
                main.FormClosed += (_, __) =>
                {
                    txtPass.Text = "";      // saugiau
                    // txtUser.Text = "";   // jei nori, kad išvalytų ir username
                    Show();
                    Activate();
                };

                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
