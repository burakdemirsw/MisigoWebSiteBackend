﻿// <auto-generated />
using System;
using GoogleAPI.Persistance.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    [DbContext(typeof(GooleAPIDbContext))]
    [Migration("20230606113351_new15")]
    partial class new15
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("CategoryPhoto", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<int>("PhotosId")
                        .HasColumnType("int");

                    b.HasKey("CategoriesId", "PhotosId");

                    b.HasIndex("PhotosId");

                    b.ToTable("CategoryPhoto");
                });

            modelBuilder.Entity("CategoryProductCard", b =>
                {
                    b.Property<int>("CategoriesId")
                        .HasColumnType("int");

                    b.Property<int>("ProductCardsId")
                        .HasColumnType("int");

                    b.HasKey("CategoriesId", "ProductCardsId");

                    b.HasIndex("ProductCardsId");

                    b.ToTable("CategoryProductCard");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Barcode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BarcodeTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BarcodeTypeId");

                    b.ToTable("Barcodes");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.BarcodeType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FixedNumber")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("BarcodeTypes");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Brand", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Brands");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Color", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Colors");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Dimention", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.ToTable("Dimentions");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("PhotoTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("PhotoTypeId");

                    b.ToTable("Photos");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.PhotoType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("CategoryPhoto")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("ProductPhoto")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("PhotoTypes");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("BarcodeId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductCardId")
                        .HasColumnType("int");

                    b.Property<int>("ProductStockId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BarcodeId")
                        .IsUnique();

                    b.HasIndex("ProductCardId");

                    b.HasIndex("ProductStockId")
                        .IsUnique();

                    b.ToTable("Products");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductCard", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool?>("Blocked")
                        .HasColumnType("bit");

                    b.Property<int>("BrandId")
                        .HasColumnType("int");

                    b.Property<bool?>("CardBlocked")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("FreeShipping")
                        .HasColumnType("bit");

                    b.Property<bool?>("New")
                        .HasColumnType("bit");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("ProductPriceId")
                        .HasColumnType("int");

                    b.Property<int>("ProductPropertyId")
                        .HasColumnType("int");

                    b.Property<int>("ProductSeoDetailId")
                        .HasColumnType("int");

                    b.Property<bool?>("ShowCase")
                        .HasColumnType("bit");

                    b.Property<string>("StockCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("BrandId");

                    b.HasIndex("ProductPriceId")
                        .IsUnique();

                    b.HasIndex("ProductPropertyId")
                        .IsUnique();

                    b.HasIndex("ProductSeoDetailId")
                        .IsUnique();

                    b.ToTable("ProductCards");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductPrice", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("DiscountedPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MemberTypePrice1")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MemberTypePrice2")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MemberTypePrice3")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MemberTypePrice4")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("MemberTypePrice5")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("PurchasePrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal?>("SellingPrice")
                        .HasColumnType("decimal(18,2)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<decimal?>("VATRate")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.ToTable("ProductPrices");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductProperty", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CoverLetter")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("KeyWords")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SalesUnit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecialField1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecialField2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecialField3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecialField4")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SpecialField5")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ProductProperty");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductSeoDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("RateValue")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SeoHeaderLine")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SeoPageDescription")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("ProductSeoDetails");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductStock", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("ColorID")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DimentionID")
                        .HasColumnType("int");

                    b.Property<int>("In_Qty1")
                        .HasColumnType("int");

                    b.Property<int>("In_Qty2")
                        .HasColumnType("int");

                    b.Property<int>("Out_Qty1")
                        .HasColumnType("int");

                    b.Property<int>("Out_Qty2")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("ColorID")
                        .IsUnique();

                    b.HasIndex("DimentionID")
                        .IsUnique();

                    b.ToTable("ProductStocks");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("ProductCardId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.HasIndex("ProductCardId");

                    b.ToTable("SubCategories");
                });

            modelBuilder.Entity("GooleAPI.Application.Models.VM_Get_ProductModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Barcode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool?>("Blocked")
                        .HasColumnType("bit");

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("CategoryDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ColorDescription")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool?>("New")
                        .HasColumnType("bit");

                    b.Property<string>("ProductName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("PurchasePrice")
                        .HasColumnType("int");

                    b.Property<int?>("SellingPrice")
                        .HasColumnType("int");

                    b.Property<int?>("StockAmount")
                        .HasColumnType("int");

                    b.Property<string>("StockCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Url")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("VM_Get_ProductModels");
                });

            modelBuilder.Entity("PhotoProductCard", b =>
                {
                    b.Property<int>("PhotoId")
                        .HasColumnType("int");

                    b.Property<int>("ProductCardsId")
                        .HasColumnType("int");

                    b.HasKey("PhotoId", "ProductCardsId");

                    b.HasIndex("ProductCardsId");

                    b.ToTable("PhotoProductCard");
                });

            modelBuilder.Entity("CategoryPhoto", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.Photo", null)
                        .WithMany()
                        .HasForeignKey("PhotosId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("CategoryProductCard", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Category", null)
                        .WithMany()
                        .HasForeignKey("CategoriesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductCard", null)
                        .WithMany()
                        .HasForeignKey("ProductCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Barcode", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.BarcodeType", "BarcodeType")
                        .WithMany("Barcodes")
                        .HasForeignKey("BarcodeTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BarcodeType");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Color", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Product", null)
                        .WithMany("Colors")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Dimention", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Product", null)
                        .WithMany("Dimentions")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Photo", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.PhotoType", "PhotoType")
                        .WithMany("Photos")
                        .HasForeignKey("PhotoTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PhotoType");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Product", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Barcode", "Barcode")
                        .WithOne("Product")
                        .HasForeignKey("GoogleAPI.Domain.Entities.Product", "BarcodeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductCard", "ProductCard")
                        .WithMany("Products")
                        .HasForeignKey("ProductCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductStock", "ProductStock")
                        .WithOne("Product")
                        .HasForeignKey("GoogleAPI.Domain.Entities.Product", "ProductStockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Barcode");

                    b.Navigation("ProductCard");

                    b.Navigation("ProductStock");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductCard", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Brand", "Brand")
                        .WithMany("ProductCards")
                        .HasForeignKey("BrandId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductPrice", "ProductPrice")
                        .WithOne("ProductCard")
                        .HasForeignKey("GoogleAPI.Domain.Entities.ProductCard", "ProductPriceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductProperty", "ProductProperty")
                        .WithOne("ProductCard")
                        .HasForeignKey("GoogleAPI.Domain.Entities.ProductCard", "ProductPropertyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductSeoDetail", "ProductSeoDetail")
                        .WithOne("ProductCard")
                        .HasForeignKey("GoogleAPI.Domain.Entities.ProductCard", "ProductSeoDetailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Brand");

                    b.Navigation("ProductPrice");

                    b.Navigation("ProductProperty");

                    b.Navigation("ProductSeoDetail");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductStock", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Color", "Color")
                        .WithOne("ProductStock")
                        .HasForeignKey("GoogleAPI.Domain.Entities.ProductStock", "ColorID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.Dimention", "Dimention")
                        .WithOne("ProductStock")
                        .HasForeignKey("GoogleAPI.Domain.Entities.ProductStock", "DimentionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Color");

                    b.Navigation("Dimention");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.SubCategory", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Category", "Category")
                        .WithMany("Subcategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductCard", null)
                        .WithMany("SubCategories")
                        .HasForeignKey("ProductCardId");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("PhotoProductCard", b =>
                {
                    b.HasOne("GoogleAPI.Domain.Entities.Photo", null)
                        .WithMany()
                        .HasForeignKey("PhotoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GoogleAPI.Domain.Entities.ProductCard", null)
                        .WithMany()
                        .HasForeignKey("ProductCardsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Barcode", b =>
                {
                    b.Navigation("Product")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.BarcodeType", b =>
                {
                    b.Navigation("Barcodes");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Brand", b =>
                {
                    b.Navigation("ProductCards");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Category", b =>
                {
                    b.Navigation("Subcategories");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Color", b =>
                {
                    b.Navigation("ProductStock")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Dimention", b =>
                {
                    b.Navigation("ProductStock")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.PhotoType", b =>
                {
                    b.Navigation("Photos");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.Product", b =>
                {
                    b.Navigation("Colors");

                    b.Navigation("Dimentions");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductCard", b =>
                {
                    b.Navigation("Products");

                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductPrice", b =>
                {
                    b.Navigation("ProductCard")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductProperty", b =>
                {
                    b.Navigation("ProductCard")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductSeoDetail", b =>
                {
                    b.Navigation("ProductCard")
                        .IsRequired();
                });

            modelBuilder.Entity("GoogleAPI.Domain.Entities.ProductStock", b =>
                {
                    b.Navigation("Product")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}