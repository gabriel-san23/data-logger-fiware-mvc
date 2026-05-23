using Microsoft.AspNetCore.Mvc;

namespace DataLogger.Controllers
{
    public class UsuarioController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
