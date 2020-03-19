using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    public class StatsController : Controller
    {
        /// <summary>
        /// Returns a view with stats to display.
        /// </summary>
        public IActionResult Index()
        {
            return View();
        }
    }
}