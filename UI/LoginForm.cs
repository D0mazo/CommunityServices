using CommunityServices.Domain;
using CommunityServices.Services;
using System;
using System.Reflection.Emit;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Collections.Generic;


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

            lblInfo.Text = "Default admin: admin / Admin123!";
            btnLogin.Click += (_, __) => DoLogin();

            AcceptButton = btnLogin;
        }

        private void DoLogin()
        {
            try
            {
                var user = _auth.Login(txtUser.Text.Trim(), txtPass.Text);
                Hide();

                var main = new MainForm(user, _adminService, _managerService, _residentService);
                main.FormClosed += (_, __) => Close();
                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
