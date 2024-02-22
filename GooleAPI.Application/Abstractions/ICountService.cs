using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;

namespace GoogleAPI.Persistance.Concreates
{
    public interface ICountService
    {
        Task<List<CountListModel>> GetCountList( );
        Task<List<CountListModel>> GetCountListByFilter(CountListFilterModel model);
        Task<ProductCountModel> CountTransferProductPuschase(CreatePurchaseInvoice model);
        Task<ProductCountModel> CountProduct(CreatePurchaseInvoice model);
        Task<ProductCountModel> CountTransferProduct(WarehouseFormModel model);
        Task<List<ProductCountModel>> GetShelvesOfProduct(string barcode);
        Task<List<ProductCountModel3>> GetShelvesOfProduct2(string barcode);
        Task<ProductCountModel> CountProduct3(CountProductRequestModel2 model);
        Task<ProductCountModel> CountProduct4(CountProductRequestModel3 model);

        Task<ProductCountModel> CountProductControl(CountProductRequestModel2 model);
        Task<List<ProductCountModel2>> CountProductByBarcode2(string barcode);
        Task<ProductCountModel> CountTransfer(WarehouseFormModel model);

        Task<bool> CompleteCount(string orderNumber, bool isShelfBased, bool isShelfBased2);
        Task<List<CountedProduct>> GetProductOfCount(string orderNumber);

        Task<int> DeleteProductOfCount(DeleteProductOfCount model);

        Task<int> DeleteCountById(string id);




    }
}
