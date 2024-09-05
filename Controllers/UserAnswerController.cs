using CDO.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class UserAnswerController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public UserAnswerController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var postgrescontext = _postgresContext.UserAnswers.Include(x => x.QuestioncdNavigation).Include(y => y.AnswercdNavigation)
                .Include(z => z.ProgresscdNavigation).ThenInclude(n => n.UsercdNavigation);
            return View(await postgrescontext.ToListAsync());
        }

        public async Task<IActionResult> IndexFromTest(int progresscd)
        {
            var postgrescontext = _postgresContext.UserAnswers.Include(x => x.QuestioncdNavigation).Include(y => y.AnswercdNavigation)
                .Include(z => z.ProgresscdNavigation).ThenInclude(n => n.UsercdNavigation).Where(x => x.Progresscd == progresscd);
            return View(await postgrescontext.ToListAsync());
        }

    }
}
