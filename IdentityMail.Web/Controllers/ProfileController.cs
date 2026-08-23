using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.ProfileDtos;
using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.Controllers
{
    public class ProfileController(UserManager<AppUser> _userManager, AppDbContext _appDbContext ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userDto = new UserDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfileImageUrl = user.ProfileImageUrl,
            };

            return View(userDto);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var profile= await _userManager.FindByIdAsync(id.ToString());
            var profileDto= new UpdateProfileDto
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ProfileImageUrl = profile.ProfileImageUrl,
            };
            return View(profileDto);

        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateProfileDto updateProfileDto)
        {
            var profile = await _userManager.FindByIdAsync(updateProfileDto.Id.ToString());
            profile.ProfileImageUrl = updateProfileDto.ProfileImageUrl;
            profile.FirstName= updateProfileDto.FirstName;
            profile.LastName= updateProfileDto.LastName;
            
            return RedirectToAction(nameof(Index));

        }
    }
}
