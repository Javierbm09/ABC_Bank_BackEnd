using Microsoft.EntityFrameworkCore;

namespace ABC_Bank.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }


        public DbSet<Contacto> Contactos { get; set; }
    }
}
