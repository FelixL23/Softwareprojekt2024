using Microsoft.AspNetCore.Mvc;
using SoPro24Team03.Data;
using SoPro24Team03.ViewModels;
using SoPro24Team03.Models; 
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace SoPro24Team03.Controllers
{
    [Authorize]
    public class ArchivController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IProcedureTemplateRepository _procedureTemplateRepository;
        private readonly ITaskTemplateRepository _taskTemplateRepository;

        // Konstruktor zur Initialisierung der benötigten Repositorys
        public ArchivController(IUserRepository userRepository, IProcedureRepository procedureRepository, IProcedureTemplateRepository procedureTemplateRepository, ITaskTemplateRepository taskTemplateRepository)
        {
            _userRepository = userRepository;
            _procedureRepository = procedureRepository;
            _procedureTemplateRepository = procedureTemplateRepository;
            _taskTemplateRepository = taskTemplateRepository;
        }

        public IActionResult Index()
        {

            var viewModel = new ArchivedViewModel
            {
                isAdmin = HttpContext.User.IsInRole("Admin"),
            };

            return View(viewModel);
        }
        
        public IActionResult ArchivedTask()
        {
            return View();
        }

        public async Task<IActionResult> ArchivedUser()
        {
            var archivedUsers = await _userRepository.GetArchivedUsersAsync();
            var viewModel = archivedUsers.Select(u => new ArchivedUserViewModel
            {
                Id = u.Id,
                Name = $"{u.FirstName} {u.LastName}",
                Email = u.EmailAddress,
            }).ToList();

            return View(viewModel);
        }

        /// <summary>
        /// Filters the Archived Procedures a Users sees in the Archive
        /// if a user is Admin he sees everything, else he sees only Procedures where he is Resp
        /// </summary>
        /// <returns>
        /// A view displaying the list of archived procedures
        /// </returns>
        /// <remarks>
        /// Made by Felix
        /// </remarks>
        public async Task<IActionResult> ArchivedProcedure()
        {
            bool isAdmin = HttpContext.User.IsInRole("Admin");
            int userId = int.Parse(HttpContext.User.Identity!.Name!);
            var archivedProcedures = await _procedureRepository.GetArchivedProcedureAsync();
            List<ArchivedProcedureViewModel> viewModel;
            if(isAdmin)
            {
            viewModel = archivedProcedures.Select(p => new ArchivedProcedureViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Resp = p.Resp,
                RespId = p.RespId,
                TargetDate = p.TargetDate,
                TaskInsts = p.TaskInsts
            }).ToList();
            }else {
                var filteredArchivedProcedures = archivedProcedures.Where(p => p.RespId == userId);
                viewModel = filteredArchivedProcedures.Select(p => new ArchivedProcedureViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Resp = p.Resp,
                RespId = p.RespId,
                TargetDate = p.TargetDate,
                TaskInsts = p.TaskInsts
            }).ToList();
            }

            return View(viewModel);
        }

        /// <summary>
        /// Retrieves the task list for an archived procedure by its ID.
        /// </summary>
        /// <param name="Id">Id of the Procedure.</param>
        /// <returns>
        /// ViewModels to the View
        /// </returns>
        /// <remarks>
        /// Made by Felix
        /// </remarks>
        public async Task<IActionResult> ArchivedTaskList(int id)
        {
            var procedure = await _procedureRepository.Find(id);
            if (procedure == null || !procedure.IsArchived)
            {
                return NotFound();
            }

            var viewModel = new ArchivedProcedureViewModel
            {
                Id = procedure.Id,
                Name = procedure.Name,
                Resp = procedure.Resp,
                RespId = procedure.RespId,
                TargetDate = procedure.TargetDate,
                TaskInsts = procedure.TaskInsts
            };

            return View(viewModel);
        }

       
    }
}

