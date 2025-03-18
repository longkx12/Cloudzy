using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class AdminUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
