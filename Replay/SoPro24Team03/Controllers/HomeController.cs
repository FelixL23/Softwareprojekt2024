using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;


using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;

namespace SoPro24Team03.Controllers;

[Authorize]
public class HomeController : Controller
{
    private IProcedureRepository _prodRepo;
    private ITaskInstRepository _taskRepo;

    public HomeController(IProcedureRepository prodRepo, ITaskInstRepository taskRepo)
    {
        _prodRepo = prodRepo;
        _taskRepo = taskRepo;
    }

    //made Daniel H, Felix und 
    public async Task<ViewResult> Index()
    {
        // Get the user ID from the current HttpContext
        int userId = int.Parse(HttpContext.User.Identity!.Name!);
        // Check if the current user is an admin
        bool isAdmin = HttpContext.User.IsInRole("Admin"); 
        // Retrieve the filter string from cookies   
        string filterString = Request.Cookies["FilterTaskInst"] ?? "";

        DashboardViewModel dVM = new DashboardViewModel()
        {
             // Fetch the list of procedures for the current user
            Procedures =  await _prodRepo.ToList(userId, null, 5),
            // Fetch the list of task instances for the current user
            TaskInsts = await _taskRepo.ToList(userId, null, 0, false, 10),
        };
        
        return View(dVM);
    }

    public IActionResult Error()
    {
        return View("Error!");
    }
}
