using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PartsPaladin.Models;

namespace PartsPaladin.Data
{
    public class PartsPaladinContext : DbContext
    {
        public PartsPaladinContext (DbContextOptions<PartsPaladinContext> options)
            : base(options)
        {
        }

        public DbSet<PartsPaladin.Models.Customer> Customer { get; set; } = default!;
        public DbSet<PartsPaladin.Models.Cart> Cart { get; set; } = default!;
        public DbSet<PartsPaladin.Models.CartItems> CartItems { get; set; } = default!;
        public DbSet<PartsPaladin.Models.OrderDetails> OrderDetails { get; set; } = default!;
        public DbSet<PartsPaladin.Models.Orders> Orders { get; set; } = default!;
        public DbSet<PartsPaladin.Models.Product> Product { get; set; } = default!;
        public DbSet<PartsPaladin.Models.Records> Records { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Orders)
                .WithOne(o => o.Customer)
                .HasForeignKey(o => o.customer_id);

            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Carts)
                .WithOne(cart => cart.Customer)
                .HasForeignKey(cart => cart.customer_id);

            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.cart_id);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.CartItems)
                .WithOne(ci => ci.Product)
                .HasForeignKey(ci => ci.product_id);

            modelBuilder.Entity<Orders>()
                .HasMany(o => o.OrderDetails)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.order_id);

            modelBuilder.Entity<Product>()
                .HasMany(p => p.OrderDetails)
                .WithOne(od => od.Product)
                .HasForeignKey(od => od.product_id);
        }
    }
}
