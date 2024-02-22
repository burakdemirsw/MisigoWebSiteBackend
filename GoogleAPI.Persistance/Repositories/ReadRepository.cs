using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GoogleAPI.Persistance.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly GooleAPIDbContext _context;

        public ReadRepository(GooleAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll(bool tracking = true)
        {
            var query = Table.AsQueryable();
            query = query.AsNoTracking();
            return query;
            ;
        }

        public async Task<T> GetByIdAsync(int id, bool tracking = true)
        //=>Table.FirstOrDefaultAsync(x => x.Id == id);
        //=> await Table.FindAsync(id);
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();

            return await Table.FirstOrDefaultAsync(data => data.Id == id);
        }

        public async Task<T> GetSingleAsync(
            System.Linq.Expressions.Expression<Func<T, bool>> method,
            bool tracking = true
        )
        {
            var query = Table.AsQueryable();
            if (!tracking)
                query = Table.AsNoTracking();

            return await Table.FirstOrDefaultAsync(method);
        }

        public IQueryable<T> GetWhere(
            System.Linq.Expressions.Expression<Func<T, bool>> method,
            bool tracking = true
        )
        {
            var query = Table.Where(method);
            query = query.AsNoTracking();
            return query;
            ;
        }
    }
}
