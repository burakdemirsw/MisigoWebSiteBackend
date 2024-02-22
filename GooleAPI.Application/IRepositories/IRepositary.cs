using GoogleAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace GooleAPI.Application.IRepositories
{
    public interface IRepositary<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }
    }
}
