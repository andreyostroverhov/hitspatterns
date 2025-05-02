using Microsoft.AspNetCore.Mvc;

namespace Monitoring.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}