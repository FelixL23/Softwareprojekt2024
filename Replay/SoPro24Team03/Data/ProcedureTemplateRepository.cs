using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data;

public class ProcedureTemplateRepository : IProcedureTemplateRepository
{
    private readonly ReplayContext _context;

    public ProcedureTemplateRepository()
    {
        _context = new ReplayContext();
    }

    public ProcedureTemplateRepository(ReplayContext context)
    {
        _context = context;
    }

    // Made by Fabio
    // Methode um ein Prozess nach Id zu finden
    public async Task<ProcedureTemplate?> Find(int id)
    {
        var procedureTemplate = await _context
            .ProcedureTemplates
            .Where(p => p.Id == id)
            .Include(p => p.TaskTemplates)
                .ThenInclude(t => t.TaskResponsible)
            .Include(p => p.Roles)
            .FirstOrDefaultAsync();

        if (procedureTemplate == null)
        {
            return null;
        }

        procedureTemplate.TaskTemplates = OrderTaskTemplates(procedureTemplate.TaskTemplates, procedureTemplate.OrderedTaskTemplateIds);

        return procedureTemplate;
    }

    // Made by Fabio
    // Methode um zu überprüfen ob ein Prozess mit der Id existiert
    public async Task<bool> Exists(int id)
    {
        var procTempl = await _context.ProcedureTemplates.FirstAsync(p => p.Id == id);

        return procTempl != null;
    }

    // Made by Fabio
    // Methode um ein Prozess in die Datenbank zu speichern
    public async Task Add(ProcedureTemplate procedureTemplate)
    {
        // EF-Core spezifisch um zu verhindern, dass die TaskTemplates bzw. Rollen erstellt werden
        procedureTemplate.TaskTemplates.ForEach(tt =>
        {
            _context.Entry(tt).State = EntityState.Unchanged;
        });

        procedureTemplate.Roles.ForEach(r =>
        {
            _context.Entry(r).State = EntityState.Unchanged;
        });

        await _context.ProcedureTemplates.AddAsync(procedureTemplate);
        await _context.SaveChangesAsync();
    }

    // Made by Fabio
    // Methode um alle Prozesse aus der Datenbank zu lesen
    public async Task<List<ProcedureTemplate>> ToList()
    {
        List<ProcedureTemplate> procedureTemplates = await _context
            .ProcedureTemplates
            .Include(p => p.TaskTemplates)
            .Include(p => p.Roles)
            .ToListAsync();

        // Aufgabenvorlagen in den einzelnen Prozessen ordnen
        procedureTemplates = procedureTemplates.Select(pt =>
        {
            pt.TaskTemplates = OrderTaskTemplates(
            pt.TaskTemplates,
            pt.OrderedTaskTemplateIds
        );
            return pt;
        }).ToList();

        return procedureTemplates;
    }

    // Made by Fabio
    // Methode um Benutzer spezifische Prozesse zu filtern
    public async Task<List<ProcedureTemplate>> ToList(ClaimsPrincipal user)
    {
        var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == int.Parse(user.Identity!.Name!));

        if (existingUser == null)
        {
            throw new Exception("User not found!");
        }

        IQueryable<ProcedureTemplate> query = _context
            .ProcedureTemplates
            .Include(p => p.TaskTemplates)
            .Include(p => p.Roles)
            .Where(pt => pt.Roles.Any(r => r.users.Contains(existingUser)) || user.IsInRole("Admin"))
            .AsQueryable();

        var procedureTemplates = await query.ToListAsync();

        procedureTemplates = procedureTemplates.Select(pt =>
        {
            pt.TaskTemplates = OrderTaskTemplates(
            pt.TaskTemplates,
            pt.OrderedTaskTemplateIds
        );
            return pt;
        }).ToList();

