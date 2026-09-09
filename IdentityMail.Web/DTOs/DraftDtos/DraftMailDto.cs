namespace IdentityMail.Web.DTOs.DraftDtos
{
    public class DraftMailDto
    {
        public int Id { get; set; }
        public string ReceiverEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
