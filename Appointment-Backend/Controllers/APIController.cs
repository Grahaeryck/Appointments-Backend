using Microsoft.AspNetCore.Mvc;

namespace Appointment_Backend.Controllers
{
    public class APIController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
