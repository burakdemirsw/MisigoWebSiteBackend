﻿using GoogleAPI.Domain.Models.NEBIM.Category;
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


        }
        //OrderBillingModel WarehouseFormModel
        public DbSet<WarehosueOperationListModel>? ztWarehosueOperationListModel { get; set; } //WarehosueOperationDetail
        public DbSet<WarehosueOperationDetailModel>? ztWarehosueOperationDetail { get; set; } //
        public DbSet<OrderBillingModel>? ztOrderBillingModel { get; set; }
        public DbSet<ProductOfOrderModel>? ztProductOfOrderModel { get; set; }

        public DbSet<SaleOrderModel>? SaleOrderModels { get; set; }
        public DbSet<ShelfModel>? ztShelves { get; set; }

        public DbSet<CategoryModel>? ztCategories { get; set; }
        public DbSet<BarcodeAddModel>? ztBarcodeAddModel { get; set; } //geçici BarcodeModel
        public DbSet<BarcodeModel>? BarcodeModels { get; set; } //geçici BarcodeModel SatisSiparisDetay

        public DbSet<OrderSaleDetailModel>? OrderSaleDetails { get; set; } //geçici BarcodeModel SatisSiparisDetay

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
