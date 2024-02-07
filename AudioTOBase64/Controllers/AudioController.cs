using AudioTOBase64.Models;
using AudioTOBase64.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Controllers
{
    public class AudioController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<AudioController> _logger;
        public AudioController(IConfiguration configuration, ILogger<AudioController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult audioRecording()
        {
            Audio audioRepository = new Audio(_configuration);
            string[] promptValue = new string[3];
            promptValue = audioRepository.GetRandomPromptsFromDatabase();
            ViewBag.PromptValue = promptValue;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> audioRecording(Class model)
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
                TempData["EmployeeID"] = model.EmployeeID;
                TempData["Email"] = model.Email;
                //_logger.LogInformation(model.Email,model.EmployeeID);
                return View();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


       

    }
}