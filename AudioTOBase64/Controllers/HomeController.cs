using AudioTOBase64.Models;
using AudioTOBase64.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AudioTOBase64.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<IActionResult> resultPage(Class model)
        {
            if (model.File == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                Audio audioRepository = new Audio(_configuration);
                string response = await audioRepository.PostAudio(model);
                ViewBag.EmployeeID = model.EmployeeID;
                ViewBag.Email = model.Email;
                ViewBag.Time = DateTime.Now;
                ViewBag.Base64String = response;
                string checkstring = response.Substring(0, 3);
                TempData["checkstring"] = checkstring;
                TempData["result"] = response;
                ViewBag.result = response;
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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