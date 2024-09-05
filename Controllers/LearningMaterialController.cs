using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class LearningMaterialController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public LearningMaterialController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index(string name = "admin")
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Student")
            {
                var postgrescontext = _postgresContext.Groups.Include(a => a.Disciplinecds)
                    .ThenInclude(b => b.Themes).ThenInclude(c => c.LearningMaterials).ThenInclude(d => d.ThemecdNavigation).ThenInclude(c => c.DisciplinecdNavigation)
                    .Where(b => b.Groupcd == user.Groupcd).SelectMany(c => c.Disciplinecds.SelectMany(d => d.Themes).SelectMany(m => m.LearningMaterials));
                return View(await postgrescontext.ToListAsync());
            }
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var postgrescontext = _postgresContext.LearningMaterials.Include(x => x.ThemecdNavigation).ThenInclude(y => y.DisciplinecdNavigation)
                    .Where(z => z.ThemecdNavigation.DisciplinecdNavigation.Usercd == user.Usercd);
                return View(await postgrescontext.ToListAsync());
            }
            else
            {
                var postgrescontext = _postgresContext.LearningMaterials.Include(x => x.ThemecdNavigation).ThenInclude(y => y.DisciplinecdNavigation);
                return View(await postgrescontext.ToListAsync());
            }
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.LearningMaterials == null)
            {
                return NotFound();
            }

            var learnmaterial = await _postgresContext.LearningMaterials
                .Include(x => x.ThemecdNavigation).ThenInclude(y => y.DisciplinecdNavigation)
                .FirstOrDefaultAsync(m => m.LearningMaterialcd == cd);
            if (learnmaterial == null)
            {
                return NotFound();
            }

            return View(learnmaterial);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename");
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Materialname,Materialcontent,Materialvolume,Themecd")] LearningMaterial lm)
        {
            if (ModelState.IsValid)
            {
                lm.LearningMaterialcd = _postgresContext.LearningMaterials.Max(x => x.LearningMaterialcd) + 1;
                _postgresContext.Add(lm);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename", lm.Themecd);
            return View(lm);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.LearningMaterials == null)
            {
                return NotFound();
            }

            var lm = await _postgresContext.LearningMaterials.FindAsync(cd);
            if (lm == null)
            {
                return NotFound();
            }
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename");
            return View(lm);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Materialname,Materialcontent,Materialvolume,Themecd")] LearningMaterial lm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(lm);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LearningMaterialExists(lm.LearningMaterialcd))
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
            return View(lm);
        }
        private bool LearningMaterialExists(int cd)
        {
            return (_postgresContext.LearningMaterials?.Any(e => e.LearningMaterialcd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.LearningMaterials == null)
            {
                return NotFound();
            }

            var lm = await _postgresContext.LearningMaterials.Include(x => x.ThemecdNavigation).ThenInclude(y => y.DisciplinecdNavigation)
                .FirstOrDefaultAsync(m => m.LearningMaterialcd == cd);
            if (lm == null)
            {
                return NotFound();
            }

            return View(lm);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int LearningMaterialcd)
        {
            if (_postgresContext.LearningMaterials == null)
            {
                return Problem("Entity set 'DatabaseContext.LearningMaterials'  is null.");
            }
            var lm = await _postgresContext.LearningMaterials.FindAsync(LearningMaterialcd);
            if (lm != null)
            {
                _postgresContext.LearningMaterials.Remove(lm);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
