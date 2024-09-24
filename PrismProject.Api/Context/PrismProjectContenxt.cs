using Microsoft.EntityFrameworkCore;

namespace PrismProject.Api.Context
{
    public class PrismProjectContenxt : DbContext
    {
        public PrismProjectContenxt(DbContextOptions<PrismProjectContenxt> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<ToDo> ToDos { get; set; }
        public DbSet<Memo> Memos { get; set; }

    }
}
