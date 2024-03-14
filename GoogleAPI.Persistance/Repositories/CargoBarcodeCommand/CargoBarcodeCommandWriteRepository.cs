
using GoogleAPI.Domain.Entities;
using GoogleAPI.Persistance.Contexts;
using GooleAPI.Application.IRepositories.UserAndCommunication;

namespace GoogleAPI.Persistance.Repositories.UserAndCommunication
{
    public class CargoBarcodeWriteRepository : WriteRepository<CargoBarcode>, ICargoBarcodeWriteRepository
    {
        public CargoBarcodeWriteRepository(GooleAPIDbContext context) : base(context) { }
    }
}
