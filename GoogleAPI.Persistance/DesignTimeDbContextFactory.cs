using GoogleAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;


namespace GoogleAPI.Persistance
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<GooleAPIDbContext>
    {
        public GooleAPIDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<GooleAPIDbContext> dbContextBuilder = new();
            dbContextBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=WhatsappAPI;trusted_connection=true");
            return new GooleAPIDbContext(dbContextBuilder.Options);
        }
    }
}
