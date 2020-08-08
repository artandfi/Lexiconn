using Microsoft.AspNetCore.Mvc;

namespace Lexiconn.Controllers
{
    public class HelpController : Controller
    {
        public IActionResult Index() => View();
    }
}