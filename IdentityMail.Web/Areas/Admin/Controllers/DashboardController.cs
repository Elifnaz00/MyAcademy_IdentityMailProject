using IdentityMail.Web.Areas.Admin.Models;
using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace IdentityMail.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _appDbContext;

        public DashboardController(UserManager<AppUser> userManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsersCount = await _userManager.Users.CountAsync();
            var activeUsersCount = await _userManager.Users.Where(x => x.IsActive == true).CountAsync();
            var unReadMessagesCount = await _appDbContext.UserMessages.Where(x => x.IsRead == false).CountAsync();
            var trackCount = await _appDbContext.UserMessages.Where(x => x.IsDeleted == true).CountAsync();

          
            var groupCategoryList = await _appDbContext.Categories.GroupBy(x => x.Name).Select(g => new CategoryListViewModel
            {
                Name = g.Key,
                TotalCount = g.Count()
            }).ToListAsync();

            var groupMessageList = await _appDbContext.UserMessages.Include(a => a.Sender).GroupBy(x => x.Sender.UserName).Select(g => new UserListViewModel
            {
                Name = g.Key,
                IsActive = g.Select(x => x.Sender.IsActive).First().ToString(),
                TotalCount = g.Count()

            }).OrderBy(x => x.TotalCount).Take(5).ToListAsync();

            var dashboardVm= new DashboardViewModel
            {
                ActiveUserCount = activeUsersCount,
                TotalUserCount = totalUsersCount,
                TrackCount = trackCount,
                UnReadMessageCount = unReadMessagesCount,
                Categories = groupCategoryList,
                UserMessages = groupMessageList

            };
            return View(dashboardVm);
        }
    }
}
