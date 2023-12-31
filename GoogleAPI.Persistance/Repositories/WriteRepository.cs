﻿using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return true;
        }

        public bool Remove(T model)
        {
            EntityEntry<T> entityEntry = Table.Remove(model);
            Boolean state = entityEntry.State == EntityState.Deleted;
            SaveAsync(model);

            return state;
        }

        public bool RemoveRange(List<T> datas)
        {
            Table.RemoveRange(datas);
            return true;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            T model = await Table.FirstOrDefaultAsync(data => data.Id == id);

            return Remove(model);
        }

        public async Task<bool> Update(T model)
        {
            EntityEntry<T> entityEntry = Table.Update(model);
            entityEntry.State = EntityState.Modified;
            await SaveAsync(model);
            return entityEntry.State == EntityState.Modified;
        }

        public async Task<int> SaveAsync(T model) => await _context.SaveChangesAsync();
    }
}
