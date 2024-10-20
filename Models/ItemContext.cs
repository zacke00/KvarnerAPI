#nullable disable
using Microsoft.EntityFrameworkCore;

namespace KvarnerAPI.Models
{
    public class ItemContext : DbContext
    {
        public ItemContext(DbContextOptions<ItemContext> options) : base(options)
        {
        }

        public DbSet<Items> Items { get; set; }
    }
}