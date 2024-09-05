using CDO.Data;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class UserController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public UserController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var postgrescontext = _postgresContext.Users.Include(b => b.RolecdNavigation).Include(x => x.GroupcdNavigation);
            return View(await postgrescontext.ToListAsync());
        }
        //public async Task<IActionResult> Details(int? cd)
        //{
        //    if (cd == null || _postgresContext.Users == null)
        //    {
        //        return NotFound();
        //    }

        //    var user = await _postgresContext.Users
        //        .Include(b => b.RolecdNavigation).Include(x => x.GroupcdNavigation)
        //        .FirstOrDefaultAsync(m => m.Usercd == cd);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(user);
        //}
        public async Task<IActionResult> Create()
        {
            ViewData["Groupcd"] = new SelectList(_postgresContext.Groups, "Groupcd", "Number");
            ViewData["Rolecd"] = new SelectList(_postgresContext.Roles, "Rolecd", "Rolename");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Fio,Login,Password,Email,Teacher,Groupcd,Rolecd")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Usercd = _postgresContext.Users.Max(x => x.Usercd) + 1;
                _postgresContext.Add(user);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Groupcd"] = new SelectList(_postgresContext.Groups, "Groupcd", "Number", user.Groupcd);
            ViewData["Rolecd"] = new SelectList(_postgresContext.Roles, "Rolecd", "Rolename",user.Rolecd);
            return View(user);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Users == null)
            {
                return NotFound();
            }

            var user = await _postgresContext.Users.FindAsync(cd);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Groupcd"] = new SelectList(_postgresContext.Groups, "Groupcd", "Number");
            ViewData["Rolecd"] = new SelectList(_postgresContext.Roles, "Rolecd", "Rolename");
            return View(user);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Usercd,Fio,Login,Password,Email,Teacher,Groupcd,Rolecd")] User user)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(user);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Usercd))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }
        private bool UserExists(int cd)
        {
            return (_postgresContext.Users?.Any(e => e.Usercd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Users == null)
            {
                return NotFound();
            }

            var user = await _postgresContext.Users.Include(x => x.GroupcdNavigation).Include(y => y.RolecdNavigation)
                .FirstOrDefaultAsync(m => m.Usercd == cd);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Usercd)
        {
            if (_postgresContext.Users == null)
            {
                return Problem("Entity set 'DatabaseContext.Users'  is null.");
            }
            var user = await _postgresContext.Users.FindAsync(Usercd);
            if (user != null)
            {
                _postgresContext.Users.Remove(user);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
