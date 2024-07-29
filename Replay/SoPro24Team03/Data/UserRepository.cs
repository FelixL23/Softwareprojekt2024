using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BC = BCrypt.Net.BCrypt;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;
using System.Text.Json;

namespace SoPro24Team03.Data
{
    public class UserRepository : IUserRepository
    {
        protected readonly ReplayContext _context;

        public UserRepository()
        {
            _context = new ReplayContext();
        }

        public UserRepository(ReplayContext context)
        {
            _context = context;
        }

        //Fuege Nutzer zur Liste hinzu
        //Made by Daniel H and Celina 
        public virtual async Task<List<User>> ToList()
        {
            return await _context.Users.Where(user => user.isArchived == false).Include(u => u.Roles).ToListAsync();
        }

        //Finde User über ID
        //Made by Daniel H und Celina
        public virtual async Task<User?> Find (int id)
        {
            return await _context.Users
                .Where(e => e.Id == id)
                .Where(user => user.isArchived == false)
                .Include(u => u.Roles)
                .FirstOrDefaultAsync();
        }

        //Finde User über Username
        //Made by Daniel H und Celina
        public virtual async Task<User?> Find(string userName)
        {
            return await _context.Users
                .Include(u => u.Roles)
                .Where(user => user.isArchived == false)
                .FirstOrDefaultAsync(e => e.UserName == userName);
        }

        //Existiert bereits ein User mit ID?
        //Made by Daniel H und Celina
        public virtual async Task<bool> Exists (int id)
        {
            var user = await Find(id);
            return user != null;
        }

        //Existiert bereits ein User mit Username?
        //Made by Daniel H und Celina
        public virtual async Task<bool> Exists (string userName)
        {
            var user = await Find(userName);
            return user != null;
        }

        //Made by Daniel H und Celina
        public virtual async Task Save()
        {
            await _context.SaveChangesAsync();
        }

        //Fuege Nutzer zur Datenbank hinzu
        //Made by Daniel H und Celina
        public virtual async Task Add(User newUser)
        {
            newUser.Roles?.ForEach(r => {
                _context.Entry(r).State = EntityState.Unchanged;
            });
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
        }

        //Gebe Rollen des aktuellen Users aus
        //Made by Daniel H und Celina
        public virtual async Task<string> GetUserRoles(string userName)
        {
            var user = await _context.Users.Where(e => e.UserName == userName).FirstOrDefaultAsync();
            return user.Roles.ToString();
        }

        public Task Remove (int id)
        {
            return null;
        }

        public Task Remove (User user)
        {
            return null;
        }

        //Weise dem User eine Rolle zu
        //Made by Daniel H
        public async Task AssignRole (int id, Role role) 
        {
            var user = await Find(id);
            if (user != null) 
            {
                user.Roles?.Add(role);
                await _context.SaveChangesAsync();
            }
        }

        //Entferne eine Rolle aus dem User
        //Made by Daniel H
        public async Task UnassignRole (int id, Role role)
        {
            var user = await Find(id);
            user?.Roles?.Remove(role);
            await _context.SaveChangesAsync();
        }


        // Retrieves a list of archived users. Felix
        public async Task<List<User>> GetArchivedUsersAsync()
        {
            return await _context.Users.Where(u => u.isArchived).ToListAsync();
        }

         // <summary>
        /// Archives a user based on the provided ID. Sets Flag of User
        /// </summary>
        /// <param name="id">ID of the user to archive.</param>
        /// Made by Felix
        /// </remarks>
        public async Task ArchiveUserAsync (int id)
        {
            var user = await _context.Users.FindAsync(id);
            var procedure = await _context.Procedures.Where(p => !p.IsArchived).Include(p => p.Resp).ToListAsync();
            if(procedure.Any(p => p.Resp.Id == id))
            {
                throw new Exception("User hat noch laufende Vorgänge");
            }

            if (user != null)
            {
                user.isArchived = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }

        }

        /// <summary>
        /// Method for creating a drop down menu with all Users.
        /// </summary>
        /// <returns>
        /// List of all Users as SelecListItems
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<List<SelectListItem>> GetSelectList()
        {
            var allUsers = await _context.Users
                .Where(e => !e.isArchived)
                .Include(u => u.Roles)
                .Select(e => new SelectListItem() {
                    Value = e.Id.ToString(),
                    Text = e.FirstName + " " + e.LastName
                })
                .ToListAsync();
            return allUsers;
        }

        //Weise dem User neue Werte zu
        //Made by Celina
        public async Task Update(User user)
        {
            var existingUser = await Find(user.Id);

            if (existingUser == null)
            {
                throw new Exception($"User with ID {user.Id} not found!");
            }
            
            _context.Entry(existingUser).CurrentValues.SetValues(user);

            if(user.Roles != null){
                foreach (var role in user.Roles)
                {
                    var existingRole = existingUser.Roles?.FirstOrDefault(r => r.Id == role.Id);

                    if (existingRole == null)
                    {
                        existingUser.Roles?.Add(role);
                    }
                    else
                    {
                        _context.Entry(existingRole).CurrentValues.SetValues(role);
                    }
                }

                existingUser.Roles?.RemoveAll(role => 
                    !user.Roles.Any(r => r.Id == role.Id)
                );
                
            } else{
                existingUser.Roles = null;
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Loads the currently logged in User from the database.
        /// If the user is not present in the database, or if he is
        /// suspended or archived, the session will be cleared and null
        /// is returned.
        /// </summary>
        /// <param name="httpContext">HttpContext from the calling Controller.</param>
        /// <returns>
        /// User currently logged in; null if no valid login.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<User?> LoadCurrentUser(HttpContext httpContext)
        {
            // load userId from Cookie
            int userId = int.Parse(httpContext.User.Identity!.Name!);
            
            // load User by Id from database
            var ThisUser = await this.Find(userId);
            if (ThisUser == null || ThisUser.isArchived || ThisUser.isSuspended) {
                // Logout
                Console.WriteLine("Invalidating Session-Cookie: " + ((ThisUser == null) ? "<null>" : ThisUser!.UserName));
                httpContext.Session.Clear();
                return null;
            }
            return ThisUser;
        }

        /// <summary>
        /// Imports Users from a JSON text into the database.
        /// </summary>
        /// <param name="text">JSON text containing user data</param>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public void ImportFromJSON(string text)
        {
            Console.WriteLine("DEBUG: ImportFromJSON: User");

            var users = JsonSerializer.Deserialize<List<User>>(text);
            if (users != null) {
                foreach (var user in users) {
                    List<int> roleIds = user.Roles.Select(r => r.Id).ToList();
                    user.Roles.Clear();
                    Console.Write($"  importing: Id={user.Id}, UserName={user.UserName}, Roles: [");
                    
                    using (var context = new ReplayContext())
                    {
                        foreach(var rid in roleIds) {
                            var loadedRole = context.Roles.First(r => r.Id == rid);
                            user.Roles.Add(loadedRole);
                            Console.Write($"({loadedRole.Id}), ");
                        }
                        context.Users.Add(user);
                        context.SaveChanges();
                    }
                    Console.Write($"]\n");
                }
            }
            Console.WriteLine("DEBUG: ImportFromJSON: end");
        }

        /// <summary>
        /// Exports all Users in the database to JSON text.
        /// </summary>
        /// <param name="options">options on how to generate the text. Default: null</param>
        /// <returns>
        /// JSON text containing user data
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public string ExportToJSON(JsonSerializerOptions? options = null)
        {
            string text = JsonSerializer.Serialize<List<User>>(
                _context.Users
                    .Include(u => u.Roles)
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