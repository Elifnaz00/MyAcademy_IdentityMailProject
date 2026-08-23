using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Context
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                .HasMany(message=> message.SentMessages)
                .WithOne(x=> x.Sender)
                .HasForeignKey(x=>x.SenderId).OnDelete(DeleteBehavior.Restrict);


            builder.Entity<AppUser>()
               .HasMany(message => message.ReceivedMessages)
               .WithOne(x => x.Receiver)
               .HasForeignKey(x => x.ReceiverId).OnDelete(DeleteBehavior.Restrict);


            base.OnModelCreating(builder);
        }

        public DbSet<UserMessage> UserMessages { get; set; }
        public DbSet<Category> Categories { get; set; }



    }
}
