using System;
using System.Linq;
using System.Windows.Forms;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class ManagerControl : UserControl
    {
        private readonly ManagerService _svc;

        private ComboBox cmbCommunity = new() { Left = 10, Top = 10, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
        private ComboBox cmbService = new() { Left = 300, Top = 10, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
        private Button btnAssign = new() { Left = 590, Top = 8, Width = 160, Text = "Priskirti" };

        private DataGridView grid = new() { Left = 10, Top = 50, Width = 960, Height = 420, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        private TextBox txtPrice = new() { Left = 10, Top = 490, Width = 120, PlaceholderText = "Kaina" };
        private Button btnSetPrice = new() { Left = 140, Top = 488, Width = 200, Text = "Nustatyti / redaguoti kainą" };
        private Label lblHint = new() { Left = 360, Top = 492, Width = 610, Height = 40, Text = "Pasirinkite eilutę (bendrijos paslauga), tada įveskite kainą." };

        public ManagerControl(ManagerService svc)
        {
            _svc = svc;
            Controls.AddRange(new Control[] { cmbCommunity, cmbService, btnAssign, grid, txtPrice, btnSetPrice, lblHint });

            LoadLists();

            cmbCommunity.SelectedIndexChanged += (_, __) => ReloadGrid();
            btnAssign.Click += (_, __) => Assign();
            btnSetPrice.Click += (_, __) => SetPrice();

            ReloadGrid();
        }

        private void LoadLists()
        {
            cmbCommunity.DataSource = _svc.GetCommunities();
            cmbCommunity.DisplayMember = "Name";
            cmbCommunity.ValueMember = "Id";

            cmbService.DataSource = _svc.GetServices();
            cmbService.DisplayMember = "Name";
            cmbService.ValueMember = "Id";
        }

        private int SelectedCommunityId() => Convert.ToInt32(cmbCommunity.SelectedValue);

        private void ReloadGrid()
        {
            if (cmbCommunity.SelectedValue == null) return;
            var communityId = SelectedCommunityId();

            var data = _svc.GetCommunityServices(communityId)
                .Select(x => new
                {
                    x.CommunityServiceId,
                    x.ServiceName,
                    x.Description,
                    Price = x.Price.HasValue ? x.Price.Value.ToString("0.00") : "",
                    x.Currency
                })
                .ToList();

            grid.DataSource = data;
        }

        private void Assign()
        {
            try
            {
                var communityId = SelectedCommunityId();
                var serviceId = Convert.ToInt32(cmbService.SelectedValue);
                _svc.AssignServiceToCommunity(communityId, serviceId);
                ReloadGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetPrice()
        {
            try
            {
                if (grid.CurrentRow == null) throw new InvalidOperationException("Pasirinkite paslaugą lentelėje.");
                var csId = Convert.ToInt32(grid.CurrentRow.Cells["CommunityServiceId"].Value);

                if (!decimal.TryParse(txtPrice.Text.Trim(), out var price))
                    throw new ArgumentException("Neteisingas kainos formatas.");

                _svc.SetPrice(csId, price, "EUR");
                ReloadGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Klaida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
