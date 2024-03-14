
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories.UserAndCommunication;

namespace GoogleAPI.Persistance.Repositories.UserAndCommunication
{
    public class CargoBarcodeReadRepository : ReadRepository<CargoBarcode>, ICargoBarcodeReadRepository
    {
        public CargoBarcodeReadRepository(GooleAPIDbContext context) : base(context) { }
    }
}
