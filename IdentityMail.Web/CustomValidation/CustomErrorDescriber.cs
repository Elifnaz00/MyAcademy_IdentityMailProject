using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;

namespace IdentityMail.Web.CustomValidation
{
    public class CustomErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError PasswordRequiresDigit()
        {

            return new IdentityError
            {
                Code = "PasswordRequiresDigit",
                Description = "Şifre en bir rakam içermelidir."
            };



        }


        public override IdentityError PasswordRequiresUpper()
        {

            return new IdentityError
            {
                Code = "PasswordRequiresUpper",
                Description = "Şifre en bir büyük harf içermelidir."
            };



        }


        public override IdentityError PasswordRequiresLower()
        {

            return new IdentityError
            {
                Code = "PasswordRequiresLower",
                Description = "Şifre en bir küçük harf içermelidir."
            };



        }


        public override IdentityError PasswordRequiresNonAlphanumeric()
        {

            return new IdentityError
            {
                Code = "PasswordRequresNonAlphanumeric",
                Description = "Şifre en bir özel karakter içermelidir."
            };



        }


        public override IdentityError PasswordTooShort(int length)
        {

            return new IdentityError
            {
                Code = "PasswordTooShort",
                Description = $"Şifre en az {length} karakterden oluşmalıdır."
            };



        }

        public override IdentityError DuplicateUserName(string userName)
        {
            return new IdentityError
            {
                Code = "DuplicateUserName",
                Description = "Kullanıcı adı daha önce alınmış."
            };
        }


        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "DuplicateEmail",
                Description = "Email daha önce alınmış."
            };
        }
    }
}
