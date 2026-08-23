namespace IdentityMail.Web.DTOs.AdminDtos
{
    public class UserListDto
    {
        public string UserName { get; set; }    
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? ProfileImageUrl { get; set; }
    }
}
