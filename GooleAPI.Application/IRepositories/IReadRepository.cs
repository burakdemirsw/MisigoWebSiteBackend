﻿using GoogleAPI.Domain.Entities.Common;
using System.Linq.Expressions;

namespace GooleAPI.Application.IRepositories
{
    public interface IReadRepository<T> : IRepositary<T> where T : BaseEntity
    {
        IQueryable<T> GetAll(bool tracking = true);

        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);

        Task<T> GetSingleAsync(Expression<Func<T, bool>> method, bool tracking = true); //first ro def async kullancak

        Task<T> GetByIdAsync(int id, bool tracking = true); //first or def async kullancak
    }
}
