using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Rewrite.Internal.ApacheModRewrite;
using Microsoft.EntityFrameworkCore;
using ShoppingListArduino.Models;

namespace ShoppingListArduino.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<UserProduct> UserProducts { get; set; }
        public DbSet<UserProductRfid> UserProductRfids { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .ToTable("products")
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserProduct>()
                .ToTable("userproduct")
                .HasKey(t =>t.Id);

            modelBuilder.Entity<UserProduct>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserProduct>()
                .HasOne(pt => pt.Product)
                .WithMany(t => t.UserProducts)
                .HasForeignKey(pt => pt.ProductId);

            modelBuilder.Entity<UserProductRfid>()
                .ToTable("userproductrfids")
                .HasKey(x => x.Id);

            modelBuilder.Entity<UserProductRfid>()
                .HasOne(pt => pt.UserProduct)
                .WithMany(p => p.UserProductRfids)
                .HasForeignKey(pt => pt.UserProductId);


            modelBuilder.Entity<Product>()
                .HasData(new Product
                {
                    Id = 1,
                    Title = "Хліб прибалтійський \"Київхліб\"",
                    Barcode = "4820136405090",
                    Description = "Хліб темний"
                });

            base.OnModelCreating(modelBuilder);

        }

        public DbSet<ShoppingListArduino.Models.UserProduct> UserProduct { get; set; }
    }
}
