using Microsoft.AspNetCore.Mvc;

namespace Appointment_Backend.Controllers
{
    public class Doctor_Scheduler_Controller : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
