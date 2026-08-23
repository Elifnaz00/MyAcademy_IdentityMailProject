using IdentityMail.Web.Areas.Admin.Models;
using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System.Data;

namespace IdentityMail.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly AppDbContext _appDbContext;

        public UserController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, AppDbContext appDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _appDbContext = appDbContext;
        }

        public async Task<ActionResult> Index(int page = 1, int pageSize = 6)
        {
            var userList = await _userManager.Users.ToListAsync();
            var userListVm= new List<UserRoleListViewModel>();

            foreach (var user in userList) {
                var roles = await _userManager.GetRolesAsync(user);

                userListVm.Add(new UserRoleListViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FullName = user.FirstName + " " + user.LastName,
                    IsActive = user.IsActive,
                    Image = user.ProfileImageUrl,
                    Roles = roles,

                });
            }

            PagedList<UserRoleListViewModel> userRoleListModel = new PagedList<UserRoleListViewModel>(userListVm.AsQueryable(), page, pageSize);
            return View(userRoleListModel);
        }



        [HttpGet]
        public async Task<ActionResult> RoleAssign()
        {
            var roleList = await _roleManager.Roles.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Name

            }).ToListAsync();


            ViewBag.RoleList = roleList;



            var userList = await _userManager.Users.Select(x => new SelectListItem
            {
                Text = x.FirstName + " " + x.LastName,
                Value = x.UserName
            }).ToListAsync();

            ViewBag.UserList = userList;

            return View();
        }



        [HttpPost]
        public async Task<ActionResult> RoleAssignAsync(RoleAssignViewModel roleAssignViewModel)
        {
            var user = await _userManager.FindByNameAsync(roleAssignViewModel.UserName);
            if (user is null)
                return NotFound();


            
            if(!await _roleManager.RoleExistsAsync(roleAssignViewModel.RoleName))
            {
                await _roleManager.CreateAsync(new AppRole
                {
                    Name = roleAssignViewModel.RoleName,
                });
            }

            await _userManager.AddToRoleAsync(user, roleAssignViewModel.RoleName);
            return RedirectToAction(nameof(RoleAssign));
        }



        [HttpPost]
        public async Task<ActionResult> ToggleActive(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user is null)
                return NotFound();

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);
           
           
            return RedirectToAction(nameof(Index));
        }
    }
}


       

