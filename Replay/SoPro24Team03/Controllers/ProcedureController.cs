using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;


namespace SoPro24Team03.Controllers;


/// <summary>
/// Controller handling requests with respect to procedures.
/// </summary>
[Authorize]
public class ProcedureController : Controller
{
    /// <value>repository for accessing procedures.</value>
    private IProcedureRepository _ProcRepo;

    /// <value>repository for accessing users.</value>
    private IUserRepository _UserRepo;

    /// <value>repository for accessing procedure templates.</value>
    private IProcedureTemplateRepository _procTemplRepo;

    /// <value>repository for accessing departments.</value>
    private IDepartmentRepository _depRepo;


    public ProcedureController(
        IProcedureRepository ProcRepo, IUserRepository UserRepo,
        IProcedureTemplateRepository procTemplRepo, IDepartmentRepository depRepo
        )
    {
        _ProcRepo = ProcRepo;
        _UserRepo = UserRepo;
        _procTemplRepo = procTemplRepo;
        _depRepo = depRepo;
    }

    /// <summary>
    /// HTTP-GET request: /Procedure
    /// Shows an overview of the current user's procedures.
    /// </summary>
    /// <returns>
    /// View for this request.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public async Task<ViewResult> Index()
    {
        // read userId from Cookie
        int userId = int.Parse(HttpContext.User.Identity!.Name!);
        bool isAdmin = HttpContext.User.IsInRole("Admin");

        var procVM = new ProcedureViewModel() {
            Procedures = await _ProcRepo.ToList(isAdmin ? 0 : userId)
        };
        return View(procVM);
    }

    /// <summary>
    /// HTTP-GET request: /Procedure/Create/id
    /// Shows a form for creating a new procedure from the
    /// specified procedure template.
    /// </summary>
    /// <param name="id">id of the procedure template</param>
    /// <returns>
    /// View for this request.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public async Task<IActionResult> Create(int? id)
    {
        if (id == null)
            return NotFound();

        var ThisUser = await _UserRepo.LoadCurrentUser(HttpContext);
        if (ThisUser == null)
            return RedirectToAction("Index", "Authentication");

        var procTemplate = await _procTemplRepo.Find(id.Value);
        if (procTemplate == null || !procTemplate.isUserAuthorized(ThisUser))
            return NotFound();

        var createVM = new CreateProcedureViewModel() {
            Name = procTemplate.Name,
            TargetDate = DateTime.Now,
            ReferId = ThisUser.Id,
            RespId = ThisUser.Id,
            AllDepartments = await _depRepo.GetSelectList(),
            AllUsers = await _UserRepo.GetSelectList()
        };
        return View(createVM);
    }

