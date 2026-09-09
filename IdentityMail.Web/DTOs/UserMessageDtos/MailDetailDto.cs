using IdentityMail.Web.Entities;

namespace IdentityMail.Web.DTOs.UserMessageDtos
{
    public class MailDetailDto
    {
        public UserMessage UserMessage { get; set; }
        public List<UserMessage>? Messages { get; set; }
    }
}
