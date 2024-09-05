using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class QuestionController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public QuestionController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index(string name = "admin", int pg = 1)
        {
            var user = HttpContext.GetCurrentUser();
            var postgrescontext = _postgresContext.Questions.Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).
                Include(c => c.TestcdNavigation).ThenInclude(d => d.ThemecdNavigation);
            const int Pagesize = 10;
            if (pg < 1)
                pg = 1;
            int rescount = postgrescontext.Count();
            var Paginator = new Paginator(rescount, pg, Pagesize);
            int recSkip = (pg - 1) * Pagesize;
            var data = postgrescontext.Skip(recSkip).Take(Paginator.PageSize).ToList();
            this.ViewBag.Paginator = Paginator;
            return View(data);
            //return View(await postgrescontext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Questions == null)
            {
                return NotFound();
            }

            var learnmaterial = await _postgresContext.Questions.Include(x => x.Answers)
                .Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).
                Include(c => c.TestcdNavigation).ThenInclude(d => d.ThemecdNavigation)
                .FirstOrDefaultAsync(m => m.Questioncd == cd);
            if (learnmaterial == null)
            {
                return NotFound();
            }

            return View(learnmaterial);
        }
        public IActionResult Create()
        {
            ViewData["Testcd"] = new SelectList(_postgresContext.Tests, "Testcd", "Testname");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Questionnm,Testcd")] Question question)
        {
            if (ModelState.IsValid)
            {
                question.Questioncd = _postgresContext.Questions.Max(x => x.Questioncd) + 1;
                _postgresContext.Add(question);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Testcd"] = new SelectList(_postgresContext.Tests, "Testcd", "Testname", question.Testcd);
            return View(question);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Questions == null)
            {
                return NotFound();
            }

            var question = await _postgresContext.Questions.FindAsync(cd);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["Testcd"] = new SelectList(_postgresContext.Tests, "Testcd", "Testname", question.Testcd);
            return View(question);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Questioncd,Questionnm,Testcd")] Question question)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(question);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Questioncd))
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
            return View(question);
        }
        private bool QuestionExists(int cd)
        {
            return (_postgresContext.Questions?.Any(e => e.Questioncd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Questions == null)
            {
                return NotFound();
            }

            var question = await _postgresContext.Questions.Include(a => a.TestcdNavigation).ThenInclude(b => b.DisciplinecdNavigation).
                Include(c => c.TestcdNavigation).ThenInclude(d => d.ThemecdNavigation)
                .FirstOrDefaultAsync(m => m.Questioncd == cd);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Questioncd)
        {
            if (_postgresContext.Questions == null)
            {
                return Problem("Entity set 'DatabaseContext.Questions'  is null.");
            }
            var question = await _postgresContext.Questions.FindAsync(Questioncd);
            if (question != null)
            {
                _postgresContext.Questions.Remove(question);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
