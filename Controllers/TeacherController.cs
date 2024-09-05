using Microsoft.AspNetCore.Mvc;

namespace CDO.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
