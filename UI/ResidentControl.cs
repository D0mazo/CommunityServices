using System;
using System.Linq;
using System.Windows.Forms;
using CommunityServices.Services;

namespace CommunityServices.UI
{
    public class ResidentControl : UserControl
    {
        private readonly ResidentService _svc;
        private readonly int _communityId;

        private TextBox txtSearch = new() { Left = 10, Top = 10, Width = 360, PlaceholderText = "Paieška pagal paslaugos pavadinimą..." };
        private Button btnSearch = new() { Left = 380, Top = 8, Width = 120, Text = "Filtruoti" };
        private Button btnClear = new() { Left = 510, Top = 8, Width = 120, Text = "Valyti" };

        private DataGridView grid = new() { Left = 10, Top = 50, Width = 960, Height = 520, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

        public ResidentControl(ResidentService svc, int communityId)
        {
            _svc = svc;
            _communityId = communityId;

            Controls.AddRange(new Control[] { txtSearch, btnSearch, btnClear, grid });

            btnSearch.Click += (_, __) => Reload();
            btnClear.Click += (_, __) => { txtSearch.Text = ""; Reload(); };

            Reload();
        }

        private void Reload()
        {
            var query = txtSearch.Text.Trim().ToLowerInvariant();
            var data = _svc.GetMyCommunityServices(_communityId)
                .Where(x => string.IsNullOrWhiteSpace(query) || x.ServiceName.ToLowerInvariant().Contains(query))
                .Select(x => new
                {
                    x.ServiceName,
                    x.Description,
                    Price = x.Price.HasValue ? x.Price.Value.ToString("0.00") : "—",
                    x.Currency
                })
                .ToList();

            grid.DataSource = data;
        }
    }
}
