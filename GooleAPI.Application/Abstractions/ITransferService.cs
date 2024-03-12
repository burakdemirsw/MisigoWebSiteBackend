using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;

namespace GoogleAPI.Persistance.Concreates
{
    public interface ITransferService
    {
        Task<List<TransferModel>> GetProductOfTrasfer(string orderNumber);
        Task<int> FastTransfer(FastTransferModel model);
        Task<List<FastTransferModel>> GetAllFastTransferModels( );
        Task<List<FastTransferModel>> GetFastTransferModelsByOperationId(string operationId);
        Task<FastTransferModel> DeleteProductFromFastTransfer(DeleteProductOfCount deleteModel);
        Task<List<InventoryItemModel>> GetInventoryItem(string type);
        Task<List<CountConfirmData>> GetInventoryFromOrderNumber(String OrderNo);
        Task<List<AvailableShelf>> GetAvailableShelves( );
        Task<List<OfficeModel>> GetOfficeModel( );
        Task<List<WarehouseOfficeModel>> GetWarehouseModel(string officeCode);
        Task<int> DeleteWarehouseTransferByOrderNumber(string id);
        Task<List<WarehosueTransferListModel>> GetWarehosueTransferList(WarehouseTransferListFilterModel model);
        Task<List<WarehosueOperationListModel>> GetWarehosueOperationListByFilter(WarehouseOperationListFilterModel model);
        Task<List<ProductOfOrderModel>> GetWarehosueOperationDetail(string innerNumber);
        Task<bool> TransferProducts(string orderNo);
        Task<int> SendNebımToTransferProduct(WarehouseOperationProductModel model);
        Task<bool> ConfirmOperation(List<string> InnerNumberList);
        Task<List<TransferRequestListModel>> GetTransferRequestListModel(string type);
        Task<List<BarcodeModel>> GetOperationWarehousue(string innerNumber);
        Task<List<WarehosueOperationListModel>> GetWarehosueOperationList(string status);
        Task<WarehosueOperationListModel> GetWarehosueOperationListByInnerNumber(string innerNumber);
    }
}
