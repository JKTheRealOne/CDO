using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CDO.Controllers
{
    public class DisciplineController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public DisciplineController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Student")
            {
                //var group = _postgresContext.Groups.Where(x => x.Number == user.GroupcdNavigation.Number);
                var data = _postgresContext.Groups.Include(a => a.Disciplinecds).ThenInclude(z => z.UsercdNavigation)
                    .Where(b => b.Groupcd == user.Groupcd).SelectMany(c => c.Disciplinecds);
                return View(await data.ToListAsync());
            }
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var data = _postgresContext.Disciplines.Include(x => x.UsercdNavigation).Where(y => y.Usercd == user.Usercd);
                return View(await data.ToListAsync());
            }
            else
            {
                var postgrescontext = _postgresContext.Disciplines.Include(x => x.UsercdNavigation);
                return View(await postgrescontext.ToListAsync());
            }
        }

        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Disciplines == null)
            {
                return NotFound();
            }

            var group = await _postgresContext.Disciplines
                .Include(x => x.UsercdNavigation).Include(a => a.Themes)
                .FirstOrDefaultAsync(m => m.Disciplinecd == cd);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        public async Task<IActionResult> Create()
        {
            ViewData["Usercd"] = new SelectList(_postgresContext.Users.Where(x => x.RolecdNavigation.Rolename == "Admin" | x.RolecdNavigation.Rolename == "Teacher"), "Usercd", "Fio");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Disciplinename,Usercd")] Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                discipline.Disciplinecd = _postgresContext.Disciplines.Max(x => x.Disciplinecd) + 1;
                _postgresContext.Add(discipline);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Usercd"] = new SelectList(_postgresContext.Users, "Usercd", "Fio", discipline.Usercd);
            return View(discipline);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Disciplines == null)
            {
                return NotFound();
            }

            var discipline = await _postgresContext.Disciplines.FindAsync(cd);
            if (discipline == null)
            {
                return NotFound();
            }
            ViewData["Usercd"] = new SelectList(_postgresContext.Users.Where(x => x.RolecdNavigation.Rolename == "Admin" | x.RolecdNavigation.Rolename == "Teacher"), "Usercd", "Fio");
            return View(discipline);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Disciplinecd,Disciplinename,Usercd")] Discipline discipline)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(discipline);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DisciplineExists(discipline.Disciplinecd))
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
            return View(discipline);
        }
        private bool DisciplineExists(int cd)
        {
            return (_postgresContext.Disciplines?.Any(e => e.Disciplinecd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Disciplines == null)
            {
                return NotFound();
            }

            var discipline = await _postgresContext.Disciplines.Include(x => x.UsercdNavigation)
                .FirstOrDefaultAsync(m => m.Disciplinecd == cd);
            if (discipline == null)
            {
                return NotFound();
            }

            return View(discipline);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Disciplinecd)
        {
            if (_postgresContext.Disciplines == null)
            {
                return Problem("Entity set 'DatabaseContext.Disciplines'  is null.");
            }
            var discipline = await _postgresContext.Disciplines.FindAsync(Disciplinecd);
            if (discipline != null)
            {
                _postgresContext.Disciplines.Remove(discipline);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
