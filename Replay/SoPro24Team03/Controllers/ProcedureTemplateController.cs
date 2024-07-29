using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.Controllers;

[Authorize]
public class ProcedureTemplateController : Controller
{
    private IProcedureTemplateRepository _procedureTemplateRepository;
    private ITaskTemplateRepository _taskTemplRepository;
    private IRoleRepository _roleRepository;

    public ProcedureTemplateController(
        IProcedureTemplateRepository procedureTemplateRepository,
        ITaskTemplateRepository taskTemplRepository,
        IRoleRepository roleRepository
    )
    {
        _procedureTemplateRepository = procedureTemplateRepository;
        _taskTemplRepository = taskTemplRepository;
        _roleRepository = roleRepository;
    }

    // Made by Fabio
    // Anzeigen der Prozess Startseite
    public async Task<ViewResult?> Index()
    {
        var procedureTemplates = await _procedureTemplateRepository.ToList(HttpContext.User);

        if (procedureTemplates == null)
        {
            return null;
        }

        ProcTemplViewModel procTemplVM = new ProcTemplViewModel()
        {
            ProcedureTemplates = procedureTemplates
        };

        return View(procTemplVM);
    }

    // Made by Fabio
    // Partialview um einzelne Prozessdetails anzuzeigen
    public async Task<IActionResult?> Details(int id)
    {
        var procTempl = await _procedureTemplateRepository.Find(id);

        if (procTempl == null)
        {
            return NotFound();
        }

        var procTemplViewModel = new DetailsProcTemplViewModel()
        {
            Id = id,
            Name = procTempl.Name,
            TaskTemplates = procTempl.TaskTemplates,
            Roles = procTempl.Roles,
            IsArchived = procTempl.IsArchived
        };

        return PartialView("_Details", procTemplViewModel);
    }

    // Made by Fabio
    // Anzeigen der Prozesserstellen-View
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        var taskTemplates = await _taskTemplRepository.ToList();
        var roles = await _roleRepository.ToList();

        var viewModel = new CreateProcTemplViewModel()
        {
            Roles = roles.Select(Role => new SelectListItem
            {
                Value = Role.Id.ToString(),
                Text = Role.Name
            }).ToList(),
            TaskTemplates = taskTemplates
        };

        return View(viewModel);
    }

    // Made by Fabio
    // Action um ein Prozess zu erstellen aus dem ViewModel
    [Authorize(Roles = "Admin")]
    // POST: Teachingplan/Create
    [HttpPost]
    public async Task<IActionResult> Create([Bind("Name,SelectedTaskTemplates,SelectedRoles")] CreateProcTemplViewModel newProcTemplVM)
    {
        if (ModelState.IsValid)
        {
            var taskTemplateIds =
                newProcTemplVM.SelectedTaskTemplates
                .Split(',')
                .Select(int.Parse)
                .ToList();

            var taskTemplates = await Task.WhenAll(taskTemplateIds.Select(async id =>
            {
                var taskTemplate = await _taskTemplRepository.Find(id);

                if (taskTemplate == null)
                {
                    throw new Exception($"TaskTemplate with ID {id} not found!");
                }

                return taskTemplate;
            }));

            var roleIds =
                newProcTemplVM
                .SelectedRoles
                .ToList();

            var roles = await Task.WhenAll(roleIds.Select(async id =>
            {
                var role = await _roleRepository.Find(id);

                if (role == null)
                {
                    throw new Exception($"Role with ID {id} not found!");
                }

                return role;
            }));

            var newProcedureTemplate = new ProcedureTemplate
            {
                Name = newProcTemplVM.Name,
                IsArchived = false,
                TaskTemplates = taskTemplates.ToList(),
                Roles = roles.ToList(),
                OrderedTaskTemplateIds = newProcTemplVM.SelectedTaskTemplates
            };

            await _procedureTemplateRepository.Add(newProcedureTemplate);

            return RedirectToAction(nameof(Index));
        }

        newProcTemplVM.TaskTemplates = await _taskTemplRepository.ToList();

        var allRoles = await _roleRepository.ToList();
        newProcTemplVM.Roles = allRoles.Select(Role => new SelectListItem
        {
            Value = Role.Id.ToString(),
            Text = Role.Name
        }).ToList();

        return View(newProcTemplVM);
    }

    // Made by Fabio
    // Anzeigen der Prozess bearbeitenseite
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var procedureTemplate = await _procedureTemplateRepository.Find(id.Value);
        if (procedureTemplate == null)
        {
            return NotFound();
        }

        var roles = await _roleRepository.ToList();

        var procTemplVM = new EditProcTemplViewModel()
        {
            Name = procedureTemplate.Name,
            IsArchived = procedureTemplate.IsArchived,
            SelectedRoles = procedureTemplate.Roles.Select(r => r.Id).ToList(),
            SelectedTaskTemplateIds = procedureTemplate.OrderedTaskTemplateIds,
            Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name,
                Selected = procedureTemplate.Roles.Select(r => r.Id).Contains(r.Id)
            }).ToList(),
            TaskTemplates = await _taskTemplRepository.ToList()
        };

        return View(procTemplVM);
    }

    //Made by Fabio
    // Action um ein Prozess zu bearbeiten

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, 
        [Bind("Name,SelectedTaskTemplateIds,SelectedRoles,IsArchived")] 
        EditProcTemplViewModel modifiedProcTemplVM)
    {
        if (ModelState.IsValid)
        {
            var taskTemplateIds =
                modifiedProcTemplVM.SelectedTaskTemplateIds
                .Split(',')
                .Select(int.Parse)
                .ToList();

            var taskTemplates = await Task.WhenAll(taskTemplateIds.Select(async id =>
            {
                var taskTemplate = await _taskTemplRepository.Find(id);

                if (taskTemplate == null)
                {
                    throw new Exception($"TaskTemplate with ID {id} not found!");
                }

                return taskTemplate;
            }));

            var roleIds = modifiedProcTemplVM.SelectedRoles.ToList();

            var roles = await Task.WhenAll(roleIds.Select(async id =>
            {
                var role = await _roleRepository.Find(id);

                if (role == null)
                {
                    throw new Exception($"Role with ID {id} not found!");
                }

                return role;
            }));

            var procedureTemplate = new ProcedureTemplate
            {
                Id = id,
                Name = modifiedProcTemplVM.Name,
                IsArchived = modifiedProcTemplVM.IsArchived,
                OrderedTaskTemplateIds = modifiedProcTemplVM.SelectedTaskTemplateIds,
                TaskTemplates = taskTemplates.ToList(),
                Roles = roles.ToList()
            };

            await _procedureTemplateRepository.Update(procedureTemplate);

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }

        // Formular nochmal anzeigen (mit Fehlermeldung)
        return View(modifiedProcTemplVM);
    }

    // Made by Fabio
    // Action um ein Prozess zu löschen
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _procedureTemplateRepository.Remove(id);
        return RedirectToAction(nameof(Index));
    }
}