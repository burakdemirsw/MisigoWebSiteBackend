using GoogleAPI.Domain.Models.NEBIM.Category;
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
            modelBuilder.Entity<WarehosueOperationDetailModel>().HasNoKey();
            modelBuilder.Entity<InvoiceNumberModel>().HasNoKey(); //RemainingProductsModel
            modelBuilder.Entity<RemainingProductsModel>().HasNoKey(); //RemainingProductsModel //CountProductRequestModel
            modelBuilder.Entity<CountProductRequestModel>().HasNoKey(); //RemainingProductsModel //CountProductRequestModel ztShelfNameModel
            modelBuilder.Entity<ProductCountModel>().HasNoKey(); //RemainingProductsModel //CountProductRequestModel ztShelfNameModel
            modelBuilder.Entity<RecepieModel>().HasNoKey(); //RemainingProductsModel //CountProductRequestModel ztShelfNameModel
            modelBuilder.Entity<Domain.Models.NEBIM.Order.Invoice>().HasNoKey(); //RemainingProductsModel //CountProductRequestModel ztShelfNameModel
            modelBuilder.Entity<OrderData>().HasNoKey();


        }
        //OrderBillingModel WarehouseFormModel EInvoiceModel
        public DbSet<EInvoiceModel>? ztEInvoiceModel { get; set; } //WarehosueOperationDetail

        public DbSet<OrderData>? ztOrderData { get; set; } //WarehosueOperationDetail

        public DbSet<RecepieModel>? ztRecepieModel { get; set; } //WarehosueOperationDetail
        public DbSet<Domain.Models.NEBIM.Order.Invoice>? ztInvoice { get; set; } //WarehosueOperationDetail
        public DbSet<WarehosueOperationListModel>? ztWarehosueOperationListModel { get; set; } //WarehosueOperationDetail
        public DbSet<WarehosueOperationDetailModel>? ztWarehosueOperationDetail { get; set; } //
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
        public DbSet<CountProductRequestModel>? ztCountProductRequestModel { get; set; } //ShelfNameModel

        public DbSet<ProductCountModel>? ztProductCountModel { get; set; } //ShelfNameModel

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
