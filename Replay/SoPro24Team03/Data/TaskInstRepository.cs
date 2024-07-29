using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop.Infrastructure;
using SoPro24Team03.Models;


namespace SoPro24Team03.Data
{
    /// <summary>
    /// Allows accessing TaskInsts in the database.
    /// </summary>
    public class TaskInstRepository : ITaskInstRepository
    {
        /// <value>Context for accessing the database.</value>
        protected readonly ReplayContext _context;

        public TaskInstRepository()
        {
            _context = new ReplayContext();
        }

        public TaskInstRepository(ReplayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Reads a list of all TaskInsts sorted by target date from the database.
        /// </summary>
        /// <returns>
        /// List of TaskInsts.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<List<TaskInst>> ToList()
        {
            return await ToList(0);
        }

        /// <summary>
        /// Reads a filtered list of TaskInsts sorted by target date from the database.
        /// </summary>
        /// <param name="UserId">Id of the user, that the tasks should be filtered for. Default: 0, show tasks of all users.</param>
        /// <param name="TaskName">String, that the tasks' names must contain. Default: null, no filtering.</param>
        /// <param name="ProcedureId">Id of the procedure, that the tasks should be filtered for. Default: 0, show tasks of all procedures.</param>
        /// <param name="OnlyPersonal">
        ///     True: only show tasks, that the user is personally responsible for. 
        ///     False: also show tasks associated with the user by roles or procedures.
        /// </param>
        /// <param name="num">The maximum number of elements to be returned. For Pagination. Default: 0, return all (remaining) elements.</param>
        /// <param name="skip">The number of elements to be skipped. For Pagination. Default: 0, skip no elements.</param>
        /// <returns>
        /// List of TaskInsts.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<List<TaskInst>> ToList(
            int UserId, String TaskName = null!,
            int ProcedureId = 0, bool OnlyPersonal = false,
            int num = 0, int skip = 0)
        {
            IQueryable<TaskInst> query = _context.TaskInsts
                .Where(e => !e.IsArchived);     // only non-archived tasks

            if (ProcedureId != 0) {
                // only tasks from this procedure
                query = query
                    .Where(e => e.ProcedureId == ProcedureId);
            }
            if (OnlyPersonal && UserId != 0) {
                // only tasks of this user
                query = query
                    .Include(e => e.Resp)
                    .Where(e => e.Resp.UserId == UserId);
            }

            if (ProcedureId == 0 && !OnlyPersonal && UserId != 0) {
                // tasks of this user and their roles
                var ThisUser = await _context.Users
                        .Where(u => u.Id == UserId)
                        .Include(u => u.Roles)
                        .FirstAsync();
                var RoleIds = ThisUser.Roles.Select(r => r.Id);

                query = query
                    .Include(e => e.Resp)
                    .Where(e => e.Resp.UserId == UserId || RoleIds.Contains(e.Resp.RoleId.GetValueOrDefault(-1)));
            }

            // filter by name of task
            if (!String.IsNullOrEmpty(TaskName)) {
                query = query
                    .Where(e => e.Name!.Contains(TaskName));
            }
   
            // sort by target date, first: due soon
            query = query
                .OrderBy(e => e.TargetDate);

            // Pagination: skip first 'skip' elements, show next 'num' elements
            if (skip != 0)
                query = query.Skip(skip);
            if (num != 0)
                query = query.Take(num);
            
            // include relationships with other models
            query = query
                .Include(e => e.Procedure)
                    .ThenInclude(proc => proc.Refer)
                .Include(e => e.Resp)
                    .ThenInclude(resp => resp.Role)
                .Include(e => e.Resp)
                    .ThenInclude(resp => resp.User);

            // execute query
            return await query.ToListAsync();
        }
        
