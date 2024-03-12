
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories.UserAndCommunication;

namespace GoogleAPI.Persistance.Repositories.UserAndCommunication
{
    public class UserWriteRepository : WriteRepository<User>, IUserWriteRepository
    {
        public UserWriteRepository(GooleAPIDbContext context) : base(context) { }
    }
}
