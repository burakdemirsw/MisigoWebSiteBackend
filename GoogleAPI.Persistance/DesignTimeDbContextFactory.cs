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
            var sqlCon2 = "Data Source=192.168.2.38;Initial Catalog=MISIGO;User ID=sa;Password=1524;TrustServerCertificate=True;";

            dbContextBuilder.UseSqlServer(sqlCon2);
            return new GooleAPIDbContext(dbContextBuilder.Options);
        }
    }
}
