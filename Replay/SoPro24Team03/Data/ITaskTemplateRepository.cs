using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data;

public interface ITaskTemplateRepository : IRepository<TaskTemplate>
{
    public Task<TaskResponsible?> GetTaskResponsible(int id);

    public Task<List<TaskTemplate>> GetArchivedTaskTemplateAsync(); // Felix Methode

    public Task<List<SelectListItem>> GetSelectList();  // Daniel Albert
}