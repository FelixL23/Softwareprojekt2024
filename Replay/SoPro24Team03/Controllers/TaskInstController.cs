using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;


namespace SoPro24Team03.Controllers
{
    /// <summary>
    /// Controller handling requests with respect to tasks.
    /// </summary>
    [Authorize]
    public class TaskInstController : Controller
    {
        /// <value>repository for accessing tasks.</value>
        private ITaskInstRepository _TaskRepo;

        /// <value>repository for accessing users.</value>
        private IUserRepository _UserRepo;

        /// <value>repository for accessing roles.</value>
        private IRoleRepository _RoleRepo;

        /// <value>repository for accessing procedures.</value>
        private IProcedureRepository _ProcRepo;

        /// <value>repository for accessing task templates.</value>
        private ITaskTemplateRepository _TaskTemplRepo;


        public TaskInstController(
            ITaskInstRepository TaskRepo, IUserRepository UserRepo,
            IRoleRepository RoleRepo, IProcedureRepository ProcRepo,
            ITaskTemplateRepository TaskTemplRepo)
        {
            _TaskRepo = TaskRepo;
            _UserRepo = UserRepo;
            _RoleRepo = RoleRepo;
            _ProcRepo = ProcRepo;
            _TaskTemplRepo = TaskTemplRepo;
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst
        /// Shows a filtered overview of the current user's tasks.
        /// If level is 0, only show tasks the user is personally responsible for.
        /// If level is 1, also show other tasks associated by role or procedure.
        /// If level is 2 (only for admins), show all tasks in the system.
        /// </summary>
        /// <param name="TaskName">Only show tasks containing this string.</param>
        /// <param name="ProcedureId">Only show tasks belonging to the procedure with this id.</param>
        /// <param name="level">Level (0-2) modifying how tasks are filtered.</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> Index(String? TaskName, int? ProcedureId, int? level)
        {
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            string filterStr = Request.Cookies["FilterTaskInst"] ?? "";
            Console.WriteLine("DEBUG: TaskInst/Index");
            Console.WriteLine($"  param:  TaskName:    {TaskName}");
            Console.WriteLine($"  param:  ProcedureId: {ProcedureId}");
            Console.WriteLine($"  param:  level:       {level}");
            Console.WriteLine($"  cookie: userId:    {userId}");
            Console.WriteLine($"  cookie: filterStr: {filterStr}");

            string outTaskName = "";
            int outProcedureId = 0;
            int outLevel = 1;

            if (TaskName == null && ProcedureId == null && level == null) {
                // read from cookie
                var strs = filterStr.Split(',');
                if (strs.Count() == 3) {
                    outTaskName = strs[0];
                    outProcedureId = int.Parse(strs[1]);
                    outLevel = int.Parse(strs[2]);
                }
            }
            else {
                // read from parameters
                if (TaskName != null)
                    outTaskName = TaskName;
                if (ProcedureId != null)
                    outProcedureId = ProcedureId.Value;
                if (level != null)
                    outLevel = level.Value;

                // update cookie
                filterStr = outTaskName + "," + outProcedureId.ToString() + "," + outLevel.ToString();
                Response.Cookies.Append("FilterTaskInst", filterStr);
            }

            List<TaskInst> taskList = null!;
            switch (outLevel) {
                case 2:     // 2: show all tasks (admin only)
                    if (isAdmin) {
                        taskList = await _TaskRepo.ToList(0, outTaskName, 0, false, 0, 0);
                        break;
                    }
                    outLevel = 1;
                    goto case 1;

                case 1:     // 1: also show tasks for roles and procedures
                    taskList = await _TaskRepo.ToList(userId, outTaskName, outProcedureId, false, 0, 0);
                    break;
                
                default:    // 0: only show personal tasks
                    taskList = await _TaskRepo.ToList(userId, outTaskName, outProcedureId, true, 0, 0);
                    break;
            }

            var taskInstVM = new TaskInstViewModel() {
                TaskName = outTaskName,
                ProcedureId = outProcedureId,
                isAdmin = isAdmin,
                level = outLevel,
                Tasks = taskList,
                MyProcedures = await _ProcRepo.GetSelectList(userId)
            };
            return View(taskInstVM);
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/Create/id
        /// Shows a form for creating a new task from the
        /// specified task template.
        /// </summary>
        /// <param name="id">id of the procedure the new task belongs to</param>
        /// <param name="TemplateId">id of the task template (optional)</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> Create(int? id, int? TemplateId)
        {
            if (id == null) 
                return NotFound();

            var procedure = await _ProcRepo.Find(id.Value);
            if (procedure == null)
                return NotFound();
            
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            if (!isAdmin && procedure.RespId != userId)
                return NotFound();

            CreateTaskInstViewModel createVM = null!;
            if (TemplateId == null) {
                // no template
                createVM = new CreateTaskInstViewModel() {
                    Name = "Neue Aufgabe",
                    TargetDate = DateTime.Now,
                    Status = CompletionStatus.backlog,

                    TaskRespType = TaskRespType.user,
                    RespRoleId = null,
                    RespUserId = userId,

                    TemplateId = null,
                    ProcedureId = id.Value
                };
            }
            else {
                // create from template
                var taskTemplate = await _TaskTemplRepo.Find(TemplateId.Value);
                if (taskTemplate == null)
                    return NotFound();

                var newTask = taskTemplate.CreateTaskInstance(procedure);
                createVM = new CreateTaskInstViewModel() {
                    Name = newTask.Name,
                    TargetDate = newTask.TargetDate,
                    Status = newTask.Status,

                    TaskRespType = newTask.Resp.TaskRespType,
                    RespRoleId = newTask.Resp.RoleId,
                    RespUserId = newTask.Resp.UserId,

                    TemplateId = newTask.TemplateId,
                    ProcedureId = newTask.ProcedureId
                };
            }
            createVM.AllRoles = await _RoleRepo.GetSelectList();
            createVM.AllUsers = await _UserRepo.GetSelectList();
            createVM.AllTaskTemplates = await _TaskTemplRepo.GetSelectList();
            return View(createVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/Create/id
        /// Accepts data from the form, creates the task,
        /// and writes it to the database.
        /// </summary>
        /// <param name="id">id of the procedure the new task belongs to</param>
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
            [Bind("Name,TargetDate,Status,TaskRespType,RespRoleId,RespUserId,TemplateId")]
            CreateTaskInstViewModel createVM)
        {
            if (ModelState.IsValid)
            {
                var newTask = new TaskInst() {
                    Name = createVM.Name,
                    TargetDate = createVM.TargetDate,
                    Status = createVM.Status,
                    ProcedureId = id,
                    TemplateId = createVM.TemplateId,
                    Resp = new TaskResponsible(
                        createVM.TaskRespType,
                        createVM.RespRoleId,
                        null,
                        null,
                        createVM.RespUserId)
                };
                await _TaskRepo.Add(newTask);
                
                // Weiterleitung zur Übersicht
                return RedirectToAction(nameof(Edit), "Procedure", new {id = id});
            }
            
            // Formular nochmal anzeigen (mit Fehlermeldung)
            createVM.ProcedureId = id;
            createVM.AllRoles = await _RoleRepo.GetSelectList();
            createVM.AllUsers = await _UserRepo.GetSelectList();
            return View(createVM);
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/Edit/id
        /// Shows a form for editing the specified task.
        /// <param name="id">id of the task to edit.</param>
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
            
            var ThisUser = await _UserRepo.LoadCurrentUser(HttpContext);
            if (ThisUser == null)
                return RedirectToAction("Index", "Authentication");

            var element = await _TaskRepo.Find(id.Value);
            if (element == null)
                return NotFound();

            int PermissionLevel = element.getPermissionLevel(ThisUser);
            if (PermissionLevel < 1)
                return NotFound();

            var editVM = new EditTaskInstViewModel() {
                Name = element.Name,
                TargetDate = element.TargetDate,
                Status = element.Status,

                TaskRespType = element.Resp.TaskRespType,
                RespRoleId = element.Resp.RoleId,
                RespUserId = element.Resp.UserId,

                PermissionLevel = PermissionLevel,
                UserId = ThisUser.Id,
                TaskInstId = element.Id,
                Notes = element.Notes,
                Instruction = (element.Template != null) ? element.Template.Instruction : "",
                AllRoles = await _RoleRepo.GetSelectList(),
                AllUsers = await _UserRepo.GetSelectList(),
            };
            return View(editVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/Edit/id
        /// Accepts data from the form, changes the task,
        /// and writes it to the database.
        /// </summary>
        /// <param name="id">id of the task to edit.</param>
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
            [Bind("Name,TargetDate,Status,TaskRespType,RespRoleId,RespUserId")]
            EditTaskInstViewModel modifiedElementVM)
        {
            var ThisUser = await _UserRepo.LoadCurrentUser(HttpContext);
            if (ThisUser == null)
                return RedirectToAction("Index", "Authentication");

            var element = await _TaskRepo.Find(id);
            if (element == null)
                return NotFound();

            int PermissionLevel = element.getPermissionLevel(ThisUser);
            if (PermissionLevel < 1)
                return NotFound();

            if (ModelState.IsValid)
            {
                element.Status = modifiedElementVM.Status;
                element.Resp.Update(
                    modifiedElementVM.TaskRespType,
                    modifiedElementVM.RespRoleId,
                    element.Procedure.ReferId,
                    element.Procedure.RespId,
                    modifiedElementVM.RespUserId
                );
                if (PermissionLevel >= 2) {
                    element.TargetDate = modifiedElementVM.TargetDate;
                    
                    if (PermissionLevel >= 3) {
                        element.Name = modifiedElementVM.Name;
                    }
                }
                await _TaskRepo.Update(element);

                // Weiterleitung zur Übersicht
                return RedirectToAction(nameof(Index));
            }
            
            // Formular nochmal anzeigen (mit Fehlermeldung)
            modifiedElementVM.PermissionLevel = PermissionLevel;
            modifiedElementVM.UserId = ThisUser.Id;
            modifiedElementVM.TaskInstId = element.Id;
            modifiedElementVM.Notes = element.Notes;
            modifiedElementVM.Instruction = (element.Template != null) ? element.Template.Instruction : "";
            modifiedElementVM.AllRoles = await _RoleRepo.GetSelectList();
            modifiedElementVM.AllUsers = await _UserRepo.GetSelectList();
            return View(modifiedElementVM);
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/Delete/id
        /// Shows a form for deleting the specified task.
        /// <param name="id">id of the task to delete.</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();
            
            var element = await _TaskRepo.Find(id.Value);
            if (element == null)
                return NotFound();

            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            if (!isAdmin && element.Procedure.RespId != userId)
                return NotFound();

            var deleteVM = new DeleteTaskInstViewModel() {
                Name = element.Name,
                TargetDate = element.TargetDate,
                Status = element.Status,
                RespStr = element.Resp.DisplayName()
            };
            return View(deleteVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/Delete/id
        /// Deletes the specified task from the database.
        /// </summary>
        /// <param name="id">id of the task to delete.</param>
        /// <returns>
        /// redirects to overview.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _TaskRepo.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/CreateNote/id
        /// Shows a form for creating a note for
        /// the specified task.
        /// </summary>
        /// <param name="id">id of the task the new note belongs to</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> CreateNote(int? id)
        {
            if (id == null)
                return NotFound();

            if (!await _TaskRepo.Exists(id.Value))
                return NotFound();

            var createVM = new NoteViewModel() {
                Title = "Neue Notiz",
                Text = "",
                VisibleToOthers = true
            };
            return View(createVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/CreateNote/id
        /// Accepts data from the form, creates the note,
        /// and writes it to the database.
        /// </summary>
        /// <param name="id">id of the task the new note belongs to</param>
        /// <param name="CreatedElementVM">view model containing the data from the form</param>
        /// <returns>
        /// On success, redirects to overview; on error returns the view for the form again.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNote(int id,
            [Bind("Title,Text,VisibleToOthers")]
            NoteViewModel CreatedElementVM)
        {
            int userId = int.Parse(HttpContext.User.Identity!.Name!);

            if (ModelState.IsValid)
            {
                var newNote = new Note() {
                    Title = CreatedElementVM.Title,
                    Text = CreatedElementVM.Text,
                    VisibleToOthers = CreatedElementVM.VisibleToOthers,
                    CreationDate = DateTime.Now,
                    TaskInstId = id,
                    AuthorId = userId
                };
                await _TaskRepo.AddNote(newNote);
                
                // Redirect to TaskInst/Edit/id
                return RedirectToAction(nameof(Edit), nameof(TaskInst), new {id = newNote.TaskInstId});
            }
            
            // Formular nochmal anzeigen (mit Fehlermeldung)
            return View(CreatedElementVM);
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/EditNote/id
        /// Shows a form for editing the specified note.
        /// <param name="id">id of the note to edit.</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> EditNote(int? id)
        {
            if (id == null)
                return NotFound();

            var note = await _TaskRepo.FindNote(id.Value);
            if (note == null)
                return NotFound();
            
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            if (!note.VisibleToOthers && note.AuthorId != userId)
                return NotFound();

            var editVM = new NoteViewModel() {
                Title = note.Title,
                Text = note.Text,
                VisibleToOthers = note.VisibleToOthers
            };
            return View(editVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/EditNote/id
        /// Accepts data from the form, changes the note,
        /// and writes it to the database.
        /// </summary>
        /// <param name="id">id of the note to edit.</param>
        /// <param name="NoteViewModel">view model containing the data from the form</param>
        /// <returns>
        /// On success, redirects to overview; on error returns the view for the form again.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditNote(int id,
            [Bind("Title,Text,VisibleToOthers")]
            NoteViewModel editVM)
        {
            var note = await _TaskRepo.FindNote(id);
            if (note == null)
                return NotFound();
            
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            if (!note.VisibleToOthers && note.AuthorId != userId)
                return NotFound();

            if (ModelState.IsValid)
            {
                note.Title = editVM.Title;
                note.Text = editVM.Text;
                note.VisibleToOthers = editVM.VisibleToOthers;
                await _TaskRepo.UpdateNote(note);
                
                // Redirect to TaskInst/Edit/id
                return RedirectToAction(nameof(Edit), nameof(TaskInst), new {id = note.TaskInstId});
            }
            
            // Formular nochmal anzeigen (mit Fehlermeldung)
            return View(editVM);
        }

        /// <summary>
        /// HTTP-GET request: /TaskInst/DeleteNote/id
        /// Shows a form for deleting the specified note.
        /// <param name="id">id of the note to delete.</param>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        public async Task<IActionResult> DeleteNote(int? id)
        {
            if (id == null)
                return NotFound();

            var note = await _TaskRepo.FindNote(id.Value);
            if (note == null)
                return NotFound();

            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            if (!note.VisibleToOthers && note.AuthorId != userId)
                return NotFound();

            var deleteVM = new NoteViewModel() {
                Title = note.Title,
                Text = note.Text,
                VisibleToOthers = note.VisibleToOthers
            };
            return View(deleteVM);
        }

        /// <summary>
        /// HTTP-POST request: /TaskInst/DeleteNote/id
        /// Deletes the specified note from the database.
        /// </summary>
        /// <param name="id">id of the note to delete.</param>
        /// <returns>
        /// redirects to overview.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        [HttpPost, ActionName("DeleteNote")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteNoteConfirmed(int id)
        {
            var note = await _TaskRepo.FindNote(id);
            if (note == null)
                return NotFound();
            
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            if (!note.VisibleToOthers && note.AuthorId != userId)
                return NotFound();

            await _TaskRepo.RemoveNote(note);
            return RedirectToAction(nameof(Edit), nameof(TaskInst), new {id = note.TaskInstId});
        }
    }
}