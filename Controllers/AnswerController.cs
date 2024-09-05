using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class AnswerController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public AnswerController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index(string name = "admin", int pg = 1)
        {
            var user = HttpContext.GetCurrentUser();
            var postgrescontext = _postgresContext.Answers.Include(a => a.QuestioncdNavigation).ThenInclude(b => b.TestcdNavigation);
            const int Pagesize = 10;
            if (pg < 1)
                pg = 1;
            int rescount = postgrescontext.Count();
            var Paginator = new Paginator(rescount,pg,Pagesize);
            int recSkip = (pg - 1) * Pagesize;
            var data = postgrescontext.Skip(recSkip).Take(Paginator.PageSize).ToList();
            this.ViewBag.Paginator = Paginator;
            return View(data);
            //return View(await postgrescontext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Answers == null)
            {
                return NotFound();
            }

            var answer = await _postgresContext.Answers
                .Include(a => a.QuestioncdNavigation).ThenInclude(b => b.TestcdNavigation)
                .FirstOrDefaultAsync(m => m.Answercd == cd);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }
        public IActionResult Create()
        {
            ViewData["Questioncd"] = new SelectList(_postgresContext.Questions, "Questioncd", "Questionnm");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Answernm,Isright,Questioncd")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                answer.Answercd = _postgresContext.Answers.Max(x => x.Answercd) + 1;
                _postgresContext.Add(answer);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Questioncd"] = new SelectList(_postgresContext.Questions, "Questioncd", "Questionnm", answer.Questioncd);
            return View(answer);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Answers == null)
            {
                return NotFound();
            }

            var answer = await _postgresContext.Answers.FindAsync(cd);
            if (answer == null)
            {
                return NotFound();
            }
            ViewData["Questioncd"] = new SelectList(_postgresContext.Questions, "Questioncd", "Questionnm");
            return View(answer);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Answercd,Answernm,Isright,Questioncd")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(answer);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.Answercd))
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
            return View(answer);
        }
        private bool AnswerExists(int cd)
        {
            return (_postgresContext.Answers?.Any(e => e.Answercd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Answers == null)
            {
                return NotFound();
            }

            var answer = await _postgresContext.Answers.Include(a => a.QuestioncdNavigation).ThenInclude(b => b.TestcdNavigation)
                .FirstOrDefaultAsync(m => m.Answercd == cd);
            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Answercd)
        {
            if (_postgresContext.Answers == null)
            {
                return Problem("Entity set 'DatabaseContext.Answers'  is null.");
            }
            var answer = await _postgresContext.Answers.FindAsync(Answercd);
            if (answer != null)
            {
                _postgresContext.Answers.Remove(answer);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
