using CommunityServices.Data;
using System.Collections.Generic;


namespace CommunityServices.Services
{
    public class ManagerService
    {
        private readonly ICommunityRepository _communities;
        private readonly IServiceRepository _services;
        private readonly ICommunityServiceRepository _cs;
        private readonly IPriceRepository _prices;

        public ManagerService(ICommunityRepository communities, IServiceRepository services,
            ICommunityServiceRepository cs, IPriceRepository prices)
        {
            _communities = communities;
            _services = services;
            _cs = cs;
            _prices = prices;
        }

        public List<CommunityServices.Domain.Community> GetCommunities() => _communities.GetAll();
        public List<CommunityServices.Domain.ServiceItem> GetServices() => _services.GetAll();

        public int AssignServiceToCommunity(int communityId, int serviceId) => _cs.AssignService(communityId, serviceId);

        public void SetPrice(int communityServiceId, decimal price, string currency = "EUR")
        {
            if (price < 0) throw new ArgumentException("Kaina negali būti neigiama.");
            _prices.UpsertPrice(communityServiceId, price, currency);
        }

        public List<(int CommunityServiceId, string ServiceName, string? Description, decimal? Price, string Currency)>
            GetCommunityServices(int communityId) => _cs.GetServicesForCommunity(communityId);
    }
}
