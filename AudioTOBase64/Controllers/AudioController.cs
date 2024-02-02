using AudioTOBase64.Models;
using AudioTOBase64.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AudioTOBase64.Controllers
{
    public class AudioController : Controller
    {
       


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult resultPage()
        {
            string checkstring = TempData["checkstring"] as string;
            ViewBag.checkstring = checkstring;
            return View();
        }

        public IActionResult UploadAudioFile()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadAudioFile([FromForm] Models.Class model)
        {
            if (model == null || model.File == null || model.File.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                Audio audioRepository = new Audio();
                string response = await audioRepository.PostAudio(model);
                ViewBag.EmployeeID = model.EmployeeID;
                ViewBag.Email = model.Email;
                ViewBag.Time = DateTime.Now;
                ViewBag.Base64String = response;
                string checkstring = response.Substring(0, 3);
                TempData["checkstring"] = checkstring;
                return RedirectToAction("resultPage");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}