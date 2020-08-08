using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    public class StatsController : Controller
    {
        public IActionResult Index() => View();
    }
}