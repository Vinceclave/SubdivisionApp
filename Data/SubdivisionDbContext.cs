using Microsoft.EntityFrameworkCore;
using SubdivisionApp.Models;

namespace SubdivisionApp.Data
{
    public class SubdivisionDbContext : DbContext
    {
        public SubdivisionDbContext(DbContextOptions<SubdivisionDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
    }
}
