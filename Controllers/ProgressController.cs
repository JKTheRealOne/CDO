using CDO.Data;
using CDO.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class ProgressController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public ProgressController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index(string searchstring)
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Student") 
            {
                var data = _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation)
                .Where(u => u.Usercd == user.Usercd);
                return View(await data.ToListAsync());
            }
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var data = _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Where(c => c.TestcdNavigation.DisciplinecdNavigation.Usercd == user.Usercd)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation);
                if (!String.IsNullOrEmpty(searchstring))
                {
                    data = _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Where(f => f.UsercdNavigation.Fio.Contains(searchstring))
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation);
                }
                return View(await data.ToListAsync());
            }
            else {
            var postgrescontext = _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation);
            if(!String.IsNullOrEmpty(searchstring))
            {
                    postgrescontext = _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Where(f => f.UsercdNavigation.Fio.Contains(searchstring))
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation);
            }
                return View(await postgrescontext.ToListAsync());
            }
            //return View(await postgrescontext.ToListAsync());
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Progresses == null)
            {
                return NotFound();
            }

            var progress = await _postgresContext.Progresses.Include(x => x.UsercdNavigation).ThenInclude(y => y.GroupcdNavigation)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).Include(a => a.TestcdNavigation).ThenInclude(b => b.ThemecdNavigation)
                .FirstOrDefaultAsync(m => m.Progresscd == cd);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Progresscd)
        {
            if (_postgresContext.Progresses == null)
            {
                return Problem("Entity set 'DatabaseContext.Tests'  is null.");
            }
            var progress = await _postgresContext.Progresses.FindAsync(Progresscd);
            if (progress != null)
            {
                _postgresContext.Progresses.Remove(progress);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Progresses == null)
            {
                return NotFound();
            }

            var progress = await _postgresContext.Progresses
                .Include(b => b.TestcdNavigation).ThenInclude(a => a.ThemecdNavigation).Include(w => w.UsercdNavigation).Include(x => x.UserAnswers).ThenInclude(y => y.AnswercdNavigation).ThenInclude(z => z.QuestioncdNavigation)
                .FirstOrDefaultAsync(m => m.Progresscd == cd);
            if (progress == null)
            {
                return NotFound();
            }

            return View(progress);
        }
    }
}
