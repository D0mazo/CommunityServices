using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using CommunityServices.Domain;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class MainForm : Form
    {
        private readonly User _user;
        private readonly AdminService _adminService;
        private readonly ManagerService _managerService;
        private readonly ResidentService _residentService;

        private readonly Panel topBar = new() { Dock = DockStyle.Top, Height = 60 };
        private readonly Label lblUser = new() { AutoSize = true };
        private readonly Label lblSubtitle = new() { AutoSize = true };
        private readonly Button btnLogout = new() { Width = 140, Height = 36, Text = "Atsijungti" };

        private readonly TabControl tabs = new() { Dock = DockStyle.Fill };

        public MainForm(User user, AdminService adminService, ManagerService managerService, ResidentService residentService)
        {
            _user = user;
            _adminService = adminService;
            _managerService = managerService;
            _residentService = residentService;

            Text = $"Sistema - {_user.Role} ({_user.Username})";
            Width = 1100;
            Height = 720;

            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 246, 248);
            Font = new Font("Segoe UI", 10f);
            AutoScaleMode = AutoScaleMode.Dpi;

            BuildTopBar();
            BuildTabs();

            Controls.Add(tabs);
            Controls.Add(topBar);

            BuildTabsByRole();
        }

        private void BuildTopBar()
        {
            topBar.BackColor = Color.FromArgb(250, 250, 252);
            topBar.Padding = new Padding(16, 10, 16, 10);

            topBar.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(225, 228, 233));
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
            };

            lblUser.Text = $"Prisijungęs: {_user.Role}";
            lblUser.Font = new Font("Segoe UI Semibold", 10f);
            lblUser.ForeColor = Color.FromArgb(30, 33, 37);
            lblUser.Margin = new Padding(0, 0, 0, 2);

            lblSubtitle.Text = _user.Username;
            lblSubtitle.Font = new Font("Segoe UI", 9f);
            lblSubtitle.ForeColor = Color.FromArgb(105, 112, 120);
            lblSubtitle.Margin = new Padding(0);

            StyleDangerButton(btnLogout);
            btnLogout.Click += (_, __) => Close();

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var left = new TableLayoutPanel
            {
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0),
                Anchor = AnchorStyles.Left
            };
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            left.Controls.Add(lblUser, 0, 0);
            left.Controls.Add(lblSubtitle, 0, 1);

            var rightHost = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            rightHost.Controls.Add(btnLogout);

            void CenterLogout()
            {
                btnLogout.Left = Math.Max(0, rightHost.Width - btnLogout.Width);
                btnLogout.Top = Math.Max(0, (rightHost.Height - btnLogout.Height) / 2);
            }
            rightHost.Resize += (_, __) => CenterLogout();

            layout.Controls.Add(left, 0, 0);
            layout.Controls.Add(rightHost, 1, 0);

            topBar.Controls.Clear();
            topBar.Controls.Add(layout);

            CenterLogout();
        }

        private void BuildTabs()
        {
            tabs.BackColor = Color.FromArgb(245, 246, 248);
            tabs.Padding = new Point(16, 8);

            tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabs.ItemSize = new Size(170, 40);
            tabs.SizeMode = TabSizeMode.Fixed;

            tabs.DrawItem += (_, e) =>
            {
                var tab = tabs.TabPages[e.Index];
                var selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

                var rect = e.Bounds;
                rect.Inflate(-2, -2);

                using var bg = new SolidBrush(selected ? Color.White : Color.FromArgb(250, 250, 252));
                using var pen = new Pen(Color.FromArgb(225, 228, 233));

                e.Graphics.FillRectangle(bg, rect);
                e.Graphics.DrawRectangle(pen, rect);

                var textRect = rect;
                textRect.Inflate(-10, -6);

                TextRenderer.DrawText(
                    e.Graphics,
                    tab.Text,
                    selected ? new Font("Segoe UI Semibold", 10f) : new Font("Segoe UI", 10f),
                    textRect,
                    selected ? Color.FromArgb(30, 33, 37) : Color.FromArgb(105, 112, 120),
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis
                );
            };

            tabs.ControlAdded += (_, __) =>
            {
                foreach (TabPage p in tabs.TabPages)
                {
                    p.BackColor = Color.FromArgb(245, 246, 248);
                    p.Padding = new Padding(12);
                }
            };
        }

        private void BuildTabsByRole()
        {
            tabs.TabPages.Clear();

            var actions = _user.GetAllowedActions().ToHashSet();

            if (actions.Contains("ManageCommunities"))
                tabs.TabPages.Add(WrapPage("Bendrijos", new AdminCommunitiesControl(_adminService)));

            if (actions.Contains("ManageServices"))
                tabs.TabPages.Add(WrapPage("Paslaugos", new AdminServicesControl(_adminService)));

            if (actions.Contains("ManageUsers"))
                tabs.TabPages.Add(WrapPage("Vartotojai", new AdminUsersControl(_adminService)));

            if (actions.Contains("AssignServiceToCommunity") || actions.Contains("SetServicePrices"))
                tabs.TabPages.Add(WrapPage("Priskyrimai / Kainos", new ManagerControl(_managerService)));

            if (actions.Contains("ViewMyCommunityServicesAndPrices") && _user is ResidentUser ru && ru.CommunityId != null)
                tabs.TabPages.Add(WrapPage("Mano paslaugos", new ResidentControl(_residentService, ru.CommunityId.Value)));

            if (tabs.TabPages.Count == 0)
            {
                var label = new Label
                {
                    Text = "Nėra prieinamų funkcijų.",
                    Dock = DockStyle.Top,
                    Padding = new Padding(12),
                    ForeColor = Color.FromArgb(105, 112, 120)
                };
                tabs.TabPages.Add(WrapPage("Info", label));
            }
        }

        private static TabPage WrapPage(string title, Control content)
        {
            content.Dock = DockStyle.Fill;

            var card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(12),
            };
            card.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(225, 228, 233));
                var r = card.ClientRectangle;
                r.Width -= 1; r.Height -= 1;
                e.Graphics.DrawRectangle(pen, r);
            };
            card.Controls.Add(content);

            var page = new TabPage(title)
            {
                BackColor = Color.FromArgb(245, 246, 248),
                Padding = new Padding(12)
            };
            page.Controls.Add(card);
            return page;
        }

        private static void StyleDangerButton(Button btn)
        {
            btn.AutoSize = false;
            btn.Height = 36;
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.UseCompatibleTextRendering = true;

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(220, 38, 38);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI Semibold", 10f);
            btn.Cursor = Cursors.Hand;

            btn.Padding = new Padding(6, 0, 6, 0);

            var normal = btn.BackColor;
            var hover = Color.FromArgb(239, 68, 68);
            btn.MouseEnter += (_, __) => btn.BackColor = hover;
            btn.MouseLeave += (_, __) => btn.BackColor = normal;
        }
    }
}
