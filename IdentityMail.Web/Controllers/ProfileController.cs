using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.ProfileDtos;
using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace IdentityMail.Web.Controllers
{
    public class ProfileController(UserManager<AppUser> _userManager, AppDbContext _appDbContext ) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfileImageUrl = user.ProfileImageUrl,
                UserName = user.UserName
            };

            return View(userDto);
        }


        [HttpGet]
        public async Task<IActionResult> EditProfile(int id)
        {
            var profile= await _userManager.FindByIdAsync(id.ToString());
            if (profile is null)
                return NotFound();

            var profileDto= new UpdateProfileDto
            {
                Id = profile.Id,
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                ProfileImageUrl = profile.ProfileImageUrl,
                Email = profile.Email,
                UserName= profile.UserName
            };
            return View(profileDto);

        }


        [HttpPost]
        public async Task<IActionResult> EditProfile(UpdateProfileDto updateProfileDto)
        {
            
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return NotFound();

     
            var getEmail= await _userManager.GetEmailAsync(user);
            if (updateProfileDto.Email != getEmail)
            {
                var emailExist = await _userManager.FindByEmailAsync(updateProfileDto.Email);
                if (emailExist is not null)
                {
                    ModelState.AddModelError("Email", "Bu mail adresi başka bir kullanıcı tarafından alınmış.");
                    return View(updateProfileDto);
                }
                var setEmailResullt = await _userManager.SetEmailAsync(user, updateProfileDto.Email);
                if (!setEmailResullt.Succeeded)
                {
                    ModelState.AddModelError("Email", "Mail adresi güncellenirken bir hata oluştu.");
                    return View(updateProfileDto);
                }

               
            }


            var getUserName = await _userManager.GetUserNameAsync(user);
            if (updateProfileDto.UserName != getUserName)
            {
                var userNameExist = await _userManager.FindByNameAsync(updateProfileDto.UserName);
                if (userNameExist is not null)
                {
                    ModelState.AddModelError("UserName", "Bu kullanıcı adı başka bir kullanıcı tarafından alınmış.");
                    return View(updateProfileDto);
                }
                var setUserNameResult = await _userManager.SetUserNameAsync(user, updateProfileDto.UserName);
                if (!setUserNameResult.Succeeded)
                {
                    ModelState.AddModelError("UserName", "Kullanıcı adı güncellenirken bir hata oluştu.");
                    return View(updateProfileDto);
                }

            }

             user.FirstName = updateProfileDto.FirstName;
           
             user.LastName = updateProfileDto.LastName;
          
             user.ProfileImageUrl = updateProfileDto.ProfileImageUrl;

            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));

        }
    }
}
