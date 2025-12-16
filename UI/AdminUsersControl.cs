using System;
using System.Linq;
using System.Windows.Forms;
using CommunityServices.Domain;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class AdminUsersControl : UserControl
    {
        private readonly AdminService _svc;

        private ComboBox cmbRole = new() { Left = 10, Top = 10, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbCommunity = new() { Left = 200, Top = 10, Width = 260, DropDownStyle = ComboBoxStyle.DropDownList };
        private TextBox txtFirst = new() { Left = 470, Top = 10, Width = 160, PlaceholderText = "Vardas" };
        private TextBox txtLast = new() { Left = 640, Top = 10, Width = 160, PlaceholderText = "Pavardė" };
        private Button btnCreate = new() { Left = 810, Top = 8, Width = 160, Text = "Kurti auto" };

        private TextBox txtDeleteId = new() { Left = 10, Top = 50, Width = 180, PlaceholderText = "User ID (šalinimui)" };
        private Button btnDelete = new() { Left = 200, Top = 48, Width = 260, Text = "Šalinti vartotoją" };

        private Label lblOut = new() { Left = 10, Top = 90, Width = 960, Height = 80 };

        public AdminUsersControl(AdminService svc)
        {
            _svc = svc;

            Controls.AddRange(new Control[] { cmbRole, cmbCommunity, txtFirst, txtLast, btnCreate, txtDeleteId, btnDelete, lblOut });

            cmbRole.Items.AddRange(new object[] { Role.MANAGER, Role.RESIDENT });
            cmbRole.SelectedIndex = 0;

            LoadCommunities();

            cmbRole.SelectedIndexChanged += (_, __) => cmbCommunity.Enabled = ((Role)cmbRole.SelectedItem!) == Role.RESIDENT;

            btnCreate.Click += (_, __) => CreateAuto();
            btnDelete.Click += (_, __) => DeleteUser();
        }

        private void LoadCommunities()
        {
            var communities = _svc.GetAllCommunities();
            cmbCommunity.DataSource = communities;
            cmbCommunity.DisplayMember = "Name";
            cmbCommunity.ValueMember = "Id";
            cmbCommunity.Enabled = ((Role)cmbRole.SelectedItem!) == Role.RESIDENT;
        }

        private void CreateAuto()
        {
            try
            {
                var role = (Role)cmbRole.SelectedItem!;
                var first = txtFirst.Text.Trim();
                var last = txtLast.Text.Trim();
                if (string.IsNullOrWhiteSpace(first) || string.IsNullOrWhiteSpace(last))
                    throw new ArgumentException("Įveskite vardą ir pavardę.");

                if (role == Role.MANAGER)
                {
                    var creds = _svc.CreateManagerAuto(first, last);
                    lblOut.Text = $"SUKURTA: Manager. Username={creds.username}, Password={creds.password}";
                }
                else
                {
                    if (cmbCommunity.SelectedValue == null) throw new InvalidOperationException("Pasirinkite bendriją.");
                    int communityId = Convert.ToInt32(cmbCommunity.SelectedValue);
                    var creds = _svc.CreateResidentAuto(first, last, communityId);
                    lblOut.Text = $"SUKURTA: Resident (Bendrija ID={communityId}). Username={creds.username}, Password={creds.password}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteUser()
        {
            try
            {
                if (!int.TryParse(txtDeleteId.Text.Trim(), out var id))
                    throw new ArgumentException("Neteisingas User ID.");
                _svc.DeleteUser(id);
                lblOut.Text = $"Ištrintas vartotojas ID={id}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
