using Microsoft.AspNetCore.Mvc;

namespace CDO.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
