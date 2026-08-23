namespace IdentityMail.Web.Areas.Admin.Models
{
    public class UserRoleListViewModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string FullName { get; set; }
        public string Email { get; set; }

        public string Image { get; set; }

        public bool IsActive { get; set; }

        public IList<string> Roles { get; set; }
    }
}
