using CommunityServices.Data;
using System.Collections.Generic;


namespace CommunityServices.Services
{
    public class ResidentService
    {
        private readonly ICommunityServiceRepository _cs;

        public ResidentService(ICommunityServiceRepository cs)
        {
            _cs = cs;
        }

        public List<(int CommunityServiceId, string ServiceName, string? Description, decimal? Price, string Currency)>
            GetMyCommunityServices(int communityId) => _cs.GetServicesForCommunity(communityId);
    }
}
