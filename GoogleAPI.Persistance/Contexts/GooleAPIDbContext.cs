using Google.Apis.Drive.v3.Data;
using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Entities.Common;
using GoogleAPI.Domain.Models.Filter;
using GoogleAPI.Domain.Models.NEBIM;
using GoogleAPI.Domain.Models.NEBIM.Address;
using GoogleAPI.Domain.Models.NEBIM.Category;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Customer.CreateCustomerModel;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Order.CreateOrderModel;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using GoogleAPI.Domain.Models.Payment;
using Microsoft.EntityFrameworkCore;


namespace GoogleAPI.Persistance.Contexts
{
    public class GooleAPIDbContext : DbContext
    {

        public GooleAPIDbContext(DbContextOptions<GooleAPIDbContext> options) : base(options)
        {
            Database.SetCommandTimeout(180); // SQL sorguları için zaman aşımı süresini 3 dakika olarak ayarla
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {



            modelBuilder.Entity<WarehosueOperationListModel>().HasNoKey();
            modelBuilder.Entity<SaleOrderModel>().HasNoKey();
            modelBuilder.Entity<BarcodeModel>().HasNoKey();
            modelBuilder.Entity<OrderSaleDetailModel>().HasNoKey();
            modelBuilder.Entity<OrderBillingModel>().HasNoKey();
            modelBuilder.Entity<ProductOfOrderModel>().HasNoKey();
            modelBuilder.Entity<OfficeModel>().HasNoKey();
            modelBuilder.Entity<WarehouseOfficeModel>().HasNoKey();
            modelBuilder.Entity<WarehosueOperationListModel>().HasNoKey();
            modelBuilder.Entity<WarehouseOperationDetailModel>().HasNoKey();
            modelBuilder.Entity<InvoiceNumberModel>().HasNoKey();
            modelBuilder.Entity<RemainingProductsModel>().HasNoKey();
            modelBuilder.Entity<CountProductRequestModel>().HasNoKey();
            modelBuilder.Entity<ProductCountModel>().HasNoKey();
            modelBuilder.Entity<ProductCountModel2>().HasNoKey();
            modelBuilder.Entity<RecepieModel>().HasNoKey();
            modelBuilder.Entity<CustomerModel>().HasNoKey();
            modelBuilder.Entity<OrderData>().HasNoKey();
            modelBuilder.Entity<CreatePurchaseInvoice>().HasNoKey();
            modelBuilder.Entity<SalesPersonModel>().HasNoKey();
            modelBuilder.Entity<Domain.Models.NEBIM.Order.Invoice>().HasNoKey();
            modelBuilder.Entity<CollectedProduct>().HasNoKey();
            modelBuilder.Entity<CountListModel>().HasNoKey();
            modelBuilder.Entity<CountedProduct>().HasNoKey();
            modelBuilder.Entity<CreatePurchaseInvoice>().HasNoKey();
            modelBuilder.Entity<TransferModel>().HasNoKey();
            modelBuilder.Entity<TransferData>().HasNoKey();
            modelBuilder.Entity<CountConfirmData>().HasNoKey();
            modelBuilder.Entity<WarehouseTransferListFilterModel>().HasNoKey();
            modelBuilder.Entity<WarehosueTransferListModel>().HasNoKey();
            modelBuilder.Entity<InvoiceErrorInfoModel>().HasNoKey();
            modelBuilder.Entity<ProductList_VM>().HasNoKey();
            modelBuilder.Entity<QrControlModel>().HasNoKey();
            modelBuilder.Entity<InventoryItemModel>().HasNoKey();
            modelBuilder.Entity<TransferRequestListModel>().HasNoKey();
            modelBuilder.Entity<InventoryResponseModel>().HasNoKey();
            modelBuilder.Entity<AvailableShelf>().HasNoKey();
            modelBuilder.Entity<ProductCountModel3>().HasNoKey();
            modelBuilder.Entity<QrOperationResponse>().HasNoKey();
            modelBuilder.Entity<DestroyItem_Response>().HasNoKey();
            modelBuilder.Entity<InvoiceOfCustomer_VM>().HasNoKey();
            modelBuilder.Entity<CustomerList_VM>().HasNoKey();
            modelBuilder.Entity<CustomerAddress_VM>().HasNoKey();
            modelBuilder.Entity<CheckPostalAddressModel>().HasNoKey();
            modelBuilder.Entity<Address_VM>().HasNoKey();
            modelBuilder.Entity<OrderDetail_Model>().HasNoKey();



        }
        //OrderDetail_Model

        public DbSet<OrderDetail_Model>? OrderDetail_Model { get; set; }

        public DbSet<Domain.Models.Payment.Payment>? msg_Payments { get; set; }

        public DbSet<ClientCustomer>? msg_ClientCustomers { get; set; }

        public DbSet<Domain.Entities.User>? msg_Users { get; set; }
        public DbSet<MailInfo>? msg_MailInfos { get; set; }
        public DbSet<CompanyInfo>? msg_CompanyInfos { get; set; }

        public DbSet<ClientOrder>? msg_ClientOrders { get; set; }
        public DbSet<ClientOrderBasketItem>? msg_ClientOrderBasketItems { get; set; }


        public DbSet<Address_VM>? Address_VM { get; set; }

        public DbSet<CheckPostalAddressModel>? CheckPostalAddressModel { get; set; }

        public DbSet<CustomerAddress_VM>? CustomerAddress_VM { get; set; }

        public DbSet<CustomerList_VM>? CustomerList_VM { get; set; }

        public DbSet<InvoiceOfCustomer_VM>? InvoiceOfCustomer_VM { get; set; }

        public DbSet<QrOperationResponse>? QrOperationResponse { get; set; }
        public DbSet<DestroyItem_Response>? DestroyItem_Response { get; set; }

        public DbSet<ProductCountModel3>? ProductCountModel3 { get; set; }

        public DbSet<AvailableShelf>? AvailableShelfs { get; set; }

        public DbSet<TransferRequestListModel>? TransferRequestListModels { get; set; }
        public DbSet<QrCode>? ztQrCodes { get; set; }
        public DbSet<Log>? ztLogs { get; set; }
        public DbSet<ProductList_VM>? ProductListModel { get; set; }
        public DbSet<InvoiceErrorInfoModel>? InvoiceErrorInfoModel { get; set; }
        public DbSet<InventoryItemModel>? InventoryItemModels { get; set; }
        public DbSet<WarehosueTransferListModel>? WarehosueTransferListModel { get; set; }
        public DbSet<CountConfirmData>? CountConfirmData { get; set; }
        public DbSet<WarehouseTransferListFilterModel>? WarehouseTransferListFilterModel { get; set; }

        public DbSet<ZTMSRAFInvoiceDetailBP>? ZTMSRAFInvoiceDetailBP { get; set; }
        public DbSet<TransferData>? TransferData { get; set; }

        public DbSet<TransferModel>? TransferModel { get; set; }

        public DbSet<ProductCountModel2>? ztProductCountModel2 { get; set; }

        public DbSet<WarehouseFormModel>? WarehouseFormModel { get; set; }

        public DbSet<CreatePurchaseInvoice>? CreatePurchaseInvoices { get; set; }
        public DbSet<FastTransferModel>? FastTransferModels { get; set; }

        public DbSet<ZTMSRAFSAYIM3>? ZTMSRAFSAYIM3 { get; set; }

        public DbSet<CountListModel>? CountListModels { get; set; }
        public DbSet<CountedProduct>? CountedProducts { get; set; }

        public DbSet<SalesPersonModel>? SalesPersonModels { get; set; }
        public DbSet<CollectedProduct>? CollectedProducts { get; set; }
        public DbSet<QrControlModel>? QrControlModel { get; set; }

        public DbSet<EInvoiceModel>? ztEInvoiceModel { get; set; }
        public DbSet<CreatePurchaseInvoice>? ztCreatePurchaseInvoice { get; set; }

        public DbSet<OrderData>? ztOrderData { get; set; }

        public DbSet<RecepieModel>? ztRecepieModel { get; set; }
        public DbSet<Domain.Models.NEBIM.Order.Invoice>? ztInvoice { get; set; }
        public DbSet<WarehosueOperationListModel>? ztWarehosueOperationListModel { get; set; }
        public DbSet<WarehouseOperationDetailModel>? ztWarehosueOperationDetail { get; set; } //
        public DbSet<OrderBillingModel>? ztOrderBillingModel { get; set; }
        public DbSet<ProductOfOrderModel>? ztProductOfOrderModel { get; set; }

        public DbSet<SaleOrderModel>? SaleOrderModels { get; set; }
        public DbSet<ShelfModel>? ztShelves { get; set; }

        public DbSet<CategoryModel>? ztCategories { get; set; }
        public DbSet<BarcodeAddModel>? ztBarcodeAddModel { get; set; }
        public DbSet<BarcodeModel>? BarcodeModels { get; set; }
        public DbSet<OrderSaleDetailModel>? OrderSaleDetails { get; set; }
        public DbSet<OfficeModel>? ztOfficeModel { get; set; }
        public DbSet<WarehouseOfficeModel>? ztWarehouseModel { get; set; }
        public DbSet<ReadyToShipmentPackageModel>? ztReadyToShipmentPackageModel { get; set; }
        public DbSet<InvoiceNumberModel>? ztInvoiceNumberModel { get; set; }
        public DbSet<RemainingProductsModel>? ztRemainingProductsModel { get; set; }
        public DbSet<CountProductRequestModel>? ztCountProductRequestModel { get; set; }
        public DbSet<CustomerModel>? ztCustomerModel { get; set; } //InventoryResponseModel
        public DbSet<InventoryResponseModel>? InventoryResponseModels { get; set; }

        public DbSet<ProductCountModel>? ztProductCountModel { get; set; }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
