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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace SoPro24Team03.Controllers;


[Authorize(Roles = "Admin")]
public class UsermanagementController : Controller
{
    private IUserRepository _userRepo;
    private IRoleRepository _roleRepo;
    private IDepartmentRepository _depRepo;
    private ISessionService _sessionService;

    public UsermanagementController(IUserRepository userRepo, IRoleRepository roleRepo, IDepartmentRepository depRepo, ISessionService sessionService)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _depRepo = depRepo;
        _sessionService = sessionService;
    }

    //Made by Celina
    //Anzeigen aller User in der Overview
    public async Task<ViewResult> UserOverview()
    {
        var allUsers = await _userRepo.ToList();
        var allRoles = await _roleRepo.ToList();
        var viewModel = new UserOverviewViewModel()
        {
            Users = allUsers,
            Roles = allRoles
        };
        return View(viewModel);
    }

    //Made by Celina
    //Anzeigen von Werten in der Addview
    public async Task<ViewResult> UserAdd()
    {
        var view = new UserAddViewModel()
        {
            AllRoles = await _roleRepo.GetSelectList(),
            AllDepartments = await _depRepo.GetSelectList(),
            ContractTypeList = new SelectList(Enum.GetValues(typeof(ContractType))
            .Cast<ContractType>()
            .Select(ct => new SelectListItem
            {
                Value = ct.ToString(),
                Text = ct.GetDisplayName()
            }).ToList(), "Value", "Text")
        };
        return View(view);
    }

    //Made by Celina
    //Übernehmen der Werte aus der Addview und Anlegen eines neuen Users
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserAdd(
        [Bind("FirstName, LastName, Password, DateOfBirth, ContractStart, ContractType, SelectedRoleIds, isSuspended, EmailAddress, SelectedDepartmentId")]
        UserAddViewModel viewModel)
    {
        if(ModelState.IsValid){

            var userName = Models.User.GenerateUserName(viewModel.FirstName, viewModel.LastName);

            // Überprüfung, ob der Benutzername bereits existiert
            var existingUser = await _userRepo.Find(userName);
            if (existingUser != null)
            {
                viewModel.AllRoles = await _roleRepo.GetSelectList();
                viewModel.AllDepartments = await _depRepo.GetSelectList();
                viewModel.ContractTypeList = new SelectList(Enum.GetValues(typeof(ContractType))
                .Cast<ContractType>()
                .Select(ct => new SelectListItem
                {
                    Value = ct.GetDisplayName(),
                    Text = ct.ToString()
                }).ToList(), "Value", "Text");
                return View(viewModel);
            }

            var roleIds = viewModel.SelectedRoleIds?.ToList();
            var roles = await Task.WhenAll(roleIds.Select(async id =>
            {
                var role = await _roleRepo.Find(id);

                if (role == null)
                {
                    throw new Exception($"Role with ID {id} not found!");
                }

                return role;
            }));

            var user = new User
            {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                DateOfBirth = viewModel.DateOfBirth,
                ContractStart = viewModel.ContractStart,
                ContractType = viewModel.ContractType,
                EmailAddress = viewModel.EmailAddress,
                isSuspended = viewModel.isSuspended,
                DepartmentId = viewModel.SelectedDepartmentId,
                Roles = roles?.ToList()
            };

            user.UserName = Models.User.GenerateUserName(user.FirstName, user.LastName);
            user.PasswordHash = Models.User.HashPassword(viewModel.Password);

            await _userRepo.Add(user);
            return RedirectToAction(nameof(UserOverview));
        }
        return RedirectToAction(nameof(UserAdd));
    }

    //Made by Celina
    //Anzeigen der user Daten in der Editview
    public async Task<IActionResult> UserEdit(int id)
    {
        var user = await _userRepo.Find(id);
        if (user == null)
        {
            return View("Error");
        }

        var roles = await _roleRepo.ToList();

        var viewModel = new UserEditViewModel()
        {
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            DateOfBirth = user.DateOfBirth,
            ContractStart = user.ContractStart,
            ContractType = user.ContractType,
            SelectedDepartmentId = user.DepartmentId,
            EmailAddress = user.EmailAddress,
            isSuspended = user.isSuspended,
            isArchived = user.isArchived,
            PasswordHash = user.PasswordHash,
            Notes = user.Notes,
            Roles = roles?.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name,
                Selected = user.Roles.Select(role => role.Id).Contains(r.Id),
            }).ToList(),
            AllDepartments = await _depRepo.GetSelectList(),
            ContractTypeList = new SelectList(Enum.GetValues(typeof(ContractType))
            .Cast<ContractType>()
            .Select(ct => new SelectListItem
            {
                Value = ct.ToString(),
                Text = ct.GetDisplayName()
            }).ToList(), "Value", "Text")
        };

        return View(viewModel);
    }

    //Made by Celina
    //Übernehmen der geänderten Daten aus der Editview
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UserEdit(int id,
        [Bind("UserName, FirstName, LastName, PasswordHash, DateOfBirth, ContractStart, ContractType, SelectedRoleIds, SelectedDepartmentId, isSuspended, isArchived, EmailAddress, Notes")]
        UserEditViewModel viewModel)
    {
        if (ModelState.IsValid)
        {

            var roleIds = viewModel.SelectedRoleIds?.ToList();

            //Falls Rolle vorhanden ist, neuen User mit entsprechender Rolle anlegen
            if(roleIds != null && roleIds.Count() != 0){
                var roles = await Task.WhenAll(roleIds.Select(async id =>
                {
                    var role = await _roleRepo.Find(id);

                    if (role == null)
                    {
                        throw new Exception($"Role with ID {id} not found!");
                    }

                    return role;
                }));
            

                var user = new User
                {
                    Id = id,
                    UserName = viewModel.UserName,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    PasswordHash = viewModel.PasswordHash,
                    DateOfBirth = viewModel.DateOfBirth,
                    ContractStart = viewModel.ContractStart,
                    ContractType = viewModel.ContractType,
                    DepartmentId = viewModel.SelectedDepartmentId,
                    EmailAddress = viewModel.EmailAddress,
                    isSuspended = viewModel.isSuspended,
                    isArchived = viewModel.isArchived,
                    Notes = viewModel.Notes,
                    Roles = roles?.ToList()
                };

                    if (viewModel.isSuspended)
                    {
                        await _sessionService.InvalidateUserSessions(id.ToString());
                    }

                    await _userRepo.Update(user);
                        
            } else {
                var user = new User
                {
                    Id = id,
                    UserName = viewModel.UserName,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    PasswordHash = viewModel.PasswordHash,
                    DateOfBirth = viewModel.DateOfBirth,
                    ContractStart = viewModel.ContractStart,
                    ContractType = viewModel.ContractType,
                    DepartmentId = viewModel.SelectedDepartmentId,
                    EmailAddress = viewModel.EmailAddress,
                    isSuspended = viewModel.isSuspended,
                    isArchived = viewModel.isArchived,
                    Notes = viewModel.Notes,
                    Roles = null
                };

                if (viewModel.isSuspended)
                {
                    await _sessionService.InvalidateUserSessions(id.ToString());
                }

                await _userRepo.Update(user);
            }

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(UserOverview));
        }
        return View(viewModel);
    }
}