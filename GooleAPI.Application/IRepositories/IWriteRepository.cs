using GoogleAPI.Domain.Entities.Common;

namespace GooleAPI.Application.IRepositories
{
    public interface IWriteRepository<T> : IRepositary<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> datas);

        bool Remove(T model);

        bool RemoveRange(List<T> datas);

        Task<bool> RemoveAsync(int id);

        Task<bool> Update(T model);

        Task<int> SaveAsync(T model);
    }
}
