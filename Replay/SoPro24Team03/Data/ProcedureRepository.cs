using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Models;


namespace SoPro24Team03.Data
{
    /// <summary>
    /// Allows accessing Procedures in the database.
    /// </summary>
    public class ProcedureRepository : IProcedureRepository
    {
        /// <value>Context for accessing the database.</value>
        protected readonly ReplayContext _context;

        public ProcedureRepository()
        {
            _context = new ReplayContext();
        }

        public ProcedureRepository(ReplayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Reads a list of all Procedures sorted by target date from the database.
        /// </summary>
        /// <returns>
        /// List of Procedures.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<List<Procedure>> ToList()
        {
            return await ToList(0);
        }

        /// <summary>
        /// Reads a filtered list of Procedures sorted by target date from the database.
        /// </summary>
        /// <param name="UserId">Id of the user, that the procedures should be filtered for. Default: 0, show procedures of all users.</param>
        /// <param name="ProcName">String, that the procedures' names must contain. Default: null, no filtering.</param>
        /// <param name="num">The maximum number of elements to be returned. For Pagination. Default: 0, return all (remaining) elements.</param>
        /// <param name="skip">The number of elements to be skipped. For Pagination. Default: 0, skip no elements.</param>
        /// <returns>
        /// List of Procedures.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert, Felix
        /// </remarks>
        public virtual async Task<List<Procedure>> ToList(int UserId, String ProcName = null!, int num = 0, int skip = 0)
        {
            IQueryable<Procedure> query = _context.Procedures
                .Where(e => !e.IsArchived);     // only non-archived procedures
                
            // only procedures of this user
            if (UserId != 0)
                query = query.Where(e => e.RespId == UserId);

            // filter by name of procedures
            if (!String.IsNullOrEmpty(ProcName)) {
                query = query
                    .Where(e => e.Name!.Contains(ProcName));
            }

            // sort by target date, first: due soon
            query = query
                .OrderBy(e => e.TargetDate);

            // Pagination: skip first 'skip' elements, show next 'num' elements
            if (skip > 0)
                query = query.Skip(skip);
            if (num > 0)
                query = query.Take(num);

            // include relationships with other models
            query = query
                .Include(e => e.Refer)
                .Include(e => e.Resp);
            
            // execute query
            return await query.ToListAsync();
        }

        /// <summary>
        /// Reads the Procedure with the specified Id from database.
        /// </summary>
        /// <param name="id">Id of the Procedure</param>
        /// <returns>
        /// The desired Procedure. If no such Procedure exists, returns null.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert, Felix
        /// </remarks>
        public async Task<Procedure?> Find(int id)
        {
            return await _context.Procedures
                .Include(e => e.TaskInsts)
                    .ThenInclude(task => task.Resp)
                        .ThenInclude(resp => resp.Role)
                .Include(e => e.TaskInsts)
                    .ThenInclude(task => task.Resp)
                        .ThenInclude(resp => resp.User)
                .Include(e => e.Refer)
                .Include(e => e.Resp)
                .Include(e => e.FutureDepartment)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Determines wether a Procedure with the specified Id exists in the database.
        /// </summary>
        /// <param name="id">Id of the Procedure</param>
        /// <returns>
        /// True, if the Procedure exists; false, otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<bool> Exists(int id)
        {
            var element = await _context.Procedures.FindAsync(id);
            return element != null;
        }

        /// <summary>
        /// Adds a new Procedure to the database.
        /// </summary>
        /// <param name="element">The new Procedure to be added</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Add(Procedure element)
        {
            _context.Procedures.Add(element);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Writes the updated properties of the specified Procedure to the database.
        /// </summary>
        /// <param name="element">The Procedure to be updated</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Update(Procedure element)
        {
            _context.Procedures.Update(element);
            /* Alternativen: bei kurz- bzw. langlebigem context!?
            _context.Set<T>().Update(element);
            oder:
            _context.Set<T>().Entry(element).State = EntityState.Modified;
            oder:
            var oldElement = await _context.Set<T>().FindAsync(element.id);
            if (oldElement != null)
                _context.Set<T>().Entry(oldElement).CurrentValues.SetValues(element);
            */
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the Procedure with the specified Id form the database.
        /// </summary>
        /// <param name="id">Id of the Procedure to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(int id)
        {
            var element = await _context.Procedures.FindAsync(id);
            if (element != null) {
                _context.Procedures.Remove(element);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes the specified Procedure from the database.
        /// </summary>
        /// <param name="element">Procedure to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(Procedure element)
        {
            _context.Procedures.Remove(element);
            await _context.SaveChangesAsync();
        }

        // Methode zum Abrufen archivierter Vorgänge
        // Made by Felix Linse
        public async Task<List<Procedure>> GetArchivedProcedureAsync()
        {
            return await _context.Procedures
            .Where(p => p.IsArchived)
            .Include(p => p.TaskInsts)
            .Include(p => p.Resp)
            .ToListAsync();
        }

        /// <summary>
        /// Archives a procedure based on the provided ID.
        /// </summary>
        /// <param name="id">ID of the procedure to archive.</param>
        /// <param name="userId">ID of the user attempting to archive the procedure.</param>
        /// <param name="isAdmin">Boolean indicating if the user is admin.</param>
        /// <remarks>
        /// Made by Felix
        /// </remarks>
        public async Task ArchiveProcedureAsync(int id, int userId, bool isAdmin)
        {
            var procedure = await _context.Procedures
                .Include(p => p.TaskInsts)
                .FirstOrDefaultAsync(p => p.Id == id);
                
            if (procedure != null)
            {
                if(procedure.RespId != userId && !isAdmin)
                {
                    throw new UnauthorizedAccessException("User ist nicht berechtigt.");
                }
                procedure.IsArchived = true;
                foreach (var task in procedure.TaskInsts)
                {
                    task.IsArchived = true;
                }
                _context.Procedures.Update(procedure);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Method for creating a drop down menu with all
        /// Procedures the current user is responsible for.
        /// </summary>
        /// <param name="UserId">Id of current user</param>
        /// <returns>
        /// List of the user's Procedures as SelecListItems
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<List<SelectListItem>> GetSelectList(int UserId)
        {
            var myProcs = await _context.Procedures
                .Where(e => !e.IsArchived)
                .Where(e => e.RespId == UserId)
                .Include(e => e.Refer)
                .Select(e => new SelectListItem() {
                    Value = e.Id.ToString(),
                    Text = e.Name + $" ({e.Refer!.FirstName} {e.Refer!.LastName})"
                })
                .ToListAsync();
            return myProcs;
        }

        /// <summary>
        /// Imports Procedures from a JSON text into the database.
        /// </summary>
        /// <param name="text">JSON text containing procedure data</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public void ImportFromJSON(string text)
        {
            Console.WriteLine("DEBUG: ImportFromJSON: Procedure");

            var procs = JsonSerializer.Deserialize<List<Procedure>>(text);
            if (procs != null) {
                foreach (var proc in procs) {
                    Console.Write($"  importing: Id={proc.Id}, Name={proc.Name}");
                    using (var context = new ReplayContext())
                    {
                        context.Procedures.Add(proc);
                        context.SaveChanges();
                    }
                    Console.Write($"\n");
                }
            }
            Console.WriteLine("DEBUG: ImportFromJSON: end");
        }

        /// <summary>
        /// Exports all Procedures in the database to JSON text.
        /// </summary>
        /// <param name="options">options on how to generate the text. Default: null</param>
        /// <returns>
        /// JSON text containing procedure data
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public string ExportToJSON(JsonSerializerOptions? options = null)
        {
            string text = JsonSerializer.Serialize<List<Procedure>>(
                _context.Procedures
                    .Include(p => p.TaskInsts)
                        .ThenInclude(t => t.Resp)
                    .Include(p => p.TaskInsts)
                        .ThenInclude(t => t.Notes)
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
}
