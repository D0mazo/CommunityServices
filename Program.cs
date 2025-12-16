using CommunityServices.Data;
using CommunityServices.Services;
using CommunityServices.UI;
using System;
using System.Windows.Forms;


namespace CommunityServices
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // DI (paprastas rankinis)
            var userRepo = new UserRepository();
            var commRepo = new CommunityRepository();
            var servRepo = new ServiceRepository();
            var csRepo = new CommunityServiceRepository();
            var priceRepo = new PriceRepository();

            // Seed admin
            Seeder.EnsureAdminExists(userRepo);

            var auth = new AuthService(userRepo);
            var adminService = new AdminService(commRepo, servRepo, userRepo);
            var managerService = new ManagerService(commRepo, servRepo, csRepo, priceRepo);
            var residentService = new ResidentService(csRepo);

            Application.Run(new LoginForm(auth, adminService, managerService, residentService));
        }
    }
}
