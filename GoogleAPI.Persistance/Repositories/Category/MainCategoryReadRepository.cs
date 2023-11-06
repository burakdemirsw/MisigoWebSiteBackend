using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GoogleAPI.Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GooleAPI.Application.IRepositories
{
    public class MainCategoryReadRepository : ReadRepository<MainCategory>, IMainCategoryReadRepository
    {
        public MainCategoryReadRepository(GooleAPIDbContext context) : base(context) { }
    }
}
