using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace AudioTOBase64.Models
{
    public class Class
    {
        //
        [Display(Name = "Employee ID")]
        public string EmployeeID { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Audio File")]
        public IFormFile File { get; set; }
    }
}
