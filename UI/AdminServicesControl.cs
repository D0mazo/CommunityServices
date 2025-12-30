using System;
using System.Windows.Forms;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class AdminServicesControl : UserControl
    {
        private readonly AdminService _svc;
        private DataGridView grid = new() { Dock = DockStyle.Top, Height = 360, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        private TextBox txtName = new() { Left = 10, Top = 380, Width = 220 };
        private TextBox txtDesc = new() { Left = 240, Top = 380, Width = 360 };
        private Button btnAdd = new() { Left = 610, Top = 378, Width = 120, Height = 40, Text = "Sukurti" };
        private Button btnUpdate = new() { Left = 740, Top = 378, Width = 120, Height = 40, Text = "Atnaujinti" };
        private Button btnDelete = new() { Left = 870, Top = 378, Width = 120, Height = 40, Text = "Šalinti" };

        public AdminServicesControl(AdminService svc)
        {
            _svc = svc;
            Controls.Add(grid);
            Controls.AddRange(new Control[] { txtName, txtDesc, btnAdd, btnUpdate, btnDelete });

            btnAdd.Click += (_, __) => Run(() => _svc.CreateService(txtName.Text.Trim(), txtDesc.Text.Trim()));
            btnUpdate.Click += (_, __) => Run(() =>
            {
                var id = SelectedId();
                _svc.UpdateService(id, txtName.Text.Trim(), txtDesc.Text.Trim());
            });
            btnDelete.Click += (_, __) => Run(() =>
            {
                var id = SelectedId();
                _svc.DeleteService(id);
            });

            grid.SelectionChanged += (_, __) =>
            {
                if (grid.CurrentRow?.DataBoundItem is not null)
                {
                    txtName.Text = grid.CurrentRow.Cells["Name"].Value?.ToString() ?? "";
                    txtDesc.Text = grid.CurrentRow.Cells["Description"].Value?.ToString() ?? "";
                }
            };

            Reload();
        }

        private int SelectedId()
        {
            if (grid.CurrentRow == null) throw new InvalidOperationException("Pasirinkite paslaugą.");
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
            grid.DataSource = _svc.GetAllServices();
        }
    }
}
