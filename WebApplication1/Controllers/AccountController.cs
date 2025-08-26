using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using WebApplication1.Data;
using WebApplication1.Entities;
using WebApplication1.Models.User;

namespace WebApplication1.Controllers
{

    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        public AccountController(AppDbContext db) => _db = db;

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
        [AllowAnonymous]
        public IActionResult AccessDenied(string? returnUrl)
        {
            Response.StatusCode = 403; // for logging/SEO
            ViewBag.ReturnUrl = returnUrl;
            return View();             // render nice page
        }

        // មាន Denied ចាស់? redirect មក AccessDenied
        [AllowAnonymous]
        public IActionResult Denied(string? returnUrl)
            => RedirectToAction(nameof(AccessDenied), new { returnUrl });

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
        {
            // find user by email
            var user = await _db.Set<User>()
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            // ✅ Verify password
            // If you stored hashed: BCrypt.Verify(password, user.Password)
            // If currently plain text (not recommended): password == user.Password
            var ok = false;
            try { ok = BCrypt.Net.BCrypt.Verify(password, user.Password); }
            catch { ok = user.Password == password; } // fallback if not hashed yet

            if (!ok)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }

            // Build claims
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name ?? user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role ?? "User")
        };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }  

        public IActionResult Denied() => Content("Access denied");


        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(Uservm model)
        {
            if (ModelState.IsValid)
            {


            }
            _db.users.Add(new User
            {
                Name = model.Name,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.role,
                RegisteredDate = DateTime.UtcNow
            });
             _db.SaveChanges();
            return RedirectToAction("Index", "Teams");
        }
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
