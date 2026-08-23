namespace IdentityMail.Web.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<UserMessage> UserMessages { get; set; } 
    }
}
