using System;
using System.Windows.Forms;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class AdminCommunitiesControl : UserControl
    {
        private readonly AdminService _svc;
        private DataGridView grid = new() { Dock = DockStyle.Top, Height = 360, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtName = new() { Left = 10, Top = 380, Width = 300 };
        private Button btnAdd = new() { Left = 320, Top = 378, Width = 120,Height = 40, Text = "Sukurti" };
        private Button btnUpdate = new() { Left = 450, Top = 378, Width = 120, Height = 40, Text = "Atnaujinti" };
        private Button btnDelete = new() { Left = 580, Top = 378, Width = 120, Height = 40, Text = "Šalinti" };

        public AdminCommunitiesControl(AdminService svc)
        {
            _svc = svc;
            Controls.Add(grid);
            Controls.Add(txtName);
            Controls.AddRange(new Control[] { btnAdd, btnUpdate, btnDelete });

            btnAdd.Click += (_, __) => Run(() => _svc.CreateCommunity(txtName.Text.Trim()));
            btnUpdate.Click += (_, __) => Run(() =>
            {
                var id = SelectedId();
                _svc.UpdateCommunity(id, txtName.Text.Trim());
            });
            btnDelete.Click += (_, __) => Run(() =>
            {
                var id = SelectedId();
                _svc.DeleteCommunity(id);
            });

            grid.SelectionChanged += (_, __) =>
            {
                if (grid.CurrentRow?.DataBoundItem is not null)
                    txtName.Text = grid.CurrentRow.Cells["Name"].Value?.ToString() ?? "";
            };

            Reload();
        }

        private int SelectedId()
        {
            if (grid.CurrentRow == null) throw new InvalidOperationException("Pasirinkite bendriją.");
            return Convert.ToInt32(grid.CurrentRow.Cells["Id"].Value);
        }

        private void Run(Action action)
        {
            try { action(); Reload(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Run(Func<int> action)
        {
            try { action(); Reload(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Reload()
        {
            grid.DataSource = _svc.GetAllCommunities();
        }
    }
}
