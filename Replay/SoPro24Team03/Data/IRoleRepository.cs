using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data
{
    public interface IRoleRepository : IRepository<Role>
    {
        public Task<List<User>> GetAllUsers();

        public Task<List<SelectListItem>> GetSelectList();  // Daniel Albert
    }
}