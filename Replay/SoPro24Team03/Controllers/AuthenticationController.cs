using System;
using System.Web;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

using SoPro24Team03.Data;
using SoPro24Team03.Models;
using SoPro24Team03.ViewModels;


namespace SoPro24Team03.Controllers
{
    /// <summary>
    /// Controller handling logging in and logging out.
    /// </summary>
    public class AuthenticationController : Controller
    {
        /// <value>repository for accessing users.</value>
        private IUserRepository _userRepo;

        /// <value>service for managing sessions.</value>
        private ISessionService _sessionService;

        public AuthenticationController(IUserRepository userRepo, ISessionService sessionService)
        {
            _userRepo = userRepo;
            _sessionService = sessionService;
        }

        /// <summary>
        /// HTTP-GET request: /Authentication
        /// Shows the login form.
        /// </summary>
        /// <returns>
        /// View for this request.
        /// </returns>
        /// <remarks>
        /// Made by Daniel Albert
        /// </remarks>
        [HttpGet]
        public ViewResult Index()
        {
            return View(new AuthenticationViewModel());
        }

        /// <summary>
        /// HTTP-POST request: /Authentication/Login
        /// Authenticates a user based on username and password.
        /// </summary>
        /// <param name="vm">view model containing user credentials from the form.</param>
        /// <returns>
        /// Redirect to dashboard, if login was successfull.
        /// </returns>
        /// <remarks>
        /// Made by Daniel H, Celina, Fabio, Daniel A
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Bind("userName,inputPassword")]
            AuthenticationViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            var ThisUser = await _userRepo.Find(vm.userName);
            if (ThisUser == null || !ThisUser.ValidatePassword(vm.inputPassword))
            {
                ModelState.AddModelError("", "Falscher Benutzername oder Passwort.");
                return View("Index", vm);
            }

            if (ThisUser.isArchived || ThisUser.isSuspended)
            {
                ModelState.AddModelError("", "Der Login für diesen Benutzer ist deaktiviert.");
                return View("Index", vm);
            }

            var sessionId = $"{ThisUser.Id}-{Guid.NewGuid()}";
            await _sessionService.CreateSession(sessionId);

            // create cookie
            var claims = new List<Claim>() {
                new Claim(ClaimTypes.Name, ThisUser.Id.ToString()),
                new Claim(ClaimTypes.Role, ThisUser.Roles.Any(r => r.IsAdmin) ? "Admin" : "NoAdmin"),
                new Claim("SessionId", sessionId)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme
            );

            var authProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(200)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            HttpContext.Response.Cookies.Append("SessionId", sessionId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(200)
            });

            // redirect to Dashboard
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// HTTP-POST request: /Authentication/Logout
        /// Destroys Auth Cookie and redirects to login page
        /// </summary>
        /// <returns>
        /// Redirects to login page
        /// </returns>
        /// <remarks>
        /// Made by Daniel H, Fabio
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (HttpContext.Request.Cookies.TryGetValue("SessionId", out var sessionId))
            {
                await _sessionService.InvalidateSession(sessionId);
            }

            HttpContext.Response.Cookies.Delete("SessionId");

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );
            return RedirectToAction("Index");
        }
    }
}