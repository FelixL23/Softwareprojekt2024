using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User?> Find(string userName);

        public Task<bool> Exists(string userName);
        
        public Task Save();

        public Task<string> GetUserRoles(string userName);

        public Task AssignRole(int id, Role role);

        public Task UnassignRole(int id, Role role);

        public Task<List<User>> GetArchivedUsersAsync();  //Methode Felix

        public Task ArchiveUserAsync(int id); //Methode Felix

        public Task<List<SelectListItem>> GetSelectList();  // Daniel Albert

        public Task<User?> LoadCurrentUser(HttpContext httpContext);  // Made by Daniel Albert
    }
}
