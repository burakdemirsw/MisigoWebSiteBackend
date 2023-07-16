
using GoogleAPI.Domain.Models;
using GoogleAPI.Domain.Models.NEBIM;
using Microsoft.EntityFrameworkCore;


namespace GoogleAPI.Persistance.Contexts
{
    public class GooleAPIDbContext : DbContext
    {

        public GooleAPIDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<SaleOrderModel>().HasNoKey();         
            modelBuilder.Entity<BarcodeModel>().HasNoKey();
            modelBuilder.Entity<OrderSaleDetail>().HasNoKey();
            modelBuilder.Entity<OrderBillingModel>().HasNoKey();
            modelBuilder.Entity<ProductOfOrderModel>().HasNoKey();
            modelBuilder.Entity<OfficeModel>().HasNoKey();
            modelBuilder.Entity<WarehouseOfficeModel>().HasNoKey();



        }
        //OrderBillingModel WarehouseFormModel
        public DbSet<OrderBillingModel>? ztOrderBillingModel { get; set; }
        public DbSet<ProductOfOrderModel>? ztProductOfOrderModel { get; set; }

        public DbSet<SaleOrderModel>? SaleOrderModels { get; set; }
        public DbSet<ShelfModel>? ztShelves { get; set; }

        public DbSet<CategoryModel>? ztCategories { get; set; }
        public DbSet<BarcodeAddModel>? ztBarcodeAddModel { get; set; } //geçici BarcodeModel
        public DbSet<BarcodeModel>? BarcodeModels { get; set; } //geçici BarcodeModel SatisSiparisDetay

        public DbSet<OrderSaleDetail>? OrderSaleDetails { get; set; } //geçici BarcodeModel SatisSiparisDetay

        public DbSet<OfficeModel>? ztOfficeModel { get; set; } //geçici BarcodeModel SatisSiparisDetay

        public DbSet<WarehouseOfficeModel>? ztWarehouseModel { get; set; } //geçici BarcodeModel SatisSiparisDetay


        // public DbSet<Variation>? Variations { get; set; }


        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
