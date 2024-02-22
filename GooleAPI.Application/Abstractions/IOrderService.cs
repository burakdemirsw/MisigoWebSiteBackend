using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using System.Drawing;

namespace GooleAPI.Application.Abstractions
{
    public interface IOrderService
    {
        Task<List<SaleOrderModel>> GetSaleOrders( int type);
        Task<List<SaleOrderModel>> GetSaleOrdersWithMissingItems( );

        Task<List<SaleOrderModel>> GetOrdersByFilter(OrderFilterModel model);
        Task<List<CustomerModel>> GetCustomerList(int customerType);
        Task<List<SaleOrderModel>> GetPurchaseOrders( );
        Task<List<SaleOrderModel>> GetPurchaseOrdersByFilter(OrderFilterModel model);
        Task<List<SaleOrderModel>> GetSaleOrdersById(string id);
        Task<List<ProductOfOrderModel>> GetOrderSaleDetailByPackageId(string PackageId);
        Task<List<ProductOfOrderModel>> GetPurchaseOrderSaleDetail(string orderNumber);
        Task<List<ProductOfOrderModel>> GetProductsOfOrders(int numberOfList);
        Task<List<ProductOfOrderModel>> GetOrderSaleDetail(string orderNumber);
        Task<Bitmap> GetOrderDetailsFromQrCode(string data);
        Task<List<CollectedProduct>> GetCollectedOrderProducts(string orderNumber);
        Task<bool> SetInventoryByOrderNumber(string orderNumber);
        Task<BarcodeAddModel> AddSaleBarcode(BarcodeAddModel model);
        Task<int> SetStatusOfPackages(List<ProductOfOrderModel> models);
        Task<List<ReadyToShipmentPackageModel>> GetReadyToShipmentPackages( );
        Task<int> PutReadyToShipmentPackagesStatusById(string id);
        Task<List<ProductOfOrderModel>> GetMissingProductsOfOrder(string orderNumber);


    }
}
