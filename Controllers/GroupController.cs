using CDO.Data;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CDO.Helpers;

namespace CDO.Controllers
{
    public class GroupController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public GroupController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index(string name = "admin")
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var postgrescontext = _postgresContext.Disciplines.Include(a => a.Groupcds).Where(b => b.Usercd == user.Usercd).SelectMany(c => c.Groupcds).Distinct();
                return View(await postgrescontext.ToListAsync());
            }
            else
            {
                var postgrescontext = _postgresContext.Groups.Include(w => w.Users);
                return View(await postgrescontext.ToListAsync());
            }
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Groups == null)
            {
                return NotFound();
            }

            var group = await _postgresContext.Groups
                .Include(b => b.Users).Include(w => w.Disciplinecds).ThenInclude(x => x.UsercdNavigation)
                .FirstOrDefaultAsync(m => m.Groupcd == cd);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Number,StartDate,FinishDate")] Group group)
        {
            if (ModelState.IsValid)
            {
                group.Groupcd = _postgresContext.Groups.Max(x => x.Groupcd) + 1;
                _postgresContext.Add(group);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(group);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Groups == null)
            {
                return NotFound();
            }

            var group = await _postgresContext.Groups.FindAsync(cd);
            if (group == null)
            {
                return NotFound();
            }
            return View(group);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Groupcd,Number,StartDate,FinishDate")] Group group)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(group);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Groupcd))
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
            return View(group);
        }
        private bool GroupExists(int cd)
        {
            return (_postgresContext.Groups?.Any(e => e.Groupcd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Groups == null)
            {
                return NotFound();
            }

            var group = await _postgresContext.Groups
                .FirstOrDefaultAsync(m => m.Groupcd == cd);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Groupcd)
        {
            if (_postgresContext.Groups == null)
            {
                return Problem("Entity set 'DatabaseContext.Groups'  is null.");
            }
            var group = await _postgresContext.Groups.FindAsync(Groupcd);
            if (group != null)
            {
                _postgresContext.Groups.Remove(group);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
