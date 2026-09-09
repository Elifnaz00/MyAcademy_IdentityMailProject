namespace IdentityMail.Web.Entities
{
    public class UserMessage
    {
        public int? ReplyToMessageId { get; set; }
        public UserMessage ReplyToMessage { get; set; }

        public int Id { get; set; }
        public string Subject { get; set; }

        public string Body { get; set; }
       
        public DateTime SendDate { get; set; }
        public bool IsRead { get; set; }

        public bool IsImportant { get; set; }

        public bool IsComplaint { get; set; }

        public bool IsDraft { get; set; }
        public bool IsDeleted { get; set; }

        public AppUser Sender { get; set; }
        public int SenderId { get; set; }   

        public AppUser Receiver { get; set; }
        public int ReceiverId { get; set; }

        public Category Category { get; set; }
        public int? CategoryId { get; set; }
    }
}
