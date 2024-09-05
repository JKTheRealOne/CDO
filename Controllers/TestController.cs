using CDO.Data;
using CDO.Helpers;
using CDO.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class TestController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public TestController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var user = HttpContext.GetCurrentUser();
            if (user.RolecdNavigation.Rolename == "Student")
            {
                var postgrescontext = _postgresContext.Groups.Include(a => a.Disciplinecds)
                    .ThenInclude(b => b.Tests).ThenInclude(d => d.ThemecdNavigation).ThenInclude(c => c.DisciplinecdNavigation).ThenInclude(z => z.UsercdNavigation)
                    .Where(b => b.Groupcd == user.Groupcd).SelectMany(c => c.Disciplinecds.SelectMany(d => d.Tests));
                return View(await postgrescontext.ToListAsync());
            }
            if (user.RolecdNavigation.Rolename == "Teacher")
            {
                var postgrescontext = _postgresContext.Tests.Include(x => x.DisciplinecdNavigation).Include(y => y.ThemecdNavigation).Include(z => z.UsercdNavigation)
                    .Where(a => a.UsercdNavigation.Usercd == user.Usercd);
                return View(await postgrescontext.ToListAsync());
            }
            else
            {
                var postgrescontext = _postgresContext.Tests.Include(x => x.DisciplinecdNavigation).Include(y => y.ThemecdNavigation).Include(z => z.UsercdNavigation);
                return View(await postgrescontext.ToListAsync());
            }
        }
        public async Task<IActionResult> Details(int? cd)
        {
            if (cd == null || _postgresContext.Tests == null)
            {
                return NotFound();
            }

            var test = await _postgresContext.Tests.Include(x => x.DisciplinecdNavigation).Include(y => y.ThemecdNavigation).Include(z => z.UsercdNavigation)
                .Include(a => a.Progresses).ThenInclude(b => b.UsercdNavigation).Include(d => d.Questions)
                .FirstOrDefaultAsync(m => m.Testcd == cd);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }
        public async Task<IActionResult> Create()
        {
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename");
            ViewData["Usercd"] = new SelectList(_postgresContext.Users.Where(x => x.RolecdNavigation.Rolename == "Teacher"), "Usercd", "Fio");
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Testname,Testnumquest,Testduration,Disciplinecd,Themecd,Usercd")] Test test)
        {
            if (ModelState.IsValid)
            {
                test.Testcd = _postgresContext.Tests.Max(x => x.Testcd) + 1;
                _postgresContext.Add(test);
                await _postgresContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename", test.Disciplinecd);
            ViewData["Usercd"] = new SelectList(_postgresContext.Users.Where(x => x.RolecdNavigation.Rolename == "Teacher"), "Usercd", "Fio",test.Usercd);
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename",test.Themecd);
            return View(test);
        }
        public async Task<IActionResult> Edit(int? cd)
        {
            if (cd == null || _postgresContext.Tests == null)
            {
                return NotFound();
            }

            var test = await _postgresContext.Tests.FindAsync(cd);
            if (test == null)
            {
                return NotFound();
            }
            ViewData["Disciplinecd"] = new SelectList(_postgresContext.Disciplines, "Disciplinecd", "Disciplinename");
            ViewData["Usercd"] = new SelectList(_postgresContext.Users.Where(x => x.RolecdNavigation.Rolename == "Teacher"), "Usercd", "Fio");
            ViewData["Themecd"] = new SelectList(_postgresContext.Themes, "Themecd", "Themename");
            return View(test);
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Testcd,Testname,Testnumquest,Testduration,Disciplinecd,Themecd,Usercd")] Test test)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _postgresContext.Update(test);
                    await _postgresContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TestExists(test.Testcd))
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
            return View(test);
        }
        private bool TestExists(int cd)
        {
            return (_postgresContext.Tests?.Any(e => e.Testcd == cd)).GetValueOrDefault();
        }
        public async Task<IActionResult> Delete(int? cd)
        {
            if (cd == null || _postgresContext.Tests == null)
            {
                return NotFound();
            }

            var test = await _postgresContext.Tests.Include(x => x.DisciplinecdNavigation).Include(y => y.ThemecdNavigation).Include(z => z.UsercdNavigation)
                .FirstOrDefaultAsync(m => m.Themecd == cd);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int Testcd)
        {
            if (_postgresContext.Tests == null)
            {
                return Problem("Entity set 'DatabaseContext.Tests'  is null.");
            }
            var test = await _postgresContext.Tests.FindAsync(Testcd);
            if (test != null)
            {
                _postgresContext.Tests.Remove(test);
            }

            await _postgresContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> PassTest(int Testcd)
        {
            var user = HttpContext.GetCurrentUser();
            var result = _postgresContext.Progresses.Include(z => z.UserAnswers).ThenInclude(y => y.QuestioncdNavigation).ThenInclude(x => x.Answers)
                .FirstOrDefault(x => x.Usercd == user.Usercd && x.Testcd == Testcd && x.Progressduration == null);
            if (result != null)
            {
                ViewBag.Testname = _postgresContext.Tests.Where(x => x.Testcd == Testcd).Select(x => x.Testname).ToList().FirstOrDefault();
                var questions = result.UserAnswers.Select(a => a.QuestioncdNavigation).Distinct();
                return View(questions);
            }
            else
            {
                var test = await _postgresContext.Tests.FindAsync(Testcd);
                ViewBag.Testname = _postgresContext.Tests.Where(x => x.Testcd == Testcd).Select(x => x.Testname).ToList().FirstOrDefault();
                var questions = _postgresContext.Questions.Include(y => y.Answers).Where(a => a.Testcd == Testcd)
                    .OrderBy(x => EF.Functions.Random()).Take(test.Testnumquest).ToList();
                var progress = new Progress()
                {
                    Progresscd = _postgresContext.Progresses.Max(x => x.Progresscd) + 1,
                    Usercd = user.Usercd,
                    Testcd = Testcd
                    ,
                    Progressgrade = null,
                    Progressdate = DateTime.Now,
                    Progressduration = null
                };
                _postgresContext.Add(progress);
                await _postgresContext.SaveChangesAsync();
                foreach (var question in questions)
                {
                    //if(question.Answers.Where(x => x.Isright == true).Count() > 1)
                    //{
                    //    foreach(var answer in question.Answers.Where(x => x.Isright == true))
                    //    {
                    //        _postgresContext.Add(new UserAnswer()
                    //        {
                    //            Useranswercd = _postgresContext.UserAnswers.Max(x => x.Useranswercd) + 1,
                    //            Progresscd = progress.Progresscd,
                    //            Questioncd = question.Questioncd,
                    //            Answercd = null
                    //        });
                    //        await _postgresContext.SaveChangesAsync();
                    //        _postgresContext.ChangeTracker.Clear();
                    //    }
                    //}
                    _postgresContext.Add(new UserAnswer() 
                    {Useranswercd = _postgresContext.UserAnswers.Max(x => x.Useranswercd)+1,
                        Progresscd = progress.Progresscd, Questioncd = question.Questioncd,Answercd = null});
                    await _postgresContext.SaveChangesAsync();
                    _postgresContext.ChangeTracker.Clear();
                }
                //_postgresContext.Add(useranswers);
                //await _postgresContext.SaveChangesAsync();
                return View(questions);
            }
        }

        [HttpPost, ActionName("PassTest")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PassTest(int Testcd,List<Question> questions)
        {
            var user = HttpContext.GetCurrentUser();
            var progress = _postgresContext.Progresses.Include(x => x.UserAnswers)
                .FirstOrDefault(x => x.Usercd == user.Usercd && x.Testcd == Testcd && x.Progressduration == null);
            var form = Request.Form;
            var arr = form.Keys;
            //Обновить ответы пользователя в соответствии с выбранными, если вопрос с несколькими ответами добавить новые записи
            foreach (var useranswer in new List<UserAnswer> (progress.UserAnswers))
            {
                foreach(var answer in form[$"{useranswer.Questioncd}"].Skip(1))
                {
                    _postgresContext.Add(new UserAnswer()
                        {
                            Useranswercd = _postgresContext.UserAnswers.Max(x => x.Useranswercd) + 1,
                            Progresscd = progress.Progresscd,
                            Questioncd = useranswer.Questioncd,
                            Answercd = Int32.Parse(answer)
                         });
                    await _postgresContext.SaveChangesAsync();
                    _postgresContext.ChangeTracker.Clear();
                }
                useranswer.Answercd = Int32.Parse(form[$"{useranswer.Questioncd}"].First());
                _postgresContext.Update(useranswer);
                await _postgresContext.SaveChangesAsync();
                _postgresContext.ChangeTracker.Clear();
            }
            int trueanswers = 0;
            int useranswers = 0;
            foreach(var useranswer in progress.UserAnswers)
            {
               useranswers += _postgresContext.Answers.Where(x => x.Answercd == useranswer.Answercd && x.Isright == true).Count();
               trueanswers += _postgresContext.Answers.Where(x => x.Questioncd == useranswer.Questioncd && x.Isright == true).Count();
               
            }
            var percentage = (double)useranswers / (double)trueanswers;
            int grade;
            if (percentage <= 0.4)
            {
                grade = 2;
            }
            else if (percentage <= 0.6)
            {
                grade = 3;
            }
            else if (percentage <= 0.8)
            {
                grade = 4;
            }
            else
            {
                grade = 5;
            }
            progress.Progressduration = DateTime.Now - progress.Progressdate;
            progress.Progressgrade = grade;
            _postgresContext.Update(progress);
            await _postgresContext.SaveChangesAsync();
            return Redirect($"~/{user.RolecdNavigation.Rolename}");
        }
    }
}
