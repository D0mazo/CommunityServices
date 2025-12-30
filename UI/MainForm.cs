using System;
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

        private Panel topBar = new() { Dock = DockStyle.Top, Height = 44 };
        private Label lblUser = new() { Left = 10, Top = 12, AutoSize = true };
        private Button btnLogout = new() { Left = 0, Top = 8, Width = 140, Height = 28, Text = "Atsijungti" };

        private TabControl tabs = new() { Dock = DockStyle.Fill };

        public MainForm(User user, AdminService adminService, ManagerService managerService, ResidentService residentService)
        {
            _user = user;
            _adminService = adminService;
            _managerService = managerService;
            _residentService = residentService;

            Text = $"Sistema - {_user.Role} ({_user.Username})";
            Width = 1000;
            Height = 650;

           
            lblUser.Text = $"Prisijungęs: {_user.Role} ({_user.Username})";
            topBar.Controls.Add(lblUser);
            topBar.Controls.Add(btnLogout);

        
            btnLogout.Click += (_, __) => Close();

           
            topBar.Resize += (_, __) =>
            {
                btnLogout.Left = topBar.Width - btnLogout.Width - 12;
            };
            btnLogout.Left = topBar.Width - btnLogout.Width - 12;

            Controls.Add(tabs);
            Controls.Add(topBar);

            BuildTabsByRole();
        }

        private void BuildTabsByRole()
        {
            var actions = _user.GetAllowedActions().ToHashSet();

            if (actions.Contains("ManageCommunities"))
                tabs.TabPages.Add(new TabPage("Bendrijos")
                {
                    Controls = { new AdminCommunitiesControl(_adminService) { Dock = DockStyle.Fill } }
                });

            if (actions.Contains("ManageServices"))
                tabs.TabPages.Add(new TabPage("Paslaugos")
                {
                    Controls = { new AdminServicesControl(_adminService) { Dock = DockStyle.Fill } }
                });

            if (actions.Contains("ManageUsers"))
                tabs.TabPages.Add(new TabPage("Vartotojai")
                {
                    Controls = { new AdminUsersControl(_adminService) { Dock = DockStyle.Fill } }
                });

            if (actions.Contains("AssignServiceToCommunity") || actions.Contains("SetServicePrices"))
                tabs.TabPages.Add(new TabPage("Priskyrimai / Kainos")
                {
                    Controls = { new ManagerControl(_managerService) { Dock = DockStyle.Fill } }
                });

            if (actions.Contains("ViewMyCommunityServicesAndPrices"))
            {
                if (_user is ResidentUser ru)
                    tabs.TabPages.Add(new TabPage("Mano paslaugos")
                    {
                        Controls = { new ResidentControl(_residentService, ru.CommunityId!.Value) { Dock = DockStyle.Fill } }
                    });
            }

            if (tabs.TabPages.Count == 0)
                tabs.TabPages.Add(new TabPage("Info")
                {
                    Controls = { new Label { Text = "Nėra prieinamų funkcijų.", Dock = DockStyle.Fill } }
                });
        }
    }
}
