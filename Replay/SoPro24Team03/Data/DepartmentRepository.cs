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
    /// Allows accessing Departments in the database.
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        /// <value>Context for accessing the database.</value>
        protected readonly ReplayContext _context;

        public DepartmentRepository()
        {
            _context = new ReplayContext();
        }

        public DepartmentRepository(ReplayContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Reads a list of all Departments from the database.
        /// </summary>
        /// <returns>
        /// List of Departments.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<List<Department>> ToList()
        {
            return await _context.Departments.ToListAsync();
        }

        /// <summary>
        /// Reads the Department with the specified Id from database.
        /// </summary>
        /// <param name="id">Id of the Department</param>
        /// <returns>
        /// The desired Department. If no such Department exists, returns null.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<Department?> Find(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        /// <summary>
        /// Determines wether a Department with the specified Id exists in the database.
        /// </summary>
        /// <param name="id">Id of the Department</param>
        /// <returns>
        /// True, if the Department exists; false, otherwise.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task<bool> Exists(int id)
        {
            var element = await _context.Departments.FindAsync(id);
            return element != null;
        }

        /// <summary>
        /// Adds a new Department to the database.
        /// </summary>
        /// <param name="element">The new Department to be added</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Add(Department element)
        {
            _context.Departments.Add(element);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Writes the updated properties of the specified Department to the database.
        /// </summary>
        /// <param name="element">The Department to be updated</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Update(Department element)
        {
            _context.Departments.Update(element);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the Department with the specified Id form the database.
        /// </summary>
        /// <param name="id">Id of the Department to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(int id)
        {
            var element = await _context.Departments.FindAsync(id);
            if (element != null) {
                _context.Departments.Remove(element);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Removes the specified Department from the database.
        /// </summary>
        /// <param name="element">Department to be removed</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public virtual async Task Remove(Department element)
        {
            _context.Departments.Remove(element);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Method for creating a drop down menu with all Departments.
        /// </summary>
        /// <returns>
        /// List of all Departments as SelecListItems
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<List<SelectListItem>> GetSelectList()
        {
            var allDeps = await _context.Departments
                .Select(e => new SelectListItem() {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToListAsync();
            return allDeps;
        }

        /// <summary>
        /// Imports Departments from a JSON text into the database.
        /// </summary>
        /// <param name="text">JSON text containing department data</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public void ImportFromJSON(string text)
        {
            var deps = JsonSerializer.Deserialize<List<Department>>(text);
            if (deps != null) {
                _context.AddRange(deps);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Exports all Departments in the database to JSON text.
        /// </summary>
        /// <param name="options">options on how to generate the text. Default: null</param>
        /// <returns>
        /// JSON text containing department data
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public string ExportToJSON(JsonSerializerOptions? options = null)
        {
            string text = JsonSerializer.Serialize<List<Department>>(
                _context.Departments
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