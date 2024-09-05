using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class ThemeController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public ThemeController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Student")
            {
                var postgrescontext = _postgresContext.Groups.Include(a => a.Disciplinecds).ThenInclude(d => d.Themes).ThenInclude(e => e.DisciplinecdNavigation)
                    .Where(b => b.Groupcd == user.Groupcd)
                    .SelectMany(e => e.Disciplinecds.SelectMany(f => f.Themes));
                return View(await postgrescontext.ToListAsync());
            }
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var postgrescontext = _postgresContext.Themes.Include(x => x.DisciplinecdNavigation).ThenInclude(y => y.UsercdNavigation)
                    .Where(z => z.DisciplinecdNavigation.UsercdNavigation.Usercd == user.Usercd);
                return View(await postgrescontext.ToListAsync());
            }
            else
            {
                var postgrescontext = _postgresContext.Themes.Include(x => x.DisciplinecdNavigation);
                return View(await postgrescontext.ToListAsync());
            }
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Themes == null)
            {
                return NotFound();
            }

            var theme = await _postgresContext.Themes
                .Include(b => b.LearningMaterials).Include(x => x.DisciplinecdNavigation).ThenInclude(y => y.UsercdNavigation).Include(a => a.Tests)
                .FirstOrDefaultAsync(m => m.Themecd == cd);
            if (theme == null)
            {
                return NotFound();
            }

            return View(theme);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Themename,Themevolume,Disciplinecd")] Theme theme)
        {
            if (ModelState.IsValid)
            {
                theme.Themecd = _postgresContext.Themes.Max(x => x.Themecd) + 1;
                _postgresContext.Add(theme);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename", theme.Disciplinecd);
            return View(theme);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Themes == null)
            {
                return NotFound();
            }

            var theme = await _postgresContext.Themes.FindAsync(cd);
            if (theme == null)
            {
                return NotFound();
            }
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename");
            return View(theme);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Themecd,Themename,Themevolume,Disciplinecd")] Theme theme)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(theme);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThemeExists(theme.Themecd))
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
            return View(theme);
        }
        private bool ThemeExists(int cd)
        {
            return (_postgresContext.Themes?.Any(e => e.Themecd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Themes == null)
            {
                return NotFound();
            }

            var theme = await _postgresContext.Themes.Include(x => x.DisciplinecdNavigation)
                .FirstOrDefaultAsync(m => m.Themecd == cd);
            if (theme == null)
            {
                return NotFound();
            }

            return View(theme);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Themecd)
        {
            if (_postgresContext.Themes == null)
            {
                return Problem("Entity set 'DatabaseContext.Theme'  is null.");
            }
            var theme = await _postgresContext.Themes.FindAsync(Themecd);
            if (theme != null)
            {
                _postgresContext.Themes.Remove(theme);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
