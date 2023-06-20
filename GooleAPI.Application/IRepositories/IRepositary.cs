using GoogleAPI.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.IRepositories
{
    public interface IRepositary<T> where T : BaseEntity
    {
        DbSet<T> Table { get; }
    }
}
