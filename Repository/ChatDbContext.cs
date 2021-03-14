using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options)
            : base(options)
        {
        }
        public DbSet<Message> Messages { get; set; }

    }
}
