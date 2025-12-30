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

        // ===== FILTER / LIST =====
        private TextBox txtFilter = new() { Left = 10, Top = 10, Width = 220, PlaceholderText = "Filtruoti pagal vardą" };
        private Button btnSearch = new() { Left = 240, Top = 8, Width = 90, Text = "Ieškoti" };
        private Button btnAll = new() { Left = 335, Top = 8, Width = 110, Text = "Rodyti visus" };

        private DataGridView grid = new()
        {
            Left = 10,
            Top = 45,
            Width = 960,
            Height = 220,
            ReadOnly = true,
            AllowUserToAddRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private Label lblOut = new() { Left = 10, Top = 270, Width = 960, AutoSize = false };

        // ===== CREATE USER (AUTO) =====
        private GroupBox grpCreate = new() { Left = 10, Top = 295, Width = 960, Height = 115, Text = "Sukurti vartotoją (auto: username=vardas, password=pavardė)" };

        private TextBox txtCreateFirst = new() { Left = 10, Top = 25, Width = 180, PlaceholderText = "Vardas" };
        private TextBox txtCreateLast = new() { Left = 200, Top = 25, Width = 180, PlaceholderText = "Pavardė" };
        private ComboBox cmbCreateRole = new() { Left = 390, Top = 25, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbCreateCommunity = new() { Left = 540, Top = 25, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

        private Button btnCreate = new() { Left = 750, Top = 23, Width = 190, Text = "Sukurti vartotoją" };

        private Label lblCreateHint = new() { Left = 10, Top = 60, Width = 930, Height = 45, AutoSize = false };

        // ===== EDIT USER =====
        private GroupBox grpEdit = new() { Left = 10, Top = 420, Width = 960, Height = 120, Text = "Redaguoti pasirinktą vartotoją" };

        private TextBox txtId = new() { Left = 10, Top = 25, Width = 80, ReadOnly = true };
        private TextBox txtUsername = new() { Left = 100, Top = 25, Width = 180, PlaceholderText = "Username" };
        private TextBox txtFirst = new() { Left = 290, Top = 25, Width = 180, PlaceholderText = "Vardas" };
        private TextBox txtLast = new() { Left = 480, Top = 25, Width = 180, PlaceholderText = "Pavardė" };

        private ComboBox cmbRole = new() { Left = 670, Top = 25, Width = 140, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbCommunity = new() { Left = 820, Top = 25, Width = 130, DropDownStyle = ComboBoxStyle.DropDownList };

        private Button btnSave = new() { Left = 10, Top = 60, Width = 160, Text = "Išsaugoti" };

        // ===== DELETE USER =====
        private GroupBox grpDelete = new() { Left = 10, Top = 550, Width = 960, Height = 70, Text = "Trinti vartotoją" };
        private TextBox txtDeleteId = new() { Left = 10, Top = 28, Width = 160, PlaceholderText = "User ID trynimui" };
        private Button btnDelete = new() { Left = 180, Top = 26, Width = 120, Text = "Trinti" };

        public AdminUsersControl(AdminService svc)
        {
            _svc = svc;

            Controls.Add(txtFilter);
            Controls.Add(btnSearch);
            Controls.Add(btnAll);
            Controls.Add(grid);
            Controls.Add(lblOut);

            Controls.Add(grpCreate);
            grpCreate.Controls.Add(txtCreateFirst);
            grpCreate.Controls.Add(txtCreateLast);
            grpCreate.Controls.Add(cmbCreateRole);
            grpCreate.Controls.Add(cmbCreateCommunity);
            grpCreate.Controls.Add(btnCreate);
            grpCreate.Controls.Add(lblCreateHint);

            Controls.Add(grpEdit);
            grpEdit.Controls.Add(txtId);
            grpEdit.Controls.Add(txtUsername);
            grpEdit.Controls.Add(txtFirst);
            grpEdit.Controls.Add(txtLast);
            grpEdit.Controls.Add(cmbRole);
            grpEdit.Controls.Add(cmbCommunity);
            grpEdit.Controls.Add(btnSave);

            Controls.Add(grpDelete);
            grpDelete.Controls.Add(txtDeleteId);
            grpDelete.Controls.Add(btnDelete);

            Dock = DockStyle.Fill;

            // Roles (kūrimui auto leidžiam MANAGER arba RESIDENT)
            cmbCreateRole.Items.Add(Role.MANAGER);
            cmbCreateRole.Items.Add(Role.RESIDENT);
            cmbCreateRole.SelectedIndex = 0;

            // Roles redagavimui – visi enum
            cmbRole.Items.AddRange(Enum.GetValues(typeof(Role)).Cast<object>().ToArray());

            btnSearch.Click += (_, __) => LoadUsers(txtFilter.Text);
            btnAll.Click += (_, __) => { txtFilter.Text = ""; LoadUsers(null); };

            grid.SelectionChanged += (_, __) => FillEditFromSelection();

            btnSave.Click += (_, __) => SaveUser();
            btnDelete.Click += (_, __) => DeleteUser();

            btnCreate.Click += (_, __) => CreateUserAuto();
            cmbCreateRole.SelectedIndexChanged += (_, __) => UpdateCreateHint();

            LoadCommunities();
            UpdateCreateHint();
            LoadUsers(null);
        }

        private void LoadCommunities()
        {
            cmbCommunity.Items.Clear();
            cmbCreateCommunity.Items.Clear();

            var none = new ComboItem { Id = null, Text = "(be bendrijos)" };
            cmbCommunity.Items.Add(none);
            cmbCreateCommunity.Items.Add(none);

            foreach (var c in _svc.GetAllCommunities())
            {
                var item = new ComboItem { Id = c.Id, Text = $"{c.Id} - {c.Name}" };
                cmbCommunity.Items.Add(item);
                cmbCreateCommunity.Items.Add(item);
            }

            cmbCommunity.SelectedIndex = 0;
            cmbCreateCommunity.SelectedIndex = 0;
        }

        private void UpdateCreateHint()
        {
            if (cmbCreateRole.SelectedItem is not Role role) return;

            if (role == Role.RESIDENT)
            {
                lblCreateHint.Text = "RESIDENT: privaloma pasirinkti bendriją.\r\nSukūrus gausi prisijungimus: username = vardas, password = pavardė.";
                cmbCreateCommunity.Enabled = true;
            }
            else
            {
                lblCreateHint.Text = "MANAGER: bendrija nepriskiriama.\r\nSukūrus gausi prisijungimus: username = vardas, password = pavardė.";
                cmbCreateCommunity.Enabled = false;
                cmbCreateCommunity.SelectedIndex = 0;
            }
        }

        private void LoadUsers(string? filter)
        {
            try
            {
                var users = _svc.GetUsers(filter);
                grid.DataSource = users;

                if (grid.Columns["Id"] != null) grid.Columns["Id"].HeaderText = "ID";
                if (grid.Columns["Username"] != null) grid.Columns["Username"].HeaderText = "Username";
                if (grid.Columns["FirstName"] != null) grid.Columns["FirstName"].HeaderText = "Vardas";
                if (grid.Columns["LastName"] != null) grid.Columns["LastName"].HeaderText = "Pavardė";
                if (grid.Columns["Role"] != null) grid.Columns["Role"].HeaderText = "Rolė";
                if (grid.Columns["CommunityId"] != null) grid.Columns["CommunityId"].HeaderText = "Bendrija ID";

                lblOut.Text = $"Rasta: {users.Count}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillEditFromSelection()
        {
            if (grid.CurrentRow?.DataBoundItem is not UserListItem u) return;

            txtId.Text = u.Id.ToString();
            txtUsername.Text = u.Username;
            txtFirst.Text = u.FirstName;
            txtLast.Text = u.LastName;

            cmbRole.SelectedItem = u.Role;

            var target = cmbCommunity.Items
                .OfType<ComboItem>()
                .FirstOrDefault(x => x.Id == u.CommunityId);

            cmbCommunity.SelectedItem = target ?? cmbCommunity.Items[0];
        }

        private void CreateUserAuto()
        {
            try
            {
                var first = txtCreateFirst.Text.Trim();
                var last = txtCreateLast.Text.Trim();

                if (string.IsNullOrWhiteSpace(first)) throw new ArgumentException("Vardas privalomas.");
                if (string.IsNullOrWhiteSpace(last)) throw new ArgumentException("Pavardė privaloma.");
                if (cmbCreateRole.SelectedItem is not Role role) throw new ArgumentException("Pasirink rolę.");

                (string username, string password) creds;

                if (role == Role.RESIDENT)
                {
                    var selected = cmbCreateCommunity.SelectedItem as ComboItem;
                    if (selected?.Id == null) throw new ArgumentException("RESIDENT privalo turėti bendriją.");
                    creds = _svc.CreateResidentAuto(first, last, selected.Id.Value);
                }
                else // MANAGER
                {
                    creds = _svc.CreateManagerAuto(first, last);
                }

                // išvalom laukus
                txtCreateFirst.Text = "";
                txtCreateLast.Text = "";
                cmbCreateRole.SelectedIndex = 0;
                cmbCreateCommunity.SelectedIndex = 0;
                UpdateCreateHint();

                lblOut.Text = $"Sukurta ✅  Prisijungimai: username = {creds.username}, password = {creds.password}";
                LoadUsers(string.IsNullOrWhiteSpace(txtFilter.Text) ? null : txtFilter.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveUser()
        {
            try
            {
                if (!int.TryParse(txtId.Text, out var id))
                    throw new ArgumentException("Pasirink vartotoją iš sąrašo.");

                var username = txtUsername.Text.Trim();
                var first = txtFirst.Text.Trim();
                var last = txtLast.Text.Trim();

                if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username privalomas.");
                if (string.IsNullOrWhiteSpace(first)) throw new ArgumentException("Vardas privalomas.");
                if (string.IsNullOrWhiteSpace(last)) throw new ArgumentException("Pavardė privaloma.");
                if (cmbRole.SelectedItem is not Role role) throw new ArgumentException("Pasirink rolę.");

                var selectedCommunity = cmbCommunity.SelectedItem as ComboItem;
                int? communityId = selectedCommunity?.Id;

                _svc.UpdateUser(id, username, first, last, role, communityId);

                lblOut.Text = $"Išsaugota ✅ (ID={id})";
                LoadUsers(string.IsNullOrWhiteSpace(txtFilter.Text) ? null : txtFilter.Text);
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
                lblOut.Text = $"Ištrintas ✅ vartotojas ID={id}";
                LoadUsers(string.IsNullOrWhiteSpace(txtFilter.Text) ? null : txtFilter.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboItem
        {
            public int? Id { get; set; }
            public string Text { get; set; } = "";
            public override string ToString() => Text;
        }
    }
}
