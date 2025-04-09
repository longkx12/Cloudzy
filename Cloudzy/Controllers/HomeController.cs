using System.Diagnostics;
using System.Linq;
using Cloudzy.Data;
using Cloudzy.Models;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Product;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Implementations;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cloudzy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