    /// <summary>
    /// HTTP-POST request: /Procedure/Create/id
    /// Accepts data from the form, creates the procedure,
    /// and writes it to the database.
    /// </summary>
    /// <param name="id">id of the procedure template</param>
    /// <param name="createVM">view model containing the data from the form</param>
    /// <returns>
    /// On success, redirects to overview; on error returns the view for the form again.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id,
        [Bind("Name,TargetDate,ContractType,FutureDepartmentId,ReferId,RespId")]
        CreateProcedureViewModel createVM)
    {
        if (ModelState.IsValid)
        {
            var procTemplate = await _procTemplRepo.Find(id);
            if (procTemplate == null)
                return NotFound();

            var newProc = procTemplate.CreateProcedure(
                createVM.Name!,
                createVM.TargetDate,
                createVM.RespId,
                createVM.ReferId,
                createVM.ContractType,
                createVM.FutureDepartmentId
            );
            await _ProcRepo.Add(newProc);
            
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            if (isAdmin || newProc.RespId == userId) {
                // edit the newly created procedure and it's tasks
                return RedirectToAction("Edit", "Procedure", new {id = newProc.Id});
            }
            else {
                // this user does not have permissions to edit the procedure
                return RedirectToAction("Index", "ProcedureTemplate");
            }
        }
        
        // Formular nochmal anzeigen (mit Fehlermeldung)
        createVM.AllDepartments = await _depRepo.GetSelectList();
        createVM.AllUsers = await _UserRepo.GetSelectList();
        return View(createVM);
    }

    /// <summary>
    /// HTTP-GET request: /Procedure/Edit/id
    /// Shows a form for editing the specified procedure.
    /// <param name="id">id of the procedure to edit.</param>
    /// <returns>
    /// View for this request.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();
        
        var element = await _ProcRepo.Find(id.Value);
        if (element == null)
            return NotFound();

        int userId = int.Parse(HttpContext.User.Identity!.Name!);
        bool isAdmin = HttpContext.User.IsInRole("Admin");
        if (!isAdmin && element.RespId != userId)
            return NotFound();

        var editVM = new EditProcedureViewModel() {
            Id = element.Id,
            Name = element.Name,
            FutureDepartmentId = element.FutureDepartmentId,
            ContractType = element.CreatedContractType,
            TargetDate = element.TargetDate,
            RespId = element.RespId,
            ReferId = element.ReferId,
            OrderedTaskInstIds = element.OrderedTaskInstIds,

            isAdmin = isAdmin,
            Tasks = element.TaskInsts,
            NumTasksDone = element.NumTasksDone,
            NumTasksTotal = element.NumTasksTotal,
            TemplateId = element.TemplateId,
            AllDepartments = await _depRepo.GetSelectList(),
            AllUsers = await _UserRepo.GetSelectList()
        };
        return View(editVM);
    }

    /// <summary>
    /// HTTP-POST request: /Procedure/Edit/id
    /// Accepts data from the form, changes the procedure,
    /// and writes it to the database.
    /// </summary>
    /// <param name="id">id of the procedure to edit.</param>
    /// <param name="modifiedElementVM">view model containing the data from the form</param>
    /// <returns>
    /// On success, redirects to overview; on error returns the view for the form again.
    /// </returns>
    /// <remarks>
    /// Made by Daniel Albert
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Name,TargetDate,ContractType,FutureDepartmentId,RespId,ReferId,OrderedTaskInstIds")]
        EditProcedureViewModel modifiedElementVM)
    {
        var element = await _ProcRepo.Find(id);
        if (element == null)
            return NotFound();
        
        int userId = int.Parse(HttpContext.User.Identity!.Name!);
        bool isAdmin = HttpContext.User.IsInRole("Admin");
        if (!isAdmin && element.RespId != userId)
            return NotFound();

        if (modifiedElementVM.TargetDate != element.TargetDate &&
            modifiedElementVM.TargetDate <= DateTime.Now) {
            ModelState.AddModelError("TargetDate",
                "Fehler! Die Frist muss in der Zukunft liegen.");
        }

        if (ModelState.IsValid)
        {
            if (isAdmin) {
                element.Name = modifiedElementVM.Name;
                element.TargetDate = modifiedElementVM.TargetDate;
                element.CreatedContractType = modifiedElementVM.ContractType;
                element.FutureDepartmentId = modifiedElementVM.FutureDepartmentId;
                element.RespId = modifiedElementVM.RespId;
                element.ReferId = modifiedElementVM.ReferId;
            }
            element.OrderedTaskInstIds = modifiedElementVM.OrderedTaskInstIds;
            await _ProcRepo.Update(element);

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }
        
        // Formular nochmal anzeigen (mit Fehlermeldung)
        modifiedElementVM.isAdmin = isAdmin;
        modifiedElementVM.Tasks = element.TaskInsts;
        modifiedElementVM.NumTasksDone = element.NumTasksDone;
        modifiedElementVM.NumTasksTotal = element.NumTasksTotal;
        modifiedElementVM.TemplateId = element.TemplateId;
        modifiedElementVM.AllDepartments = await _depRepo.GetSelectList();
        modifiedElementVM.AllUsers = await _UserRepo.GetSelectList();
        return View(modifiedElementVM);
    }
    /// <summary>
    /// HTTP-POST request: /Procedure/Archive/id
    /// Archives a procedure based on the provided ID.
    /// </summary>
    /// <param name="id">ID of the procedure to archive.</param>
    /// <returns>
    /// On success, redirects to the index view; on unauthorized access, adds an error message and redirects to the index view.
    /// </returns>
    /// <remarks>
    /// Made by Felix
    /// </remarks>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id)
    {
        try
        {
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            await _ProcRepo.ArchiveProcedureAsync(id, userId, isAdmin);
            return RedirectToAction(nameof(Index));
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        return RedirectToAction(nameof(Index));
    }

}
