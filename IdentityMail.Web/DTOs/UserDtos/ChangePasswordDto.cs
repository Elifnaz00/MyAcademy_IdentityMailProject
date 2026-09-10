using System.ComponentModel.DataAnnotations;

namespace IdentityMail.Web.DTOs.UserDtos
{       
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Mevcut şifre alanı boş geçilemez.")]
        public string? CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre alanı boş geçilemez.")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifreyi onaylama alanı boş geçilemez.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Yeni şifreler birbiriyle uyumlu değil.")]
        public string? ConfirmNewPassword { get; set; }
    }
}