        /// <summary>
        /// Reads the TaskInst with the specified Id from database.
        /// </summary>
        /// <param name="id">Id of the TaskInst</param>
        /// <returns>
        /// The desired TaskInst. If no such TaskInst exists, returns null.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<TaskInst?> Find(int id)
        {
            return await _context.TaskInsts
                .Include(e => e.Resp)
                    .ThenInclude(resp => resp.Role)
                .Include(e => e.Resp)
                    .ThenInclude(resp => resp.User)
                .Include(e => e.Procedure)
                .Include(e => e.Template)
                .Include(e => e.Notes!)
                    .ThenInclude(note => note.Author)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        /// <summary>
        /// Determines wether a TaskInst with the specified Id exists in the database.
        /// </summary>
        /// <param name="id">Id of the TaskInst</param>
        /// <returns>
        /// True, if the TaskInst exists; false, otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<bool> Exists(int id)
        {
            var element = await _context.TaskInsts.FindAsync(id);
            return element != null;
        }

        /// <summary>
        /// Adds a new TaskInst to the database.
        /// </summary>
        /// <param name="element">The new TaskInst to be added</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Add(TaskInst element)
        {
            _context.TaskInsts.Add(element);

            // update task counters in procedure
            var proc = await _context.Procedures.FirstAsync(p => p.Id == element.ProcedureId);
            proc.NumTasksTotal += 1;
            proc.NumTasksDone += (element.Status == CompletionStatus.done) ? 1 : 0;
            _context.Procedures.Update(proc);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Writes the updated properties of the specified TaskInst to the database.
        /// </summary>
        /// <param name="element">The TaskInst to be updated</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Update(TaskInst element)
        {
            _context.TaskInsts.Update(element);

            // update task counters in procedure
            var proc = await _context.Procedures.FirstAsync(p => p.Id == element.ProcedureId);
            var before = (TaskInst) _context.TaskInsts.Entry(element).OriginalValues.ToObject();
            var after = (TaskInst) _context.TaskInsts.Entry(element).CurrentValues.ToObject();
            if (before.Status == CompletionStatus.done && after.Status !=  CompletionStatus.done) {
                proc.NumTasksDone += -1;
            }
            else if (before.Status != CompletionStatus.done && after.Status ==  CompletionStatus.done) {
                proc.NumTasksDone += +1;
            }
            _context.Procedures.Update(proc);

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the TaskInst with the specified Id form the database.
        /// </summary>
        /// <param name="id">Id of the TaskInst to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(int id)
        {
            var element = await _context.TaskInsts.FindAsync(id);
            if (element != null) {
                // update task counters in procedure
                var proc = await _context.Procedures.FirstAsync(p => p.Id == element.ProcedureId);
                proc.NumTasksTotal -= 1;
                proc.NumTasksDone -= (element.Status == CompletionStatus.done) ? 1 : 0;
                _context.Procedures.Update(proc);

                _context.TaskInsts.Remove(element);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes the specified TaskInst from the database.
        /// </summary>
        /// <param name="element">TaskInst to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(TaskInst element)
        {
            // update task counters in procedure
            var proc = await _context.Procedures.FirstAsync(p => p.Id == element.ProcedureId);
            proc.NumTasksTotal -= 1;
            proc.NumTasksDone -= (element.Status == CompletionStatus.done) ? 1 : 0;
            _context.Procedures.Update(proc);
            
            _context.TaskInsts.Remove(element);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Reads the Note with the specified Id from database.
        /// </summary>
        /// <param name="id">Id of the Note</param>
        /// <returns>
        /// The desired Note. If no such Note exists, returns null.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<Note?> FindNote(int id)
        {
            return await _context.TaskInstNotes.FindAsync(id);
        }

        /// <summary>
        /// Adds a new Note to the database.
        /// </summary>
        /// <param name="element">The new Note to be added</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task AddNote(Note newNote)
        {
            _context.TaskInstNotes.Add(newNote);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Writes the updated properties of the specified Note to the database.
        /// </summary>
        /// <param name="element">The Note to be updated</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task UpdateNote(Note note)
        {
            _context.TaskInstNotes.Update(note);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the specified Note from the database.
        /// </summary>
        /// <param name="element">Note to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task RemoveNote(Note note)
        {
            _context.TaskInstNotes.Remove(note);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the Note with the specified Id form the database.
        /// </summary>
        /// <param name="id">Id of the Note to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task RemoveNote(int id)
        {
            var note = await _context.TaskInstNotes.FindAsync(id);
            if (note != null) {
                _context.TaskInstNotes.Remove(note);
                await _context.SaveChangesAsync();
            }
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