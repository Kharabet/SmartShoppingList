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
        public DbSet<ShoppingListArduino.Models.Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserProduct>()
                .HasKey(t => new { t.UserId, t.ProductId});

            modelBuilder.Entity<UserProduct>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserProducts)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserProduct>()
                .HasOne(pt => pt.Product)
                .WithMany(t => t.UserProducts)
                .HasForeignKey(pt => pt.ProductId);

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
    }
}
