using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Task_Manager.Data;
using Task_Manager.Models;
using static Task_Manager.Data.MVCDataContext;

namespace Task_Manager.Controllers
{
    public class UserController : Controller
    {
        private readonly MVCDataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(MVCDataContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(UserRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var newUser = new User
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        public IActionResult Login()
        {
            bool isAuthenticated = HttpContext.Session.GetString("UserId") != null;
            ViewBag.IsAuthenticated = isAuthenticated;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);

                if (user != null)
                {
                    HttpContext.Session.SetString("LoggedInUserId", user.Id.ToString());
                    ViewBag.IsAuthenticated = true;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            ViewBag.IsAuthenticated = false;
            return View(model);
        }

        private void SetSession(User user)
        {
            HttpContext.Session.SetString("UserId", user.Id.ToString());
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            ViewBag.IsAuthenticated = false;
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public IActionResult CheckSession()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Ok();
        }
    }
}
