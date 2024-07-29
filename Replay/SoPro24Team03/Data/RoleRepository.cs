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
    public class RoleRepository : IRoleRepository
    {
        protected readonly ReplayContext _context;

        public RoleRepository()
        {
            _context = new ReplayContext();
        }

        public RoleRepository(ReplayContext context)
        {
            _context = context;
        }

        public virtual async Task<List<Role>> ToList()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> Find(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public virtual async Task<bool> Exists(int id)
        {
            var element = await _context.Roles.FindAsync(id);
            return element != null;
        }

        public virtual async Task Add(Role element)
        {
            _context.Roles.Add(element);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Update(Role element)
        {
            _context.Roles.Update(element);
            await _context.SaveChangesAsync();
        }

        public virtual async Task Remove(int id)
        {
            var element = await _context.Roles.FindAsync(id);
            if (element != null) {
                _context.Roles.Remove(element);
                await _context.SaveChangesAsync();
            }
        }

        public virtual async Task Remove(Role element)
        {
            _context.Roles.Remove(element);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.ToListAsync();
        }

        /// <summary>
        /// Method for creating a drop down menu with all Roles.
        /// </summary>
        /// <returns>
        /// List of all Roles as SelecListItems
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<List<SelectListItem>> GetSelectList()
        {
            var allRoles = await _context.Roles
                .Select(e => new SelectListItem() {
                    Value = e.Id.ToString(),
                    Text = e.Name
                })
                .ToListAsync();
            return allRoles;
        }

        /// <summary>
        /// Imports Roles from a JSON text into the database.
        /// </summary>
        /// <param name="text">JSON text containing role data</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public void ImportFromJSON(string text)
        {
            var roles = JsonSerializer.Deserialize<List<Role>>(text);
            if (roles != null) {
                _context.AddRange(roles);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Exports all Roles in the database to JSON text.
        /// </summary>
        /// <param name="options">options on how to generate the text. Default: null</param>
        /// <returns>
        /// JSON text containing role data
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public string ExportToJSON(JsonSerializerOptions? options = null)
        {
            string text = JsonSerializer.Serialize<List<Role>>(
                _context.Roles
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