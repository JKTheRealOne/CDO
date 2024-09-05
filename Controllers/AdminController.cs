using CDO.Data;
using Microsoft.AspNetCore.Mvc;

namespace CDO.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {

            return View();
        }
    }
}
