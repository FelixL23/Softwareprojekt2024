using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data;

public class TaskTemplateRepository : ITaskTemplateRepository
{
    private readonly ReplayContext _context;

    public TaskTemplateRepository()
    {
        _context = new ReplayContext();
    }

    public TaskTemplateRepository(ReplayContext context)
    {
        _context = context;
    }

    // Made by Fabio
    // Methode um alle Aufgabenvorlagen aus der Datenbank zu lesen 
    public async Task<List<TaskTemplate>> ToList()
    {
        return await _context
            .TaskTemplates
            .Include(t => t.Departments)
            .Include(t => t.TaskInstances)
            .Include(t => t.TaskResponsible)
            .ThenInclude(tr => tr!.Role)
            .ToListAsync();
    }

    // Made by Fabio
    // Methode um eine bestimme Aufgabenvorlage per Id zu finden
    public async Task<TaskTemplate?> Find(int id)
    {
        return await _context.TaskTemplates
            .Include(tt => tt.Departments)
            .Include(tt => tt.TaskResponsible)
            .ThenInclude(tr => tr!.Role)
            .FirstOrDefaultAsync(tt => tt.Id == id);
    }

    // Made by Fabio
    // Methode um eine Aufgabenvorlage in die Datenbank zu speichern
    public async Task Add(TaskTemplate taskTemplate)
    {
        // Überprüfen ob der Aufgabenzuständige eine Rolle hat und falls ja diese zu tracken
        if (taskTemplate.TaskResponsible != null && taskTemplate.TaskResponsible.Role != null)
        {
            _context.Roles.Attach(taskTemplate.TaskResponsible.Role);
        }

        taskTemplate.Departments.ForEach(d =>
        {
            _context.Entry(d).State = EntityState.Unchanged;
        });

        await _context.TaskTemplates.AddAsync(taskTemplate);
        await _context.SaveChangesAsync();
    }

    // Made by Fabio
    // Methode um änderungen an der Datenbank zu speichern
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    // Made by Fabio
    // Methode um eine Aufgabenvorlage upzudaten
    public async Task Update(TaskTemplate taskTemplate)
    {
        var existingTaskTemplate = await Find(taskTemplate.Id);

        if (existingTaskTemplate == null)
        {
            throw new Exception($"ProcedureTemplate with ID {taskTemplate.Id} not found!");
        }

        _context.Entry(existingTaskTemplate).CurrentValues.SetValues(taskTemplate);

        if (taskTemplate.TaskResponsible.Role != null)
        {
            _context.Roles.Attach(taskTemplate.TaskResponsible.Role);
        }

        foreach (var department in taskTemplate.Departments)
        {
            var existingDepartment = existingTaskTemplate.Departments.FirstOrDefault(d => d.Id == department.Id);

            if (existingDepartment == null)
            {
                existingTaskTemplate.Departments.Add(department);
            }
            else
            {
                _context.Entry(existingDepartment).CurrentValues.SetValues(department);
            }
        }

        existingTaskTemplate.Departments.RemoveAll(department =>
            !taskTemplate.Departments.Any(d => d.Id == department.Id)
        );

        await _context.SaveChangesAsync();
    }

    public Task<bool> Exists(int id)
    {
        throw new NotImplementedException();
    }

    public Task Remove(int id)
    {
        throw new NotImplementedException();
    }

    public Task Remove(TaskTemplate element)
    {
        throw new NotImplementedException();
    }

    // Made by Fabio
    // Methode um einen Aufgabenzuständigen für die Aufgabenvorlage zu finden
    public async Task<TaskResponsible> GetTaskResponsible(int id)
    {
        var taskTemplate = await _context.TaskTemplates
            .Include(tt => tt.TaskResponsible)
            .ThenInclude(tr => tr.Role)
            .FirstOrDefaultAsync(tt => tt.Id == id);

        if (taskTemplate == null || taskTemplate.TaskResponsible == null)
        {
            throw new Exception();
        }

        return taskTemplate.TaskResponsible;
    }

    public async Task<List<TaskTemplate>> GetArchivedTaskTemplateAsync()
    {
        // hier mit == true weil boolean? statt bool
        return await _context.TaskTemplates.Where(t => t.IsArchived == true).ToListAsync();
    }

    /// <summary>
    /// Method for creating a drop down menu with all TaskTemplates.
    /// </summary>
    /// <returns>
    /// List of all TaskTemplates as SelecListItems
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public async Task<List<SelectListItem>> GetSelectList()
    {
        var allTasks = await _context.TaskTemplates
            .Select(e => new SelectListItem()
            {
                Value = e.Id.ToString(),
                Text = e.Name
            })
            .ToListAsync();
        return allTasks;
    }

    /// <summary>
    /// Imports TaskTemplates from a JSON text into the database.
    /// </summary>
    /// <param name="text">JSON text containing task template data</param>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public void ImportFromJSON(string text)
    {
        Console.WriteLine("DEBUG: ImportFromJSON: TaskTemplate");

        var tasks = JsonSerializer.Deserialize<List<TaskTemplate>>(text);
        if (tasks != null) {
            foreach (var task in tasks) {
                List<int> depIds = task.Departments.Select(r => r.Id).ToList();
                task.Departments.Clear();
                Console.Write($"  importing: Id={task.Id}, Name={task.Name}, Departments: [");
                
                using (var context = new ReplayContext())
                {
                    foreach(var did in depIds) {
                        var loadedDep = context.Departments.First(d => d.Id == did);
                        task.Departments.Add(loadedDep);
                        Console.Write($"({loadedDep.Id}), ");
                    }
                    context.TaskTemplates.Add(task);
                    context.SaveChanges();
                }
                Console.Write($"]\n");
            }
        }
        Console.WriteLine("DEBUG: ImportFromJSON: end");
    }

    /// <summary>
    /// Exports all TaskTemplates in the database to JSON text.
    /// </summary>
    /// <param name="options">options on how to generate the text. Default: null</param>
    /// <returns>
    /// JSON text containing task template data
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public string ExportToJSON(JsonSerializerOptions? options = null)
    {
        string text = JsonSerializer.Serialize<List<TaskTemplate>>(
            _context.TaskTemplates
                .Include(tt => tt.Departments)
                .Include(tt => tt.TaskResponsible)
                .ToList(),
            options
        );
        return text;
    }

    /// <summary>
    /// Destroys this object. See IDisposable.
    /// </summary>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public void Dispose()
    {
        if (_context != null) {
            _context.Dispose();
        }
    }
}