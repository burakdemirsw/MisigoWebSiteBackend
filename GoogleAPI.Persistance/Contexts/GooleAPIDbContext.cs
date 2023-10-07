using GoogleAPI.Domain.Models.NEBIM.Category;
using GoogleAPI.Domain.Models.NEBIM.Customer;
using GoogleAPI.Domain.Models.NEBIM.Invoice;
using GoogleAPI.Domain.Models.NEBIM.Order;
using GoogleAPI.Domain.Models.NEBIM.Product;
using GoogleAPI.Domain.Models.NEBIM.Shelf;
using GoogleAPI.Domain.Models.NEBIM.Warehouse;
using Microsoft.EntityFrameworkCore;


namespace GoogleAPI.Persistance.Contexts
{
    public class GooleAPIDbContext : DbContext
    {

        public GooleAPIDbContext(DbContextOptions options) : base(options) { }
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
            modelBuilder.Entity<RecepieModel>().HasNoKey();
            modelBuilder.Entity<CustomerModel>().HasNoKey();
            modelBuilder.Entity<OrderData>().HasNoKey();
            modelBuilder.Entity<CreatePurchaseInvoice>().HasNoKey();
            modelBuilder.Entity<SalesPersonModel>().HasNoKey();//CollectedProduct
            modelBuilder.Entity<Domain.Models.NEBIM.Order.Invoice>().HasNoKey();
            modelBuilder.Entity<CollectedProduct>().HasNoKey();//CollectedProduct
            modelBuilder.Entity<CountListModel>().HasNoKey();//CollectedProduct CountedProduct
            modelBuilder.Entity<CountedProduct>().HasNoKey();//CollectedProduct CountedProduct
            modelBuilder.Entity<CreatePurchaseInvoice>().HasNoKey();//CollectedProduct CountedProduct



        }
        //ZTMSRAFSAYIM3 CreatePurchaseInvoice FastTransferModel
        public DbSet<CreatePurchaseInvoice>? CreatePurchaseInvoices { get; set; }
        public DbSet<FastTransferModel>? FastTransferModels { get; set; }

        public DbSet<ZTMSRAFSAYIM3>? ZTMSRAFSAYIM3 { get; set; }

        public DbSet<CountListModel>? CountListModels { get; set; }
        public DbSet<CountedProduct>? CountedProducts { get; set; }

        public DbSet<SalesPersonModel>? SalesPersonModels { get; set; } 
        public DbSet<CollectedProduct>? CollectedProducts { get; set; } 

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
        public DbSet<CustomerModel>? ztCustomerModel { get; set; }

        public DbSet<ProductCountModel>? ztProductCountModel { get; set; } 

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
