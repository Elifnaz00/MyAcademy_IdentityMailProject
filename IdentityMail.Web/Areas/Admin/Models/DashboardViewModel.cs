using IdentityMail.Web.Entities;

namespace IdentityMail.Web.Areas.Admin.Models
{
    public class DashboardViewModel
    {
        public int TotalUserCount { get; set; } 
        public int ActiveUserCount { get; set; }

        public int UnReadMessageCount { get; set; }

        public int TrackCount { get; set; }

        public List<CategoryListViewModel> Categories { get; set; }

        public List<UserListViewModel> UserMessages { get; set; } 
    }
}
