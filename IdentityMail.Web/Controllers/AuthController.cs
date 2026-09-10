using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityMail.Web.Controllers
{
    public class AuthController(UserManager<AppUser> _userManager , SignInManager<AppUser> _signInManager) : Controller
    {


        [HttpGet]
        public async Task<IActionResult> Register()
        { 

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerDto) 
        {
            if(registerDto.Password != registerDto.ConfirmPassword)
            {
                ModelState.AddModelError(string.Empty, "Şifreler birbiriyle uyumlu değil");
                return View(registerDto);
            }
            var user = new AppUser
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserName = registerDto.UserName,
            };

            var result= await _userManager.CreateAsync(user,registerDto.Password);
            if (!result.Succeeded) {
                foreach (var item in result.Errors) {
                    ModelState.AddModelError(item.Code, item.Description);
                    return View(registerDto);
                }
            }
            await _userManager.AddToRoleAsync(user, "User");

            return RedirectToAction("Login","Auth");
        }



        [HttpGet]
        public IActionResult Login() { 
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user is null)
            {
                ModelState.AddModelError(string.Empty, "Bu Email sistemde kayıtlı değil.");
                return View(loginDto);
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
            if (!result.Succeeded) {
                ModelState.AddModelError(string.Empty, "Email veya Şifre hatalı");
            }

            if(User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }
            return RedirectToAction("Index","Message");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Auth");
        }


        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
           
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if(user is null)
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
                return View(changePasswordDto);
            }

            if(!ModelState.IsValid)
            {
                return View(changePasswordDto);
            }

            IdentityResult result= await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(changePasswordDto);
            }

            TempData["SuccessMessage"] = "Şifre Değiştirme İşleminiz Başarıyla Gerçekleştirildi!.";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }

    }
}
