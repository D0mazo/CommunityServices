using CommunityServices.Domain;
using CommunityServices.Services;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CommunityServices.UI
{
    public class LoginForm : Form
    {
        private readonly AuthService _auth;
        private readonly AdminService _adminService;
        private readonly ManagerService _managerService;
        private readonly ResidentService _residentService;

        private TextBox txtUser;
        private TextBox txtPass;
        private Button btnLogin;
        private Label lblInfo;
        private Label lblTitle;

        public LoginForm(
            AuthService auth,
            AdminService adminService,
            ManagerService managerService,
            ResidentService residentService)
        {
            _auth = auth;
            _adminService = adminService;
            _managerService = managerService;
            _residentService = residentService;

            BuildForm();
            BuildControls();
            LayoutControls();
        }

        private void BuildForm()
        {
            Text = "Prisijungimas";
            Width = 460;
            Height = 340;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10f);
            AutoScaleMode = AutoScaleMode.Dpi;
        }

        private void BuildControls()
        {
            lblTitle = new Label
            {
                Text = "Sistema",
                Font = new Font("Segoe UI Semibold", 16f),
                ForeColor = Color.FromArgb(30, 33, 37),
                AutoSize = true
            };

            txtUser = new TextBox
            {
                PlaceholderText = "Vartotojo vardas",
                Width = 260
            };

            txtPass = new TextBox
            {
                PlaceholderText = "Slaptažodis",
                UseSystemPasswordChar = true,
                Width = 260
            };

            btnLogin = new Button
            {
                Text = "Prisijungti",
                Width = 260,
                Height = 36,
                BackColor = Color.FromArgb(37, 99, 235),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 10f),
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;

            // hover effect
            btnLogin.MouseEnter += (_, __) =>
                btnLogin.BackColor = Color.FromArgb(59, 130, 246);
            btnLogin.MouseLeave += (_, __) =>
                btnLogin.BackColor = Color.FromArgb(37, 99, 235);

            btnLogin.Click += (_, __) => DoLogin();

            lblInfo = new Label
            {
                //Text = "Default admin: admin / Admin123",
                ForeColor = Color.FromArgb(105, 112, 120),
                AutoSize = true
            };

            AcceptButton = btnLogin;
        }

        private void LayoutControls()
        {
            var card = new Panel
            {
                Width = 320,
                Height = 240,
                BackColor = Color.White,
                Padding = new Padding(24)
            };

            card.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(225, 228, 233));
                var r = card.ClientRectangle;
                r.Width -= 1;
                r.Height -= 1;
                e.Graphics.DrawRectangle(pen, r);
            };

            lblTitle.Location = new Point(24, 16);
            txtUser.Location = new Point(24, 64);
            txtPass.Location = new Point(24, 104);
            btnLogin.Location = new Point(24, 148);
            lblInfo.Location = new Point(24, 196);

            card.Controls.Add(lblTitle);
            card.Controls.Add(txtUser);
            card.Controls.Add(txtPass);
            card.Controls.Add(btnLogin);
            card.Controls.Add(lblInfo);

            // center card
            card.Left = (ClientSize.Width - card.Width) / 2;
            card.Top = (ClientSize.Height - card.Height) / 2;

            Controls.Add(card);

            Resize += (_, __) =>
            {
                card.Left = (ClientSize.Width - card.Width) / 2;
                card.Top = (ClientSize.Height - card.Height) / 2;
            };
        }

        private void DoLogin()
        {
            try
            {
                var user = _auth.Login(
                    txtUser.Text.Trim(),
                    txtPass.Text.Trim());

                Hide();

                var main = new MainForm(
                    user,
                    _adminService,
                    _managerService,
                    _residentService);

                main.FormClosed += (_, __) =>
                {
                    txtPass.Text = "";
                    Show();
                    Activate();
                };

                main.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Klaida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