        return procedureTemplates;
    }

    // Made by Fabio
    // Methode um ein Prozess mit id zu löschen
    public async Task Remove(int id)
    {
        var procTempl = await Find(id);

        if (procTempl != null)
        {
            _context.ProcedureTemplates.Remove(procTempl);
            await _context.SaveChangesAsync();
        }
    }

    // Made by Fabio
    // Methode um ein Prozess zu löschen
    public async Task Remove(ProcedureTemplate element)
    {
        _context.ProcedureTemplates.Remove(element);

        await _context.SaveChangesAsync();
    }

    // Made by Fabio
    // Methode um änderungen in der Datenbank zu speicern
    public async Task Save()
    {
        await _context.SaveChangesAsync();
    }

    // Made by Fabio
    // Methode um ein Prozess upzudaten

    public async Task Update(ProcedureTemplate procedureTemplate)
    {
        var existingProcedureTemplate = await Find(procedureTemplate.Id);

        if (existingProcedureTemplate == null)
        {
            throw new Exception($"ProcedureTemplate with ID {procedureTemplate.Id} not found!");
        }

        _context.Entry(existingProcedureTemplate).CurrentValues.SetValues(procedureTemplate);

        // Rollen entweder ins Tracking von EF-Core aufzunehmen oder zu ändern
        foreach (var role in procedureTemplate.Roles)
        {
            var existingRole = existingProcedureTemplate.Roles.FirstOrDefault(r => r.Id == role.Id);

            if (existingRole == null)
            {
                existingProcedureTemplate.Roles.Add(role);
            }
            else
            {
                _context.Entry(existingRole).CurrentValues.SetValues(role);
            }
        }

        existingProcedureTemplate.Roles.RemoveAll(role =>
            !procedureTemplate.Roles.Any(r => r.Id == role.Id)
        );

        foreach (var taskTemplate in procedureTemplate.TaskTemplates)
        {
            var existingTaskTemplate = existingProcedureTemplate.TaskTemplates.FirstOrDefault(tt => tt.Id == taskTemplate.Id);

            if (existingTaskTemplate == null)
            {
                existingProcedureTemplate.TaskTemplates.Add(taskTemplate);
            }
            else
            {
                _context.Entry(existingTaskTemplate).CurrentValues.SetValues(taskTemplate);
            }
        }

        existingProcedureTemplate.TaskTemplates.RemoveAll(taskTemplate =>
            !procedureTemplate.TaskTemplates.Any(tt => tt.Id == taskTemplate.Id)
        );

        await _context.SaveChangesAsync();
    }

    // Methode zum Abrufen archivierter Prozesse Felix
    public async Task<List<ProcedureTemplate>> GetArchivedProcedureTemplateAsync()
    {
        // hier mit == true weil boolean? statt bool
        return await _context.ProcedureTemplates.Where(p => p.IsArchived == true).ToListAsync();
    }

    // Made by Fabio
    // Methode um Aufgabenvorlagen zu sortieren
    private static List<TaskTemplate> OrderTaskTemplates(List<TaskTemplate> taskTemplates, string orderedIds)
    {
        List<int> idOrder = orderedIds.Split(',').Select(int.Parse).ToList();

        return taskTemplates.OrderBy(tt => idOrder.IndexOf(tt.Id)).ToList();
    }

    /// <summary>
    /// Imports ProcedureTemplates from a JSON text into the database.
    /// </summary>
    /// <param name="text">JSON text containing procedure template data</param>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public void ImportFromJSON(string text)
    {
        Console.WriteLine("DEBUG: ImportFromJSON: ProcedureTemplate");

        var procTemplates = JsonSerializer.Deserialize<List<ProcedureTemplate>>(text);
        if (procTemplates != null) {
            foreach (var procTempl in procTemplates) {
                List<int> roleIds = procTempl.Roles.Select(r => r.Id).ToList();
                List<int> taskIds = procTempl.TaskTemplates.Select(tt => tt.Id).ToList();
                procTempl.Roles.Clear();
                procTempl.TaskTemplates.Clear();
                Console.Write($"  importing: Id={procTempl.Id}, UserName={procTempl.Name}");
                
                using (var context = new ReplayContext())
                {
                    foreach(var rid in roleIds) {
                        var loadedRole = context.Roles.First(r => r.Id == rid);
                        procTempl.Roles.Add(loadedRole);
                    }
                    foreach(var ttid in taskIds) {
                        var loadedTask = context.TaskTemplates.First(tt => tt.Id == ttid);
                        procTempl.TaskTemplates.Add(loadedTask);
                    }
                    context.ProcedureTemplates.Add(procTempl);
                    context.SaveChanges();
                }
                Console.Write($"]\n");
            }
        }
        Console.WriteLine("DEBUG: ImportFromJSON: end");
    }

    /// <summary>
    /// Exports all ProcedureTemplates in the database to JSON text.
    /// </summary>
    /// <param name="options">options on how to generate the text. Default: null</param>
    /// <returns>
    /// JSON text containing procedure template data
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public string ExportToJSON(JsonSerializerOptions? options = null)
    {
        string text = JsonSerializer.Serialize<List<ProcedureTemplate>>(
            _context.ProcedureTemplates
                .Include(pt => pt.Roles)
                .Include(pt => pt.TaskTemplates)
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