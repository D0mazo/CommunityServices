using CommunityServices.Data;
using CommunityServices.Domain;
using System.Collections.Generic;
using System;
using System.Collections.Generic;


namespace CommunityServices.Services
{
    public class AdminService
    {
        private readonly ICommunityRepository _communities;
        private readonly IServiceRepository _services;
        private readonly IUserRepository _users;

        public AdminService(ICommunityRepository communities, IServiceRepository services, IUserRepository users)
        {
            _communities = communities;
            _services = services;
            _users = users;
        }

        public int CreateCommunity(string name) => _communities.Create(name);
        public void UpdateCommunity(int id, string name) => _communities.Update(id, name);
        public void DeleteCommunity(int id) => _communities.Delete(id);
        public List<Community> GetAllCommunities() => _communities.GetAll();


        public int CreateService(string name, string? description) => _services.Create(name, description);
        public void UpdateService(int id, string name, string? description) => _services.Update(id, name, description);
        public void DeleteService(int id) => _services.Delete(id);
        public List<ServiceItem> GetAllServices() => _services.GetAll();


        public List<UserListItem> GetUsers(string? firstNameFilter)
            => _users.GetAll(firstNameFilter);

 
        public void UpdateUser(int id, string username, string firstName, string lastName, Role role, int? communityId)
        {
            if (role == Role.RESIDENT && communityId == null)
                throw new ArgumentException("RESIDENT privalo turėti bendriją (community).");

            if (role != Role.RESIDENT)
                communityId = null;

            _users.UpdateUser(id, username.Trim(), firstName.Trim(), lastName.Trim(), role, communityId);
        }

 
        public (string username, string password) CreateManagerAuto(string firstName, string lastName)
        {
            var username = firstName.Trim();
            var password = lastName.Trim();
            var hash = PasswordHasher.Hash(password);

            _users.CreateUser(username, hash, firstName, lastName, Role.MANAGER, null);
            return (username, password);
        }

        public (string username, string password) CreateResidentAuto(string firstName, string lastName, int communityId)
        {
            var username = firstName.Trim();
            var password = lastName.Trim();
            var hash = PasswordHasher.Hash(password);

            _users.CreateUser(username, hash, firstName, lastName, Role.RESIDENT, communityId);
            return (username, password);
        }

        public void DeleteUser(int userId) => _users.DeleteUser(userId);
    }
}
