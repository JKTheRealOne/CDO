using CDO.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CDO.Controllers
{
    public class RoleController : Controller
    {
        private readonly PostgresContext _postgresContext;

        public RoleController(PostgresContext postgresContext)
        {
            _postgresContext = postgresContext;
        }
        public async Task<IActionResult> Index()
        {
            var postgrescontext = _postgresContext.Roles;
            return View(await postgrescontext.ToListAsync());
        }
    }
}
