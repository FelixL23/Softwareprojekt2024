using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;

namespace SoPro24Team03.Controllers;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private IRoleRepository _roleRepository;
    private IUserRepository _userRepository;
    // Made by Philip
    public RoleController(IRoleRepository roleRepository, IUserRepository userRepository)
    {   
        _roleRepository = roleRepository;
        _userRepository = userRepository;
    }

    public async Task<ViewResult> Index()
    {
        RoleViewModel rVM = new RoleViewModel() {
            Roles = await _roleRepository.ToList()
        };
        return View(rVM);
    }

    // Made by Philip
    // GET: Role/Create
    public async Task<ViewResult> Create()
    {
        var users = await _roleRepository.GetAllUsers();
        CreateRoleViewModel rVM = new CreateRoleViewModel()
        {
            AllUsers = users
        };
        return View(rVM);
    }

    // Made by Philip
    // POST: Role/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,SelectedUsers")] CreateRoleViewModel newRoleVM)
    {
        if (ModelState.IsValid)
        {
            var newRole = new Role() {
                Name = newRoleVM.Name
            };
            await _roleRepository.Add(newRole);

            foreach(var userId in newRoleVM.SelectedUsers)
            {
                await _userRepository.AssignRole(userId, newRole);
            }

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }
        // Formular nochmal anzeigen (mit Fehlermeldung)
        return View(newRoleVM);
    }

    // Made by Philip
    // GET: Role/Edit/3
    public async Task<ViewResult?> Edit(int id)
    {
        var role = await _roleRepository.Find(id);
        if (role == null)
        {
            return null;
        }
        var roleVM = new EditRoleViewModel()
        {
               Name = role.Name
        };
        return View(roleVM);
    }

    // Made by Philip
    // POST: Role/Edit/3
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Perms")] EditRoleViewModel modifiedRoleVM)
    {
        if (ModelState.IsValid)
        {
            var modifiedRole = new Role() {
                Id = id,
                Name = modifiedRoleVM.Name
            };
            await _roleRepository.Update(modifiedRole);

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }
        // Formular nochmal anzeigen (mit Fehlermeldung)
        return View(modifiedRoleVM);
    }

}
