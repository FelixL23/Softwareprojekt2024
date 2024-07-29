using System.Security.Claims;
using SoPro24Team03.Data;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data;

public interface IProcedureTemplateRepository : IRepository<ProcedureTemplate> 
{
    public Task<List<ProcedureTemplate>> ToList(ClaimsPrincipal user);
    public Task Save();
    public Task<List<ProcedureTemplate>> GetArchivedProcedureTemplateAsync(); // Felix Methode
}