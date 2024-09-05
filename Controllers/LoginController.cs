using CDO.Data;
using CDO.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CDO.Controllers
{
    public class LoginController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public LoginController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public IActionResult Index()
        {
            return View("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login()
        {
            // получаем из формы email и пароль
            var form = Request.Form;
            string login = form["login"];
            string password = form["password"];

            var users = _postgresContext.Users.Include(w => w.RolecdNavigation);
            // находим пользователя 
            User? user = users.FirstOrDefault(p => p.Login == login && p.Password == password);
            // если пользователь не найден, отправляем статусный код 401
            if (user is null)
            {
                ViewData["Error"] = "Указанный пользователь не существует";
                return View("Index");
            }
            //Results.Unauthorized();
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.RolecdNavigation.Rolename)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(claimsPrincipal);
            return RedirectToAction("Index",$"{user.RolecdNavigation.Rolename}");
        }
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index","Login");
        }
    }
}
