using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ComplaintController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public ComplaintController(UserManager<AppUser> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();

            var compaintMessages= await _appDbContext.UserMessages.AsNoTracking().Include(a=>a.Sender).Include(b=>b.Receiver).Where(x=> x.IsComplaint).ToListAsync();
            return View(compaintMessages);
        }

        public async Task<IActionResult> ComplaintDetail(int id)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();

            var compaintMessages = await _appDbContext.UserMessages.AsNoTracking().Include(a => a.Sender).Include(b => b.Receiver).FirstOrDefaultAsync(x=>x.Id == id);
            return View(compaintMessages);
        }
    }
}
