using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GoogleAPI.Persistance.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        private readonly GooleAPIDbContext _context;

        public WriteRepository(GooleAPIDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)
        {
            EntityEntry<T> entityEntry = await Table.AddAsync(model);
            Boolean state = entityEntry.State == EntityState.Added;
            await SaveAsync(model);

            return state;
        }

        public async Task<bool> AddRangeAsync(List<T> datas)
        {
            await Table.AddRangeAsync(datas);
            await SaveAsync2(datas);
            return true;
        }

        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry = Table.Remove(model);
            Boolean state = entityEntry.State == EntityState.Deleted;
            SaveAsync(model);

            return state;
        }

        public async Task<bool> RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);
            await SaveAsync2(datas);
            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            T model = await Table.FirstOrDefaultAsync(data => data.Id == id);

            return Remove(model);
        }

        public async Task UpdateRange(List<T> models)
        {
            Table.UpdateRange(models);
            await SaveAsync2(models);
        }

        public async Task<bool> Update(T model)
        {
            EntityEntry<T> entityEntry = Table.Update(model);
            Boolean state = entityEntry.State == EntityState.Modified;
            await SaveAsync(model);
            return state;
        }

        public async Task<int> SaveAsync(T model) => await _context.SaveChangesAsync();

        public async Task<int> SaveAsync2(List<T> models) => await _context.SaveChangesAsync();
    }
}
