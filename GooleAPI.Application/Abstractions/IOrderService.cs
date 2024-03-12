using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel;
using GoogleAPI.Domain.Models.NEBIM.Product;
using System.Drawing;

namespace GooleAPI.Application.Abstractions
{
    public interface IOrderService
    {
        Task<List<SaleOrderModel>> GetSaleOrders(int type, int invoiceStatus);
        Task<List<SaleOrderModel>> GetSaleOrdersWithMissingItems( );

        Task<List<SaleOrderModel>> GetOrdersByFilter(OrderFilterModel model);
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
        Task<List<CustomerModel>> GetCustomerList(int customerType);
        Task<List<CustomerList_VM>> GetCustomerList_2(GetCustomerList_CM request);
        Task<List<CustomerAddress_VM>> GetCustomerAddress(GetCustomerAddress_CM request);
        Task<CreateCustomer_ResponseModel> SendCustomerToNebim(CreateCustomer_CM order);
        Task<bool> CreateOrder(GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel.NebimOrder Orders);
        Task<bool> CreateClientOrder(ClientOrder request);
        Task<bool> CreateClientOrderBasketItem(ClientOrderBasketItem request);
        Task<ClientOrder_DTO> GetClientOrder(Guid id);
        Task<bool> UpdateClientOrderBasketItem(Guid orderId, Guid lineId, int quantity, decimal price);
        Task<bool> UpdateClientOrderPayment(Guid orderId, string paymetnDescription);
        Task<bool> EditClientCustomer(ClientCustomer request);
        Task<List<ClientCustomer>> GetClientCustomer( );
        Task<bool> AddCustomerAddress(AddCustomerAddress_CM request);
    }

}
