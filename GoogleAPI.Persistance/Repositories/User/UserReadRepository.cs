
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories.UserAndCommunication;

namespace GoogleAPI.Persistance.Repositories.UserAndCommunication
{
    public class UserReadRepository : ReadRepository<User>, IUserReadRepository
    {
        public UserReadRepository(GooleAPIDbContext context) : base(context) { }
    }
}
