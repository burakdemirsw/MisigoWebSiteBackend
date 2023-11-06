
using GoogleAPI.Domain.DTO;
using GoogleAPI.Domain.Entities;
using GoogleAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace GoogleAPI.Persistance.Contexts
{
    public class GooleAPIDbContext : DbContext
    {

        public GooleAPIDbContext(DbContextOptions options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
                    modelBuilder.Entity<Product>()
            .HasMany(p => p.Photos)
            .WithOne(pp => pp.Product)
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<ProductPhoto>()
                .HasKey(pp => new { pp.ProductId, pp.PhotoId });

            modelBuilder.Entity<ProductCard_VM>().HasNoKey();
            modelBuilder.Entity<ProductDetail_DTO>().HasNoKey();



        }
        public DbSet<ProductDetail_DTO>? ProductDetail_DTO { get; set; }

        public DbSet<Product>? Products { get; set; }
        public DbSet<Color>? Colors { get; set; }
        public DbSet<Brand>? Brands { get; set; }
        public DbSet<Photo>? Photos { get; set; }

        public DbSet<Dimension>? Dimensions { get; set; }
        public DbSet<MainCategory>? MainCategories { get; set; }
        public DbSet<ProductVariation_VM>? ProductVariation_VM { get; set; }
        public DbSet<Brand_VM>? Brand_VM { get; set; }

        public DbSet<ProductCard_VM>? ProductCard_VM { get; set; }


        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default
        )
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
