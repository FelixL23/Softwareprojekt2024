using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.Controllers;

[Authorize(Roles = "Admin")]
public class TaskTemplateController : Controller
{
    private ITaskTemplateRepository _repository;
    private IRoleRepository _roleRepository;
    private IDepartmentRepository _departmentRepository;

    public TaskTemplateController(ITaskTemplateRepository repository, IRoleRepository roleRepository, IDepartmentRepository departmentRepository)
    {
        _repository = repository;
        _roleRepository = roleRepository;
        _departmentRepository = departmentRepository;
    }

    // Made by Fabio
    // Anzeigen der Aufgabenvorlagen auf der Index-Seite
    public async Task<ViewResult> Index()
    {
        TaskTemplateViewModel taskTemplateViewModel = new TaskTemplateViewModel()
        {
            TaskTemplates = await _repository.ToList()
        };

        return View(taskTemplateViewModel);
    }

    // Made by Fabio
    // Partialview um die Anleitung für einzelne Aufgabenvorlagen anzuzeigen
    public async Task<ActionResult?> GetInstruction(int id)
    {
        var taskTemplate = await _repository.Find(id);

        if (taskTemplate == null || taskTemplate.Instruction == null)
        {
            return NotFound();
        }

        return PartialView("_Instruction", taskTemplate.Instruction);
    }

    // Made by Fabio
    // Anzeigen der ErstellenView
    public async Task<ViewResult> Create()
    {
        var roles = await _roleRepository.ToList();

        var taskTemplateViewModel = new CreateTaskTemplateViewModel
        {
            Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList(),
            Departments = await _departmentRepository.GetSelectList()
        };

        return View(taskTemplateViewModel);
    }

    // Made by Fabio
    // Action um eine Aufgabenvorlage aus dem ViewModel zu erstellen
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Name,DueType,ContractTypes,SelectedDepartmentIds,Instruction,TaskResponsibleType,TaskResponsibleRoleId,CustomDays")]
        CreateTaskTemplateViewModel newTaskTemplateVM)
    {
        if (ModelState.IsValid)
        {
            var departments = new List<Department>();

            if (newTaskTemplateVM.SelectedDepartmentIds != null && newTaskTemplateVM.SelectedDepartmentIds.Count() != 0)
            {
                departments = (await Task.WhenAll(newTaskTemplateVM.SelectedDepartmentIds.Select(async id =>
                {
                    var department = await _departmentRepository.Find(id);

                    if (department == null)
                    {
                        throw new Exception($"Role with ID {id} not found!");
                    }

                    return department;
                }))).ToList();
            }

            var taskTemplate = new TaskTemplate()
            {
                Name = newTaskTemplateVM.Name,
                DueType = newTaskTemplateVM.DueType,
                CustomDays = newTaskTemplateVM.CustomDays,
                ContractTypes = newTaskTemplateVM.ContractTypes,
                Instruction = newTaskTemplateVM.Instruction,
                Departments = departments.ToList(),
                TaskResponsible = new TaskResponsible
                {
                    TaskRespType = newTaskTemplateVM.TaskResponsibleType,
                    Role = null
                },
                IsArchived = false
            };

            if (newTaskTemplateVM.TaskResponsibleType == TaskRespType.role)
            {
                taskTemplate.TaskResponsible.Role = await _roleRepository.Find(newTaskTemplateVM.TaskResponsibleRoleId);
            }
            else
            {
                taskTemplate.TaskResponsible.Role = null;
            }

            await _repository.Add(taskTemplate);

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }

        // Formular nochmal anzeigen (mit Fehlermeldung)
        return View(newTaskTemplateVM);
    }

    // Made by Fabio
    // Anzeigen der BearbeitenView von Aufgabenvorlagen
    public async Task<ViewResult?> Edit(int id)
    {
        var taskTemplate = await _repository.Find(id);

        if (taskTemplate == null)
        {
            return null;
        }

        var roles = await _roleRepository.ToList();
        var departmentSelectList = await _departmentRepository.GetSelectList();

        var taskTemplateViewModel = new EditTaskTemplateViewModel()
        {
            Name = taskTemplate.Name,
            DueType = taskTemplate.DueType,
            ContractTypes = taskTemplate.ContractTypes,
            Instruction = taskTemplate.Instruction,
            Roles = roles.Select(r => new SelectListItem
            {
                Value = r.Id.ToString(),
                Text = r.Name
            }).ToList(),
            SelectedDepartmentIds = taskTemplate.Departments.Select(d => d.Id).ToList(),
            Departments = departmentSelectList.Select(d =>
            {
                d.Selected = taskTemplate.Departments.Select(d => d.Id).Contains(int.Parse(d.Value));

                return d;
            }).ToList(),
            TaskResponsibleRoleId = taskTemplate.TaskResponsible!.RoleId ?? 0,
            TaskResponsibleType = taskTemplate.TaskResponsible.TaskRespType,
            CustomDays = taskTemplate.CustomDays
        };

        return View(taskTemplateViewModel);
    }

    // Made by Fabio
    // Action um eine Aufgabenvorlage zu editieren
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Name,DueType,ContractTypes,SelectedDepartmentIds,Instruction,TaskResponsibleType,TaskResponsibleRoleId,CustomDays")]
        EditTaskTemplateViewModel modifiedTaskTemplateVM)
    {
        if (ModelState.IsValid)
        {
            var departments = new List<Department>();

            // Überprüfung ob Departments ausgewählt worden 
            if (modifiedTaskTemplateVM.SelectedDepartmentIds != null && modifiedTaskTemplateVM.SelectedDepartmentIds.Count() != 0)
            {
                departments = (await Task.WhenAll(modifiedTaskTemplateVM.SelectedDepartmentIds.Select(async id =>
                {
                    var department = await _departmentRepository.Find(id);

                    if (department == null)
                    {
                        throw new Exception($"Role with ID {id} not found!");
                    }

                    return department;
                }))).ToList();
            }

            var taskResponsible = await _repository.GetTaskResponsible(id);

            var taskTemplate = new TaskTemplate()
            {
                Id = id,
                Name = modifiedTaskTemplateVM.Name,
                DueType = modifiedTaskTemplateVM.DueType,
                CustomDays = modifiedTaskTemplateVM.CustomDays,
                ContractTypes = modifiedTaskTemplateVM.ContractTypes,
                Instruction = modifiedTaskTemplateVM.Instruction,
                TaskResponsibleId = taskResponsible.Id,
                TaskResponsible = taskResponsible,
                Departments = departments.ToList()
            };

            // Falls der Typ des Aufgabenständigen die Rolle ist, setzte die rolle, ansonsten setze die Rolle auf null
            if (modifiedTaskTemplateVM.TaskResponsibleType == TaskRespType.role)
            {
                taskTemplate.TaskResponsible.TaskRespType = modifiedTaskTemplateVM.TaskResponsibleType;
                taskTemplate.TaskResponsible!.Role = await _roleRepository.Find(modifiedTaskTemplateVM.TaskResponsibleRoleId);
            }
            else
            {
                taskTemplate.TaskResponsible.TaskRespType = modifiedTaskTemplateVM.TaskResponsibleType;
                taskTemplate.TaskResponsible.Role = null;
            }

            await _repository.Update(taskTemplate);

            // Weiterleitung zur Übersicht
            return RedirectToAction(nameof(Index));
        }

        // Formular nochmal anzeigen (mit Fehlermeldung)
        return View(modifiedTaskTemplateVM);
    }

}