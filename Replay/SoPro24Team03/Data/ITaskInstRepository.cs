using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Models;

namespace SoPro24Team03.Data
{
    /// <summary>
    /// Allows accessing TaskInsts in the database.
    /// Abstraction of TaskInstRepository. For testing.
    /// </summary>
    public interface ITaskInstRepository : IRepository<TaskInst>
    {
        public Task<List<TaskInst>> ToList(
            int UserId, String TaskName = null!,
            int ProcedureId = 0, bool OnlyPersonal = false,
            int num = 0, int skip = 0);

        public Task<Note?> FindNote(int id);
        public Task AddNote(Note newNote);
        public Task UpdateNote(Note note);
        public Task RemoveNote(Note note);
        public Task RemoveNote(int id);
    }
}