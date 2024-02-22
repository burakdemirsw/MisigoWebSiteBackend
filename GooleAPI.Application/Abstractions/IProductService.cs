using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Product;

namespace GooleAPI.Application.Abstractions
{
    public interface IProductService
    {
        public Task<string> GenerateBarcode_A(List<BarcodeModel_A> barcodes);
        Task<List<BarcodeModel>> GetBarcodeDetail(string qrCode);
    }
}
