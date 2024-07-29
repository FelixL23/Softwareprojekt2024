using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data
{
    /// <summary>
    /// Allows accessing Procedures in the database.
    /// Abstraction of ProcedureRepository. For testing.
    /// </summary>
    public interface IProcedureRepository : IRepository<Procedure>
    {
        public Task<List<Procedure>> ToList(int UserId, String ProcName = null!, int num = 0, int skip = 0);

        public Task<List<Procedure>> GetArchivedProcedureAsync();

        public Task ArchiveProcedureAsync(int id, int userId, bool isAdmin);
        
        public Task<List<SelectListItem>> GetSelectList(int UserId);
    }
}