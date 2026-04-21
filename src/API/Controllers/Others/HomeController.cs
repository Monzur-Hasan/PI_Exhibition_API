using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Others
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
