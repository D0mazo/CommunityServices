using CommunityServices.Domain;
using System.Collections.Generic;


namespace CommunityServices.Data
{
    public interface IUserRepository
    {
        (int id, string username, string passHash, string firstName, string lastName, Role role, int? communityId)? GetByUsername(string username);
        int CreateUser(string username, string passwordHash, string firstName, string lastName, Role role, int? communityId);
        void DeleteUser(int userId);


       
        List<UserListItem> GetAll(string? firstNameFilter);

        void UpdateUser(int id, string username, string firstName, string lastName, Role role, int? communityId);

    }

    public interface ICommunityRepository
    {
        int Create(string name);
        void Update(int id, string name);
        void Delete(int id);
        List<Community> GetAll();
    }

    public interface IServiceRepository
    {
        int Create(string name, string? description);
        void Update(int id, string name, string? description);
        void Delete(int id);
        List<ServiceItem> GetAll();
    }

    public interface ICommunityServiceRepository
    {
        int AssignService(int communityId, int serviceId);
        void UnassignService(int communityId, int serviceId);
        List<(int CommunityServiceId, string ServiceName, string? Description, decimal? Price, string Currency)> GetServicesForCommunity(int communityId);
    }

    public interface IPriceRepository
    {
        void UpsertPrice(int communityServiceId, decimal price, string currency);
    }
}
