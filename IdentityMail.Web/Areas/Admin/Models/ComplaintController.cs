using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.Areas.Admin.Models
{
    public class ComplaintController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
