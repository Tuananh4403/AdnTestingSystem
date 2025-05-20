using Microsoft.AspNetCore.Mvc;

namespace AdnTestingSystem.IdentityService.API.Controllers
{
    public class RolesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
