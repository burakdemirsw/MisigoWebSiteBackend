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
            dbContextBuilder.UseSqlServer("Data Source=192.168.2.36;Initial Catalog=BDD2017;User ID=sa;Password=8969;Integrated Security=True;");
            return new GooleAPIDbContext(dbContextBuilder.Options);
        }
    }
}
