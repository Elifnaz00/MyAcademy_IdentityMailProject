using IdentityMail.Web.Entities;

namespace IdentityMail.Web.DTOs.UserMessageDtos
{
    public class SendMailDto
    {
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

    }
}
