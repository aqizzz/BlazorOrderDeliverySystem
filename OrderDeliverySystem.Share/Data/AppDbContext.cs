using Microsoft.EntityFrameworkCore;
using OrderDeliverySystem.Share.Data.Models;
using System.Net;

namespace OrderDeliverySystem.Share.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<DeliveryWorker> DeliveryWorkers { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<DeliveryTask> DeliveryTasks { get; set; }
        public DbSet<WorkerLocation> WorkerLocations { get; set; }
        public DbSet<AddressModel> Addresses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User Table
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserId)
                    .UseIdentityColumn(); 

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.Role)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(u => u.IsActive)
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETDATE()") // SQL Server
                    .ValueGeneratedOnAdd();
            });

            // Configure Customer Table
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.CustomerId);
                entity.HasOne(c => c.User)
                    .WithOne(u => u.Customer)
                    .HasForeignKey<Customer>(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryWorker Table
            modelBuilder.Entity<DeliveryWorker>(entity =>
            {
                entity.HasKey(dw => dw.WorkerId);
                entity.HasOne(dw => dw.User)
                    .WithOne(u => u.DeliveryWorker)
                    .HasForeignKey<DeliveryWorker>(dw => dw.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(dw => dw.CommissionRate)
                    .HasPrecision(5, 2);
            });


            // Configure Merchant Table
            modelBuilder.Entity<Merchant>(entity =>
            {
                entity.HasKey(m => m.MerchantId);
                entity.HasOne(m => m.User)
                    .WithOne(u => u.Merchant)
                    .HasForeignKey<Merchant>(m => m.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Item Table
            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(i => i.ItemId);
                entity.Property(i => i.ItemPrice)
                    .HasPrecision(10, 2);
            });

            // Configure Cart Table
            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(c => c.CartId);
                entity.HasOne(c => c.Customer)
                    .WithMany(cu => cu.Carts)
                    .HasForeignKey(c => c.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure CartItem Table
            modelBuilder.Entity<CartItem>()
            .HasOne(ci => ci.Item)
            .WithMany(i => i.CartItems)
            .HasForeignKey(ci => ci.ItemId)
            .OnDelete(DeleteBehavior.Restrict);

            // Configure Order Table
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.OrderId);
                entity.HasOne(o => o.Merchant)
                    .WithMany(m => m.Orders)
                    .HasForeignKey(o => o.MerchantId)
                    .OnDelete(DeleteBehavior.Restrict); // Change this line

                entity.HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(o => o.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict); // You might want to add this line as well

                entity.Property(o => o.TotalAmount)
                    .HasPrecision(10, 2);

                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.ToTable(tb => tb.HasCheckConstraint("CK_Order_Status", 
                    "Status IN ('Pending', 'Approved', 'In Delivery', 'Delivered', 'Cancelled')"));
            });

            // Configure OrderItem Table
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(oi => oi.OrderItemId);
                entity.Property(oi => oi.Quantity)
                    .IsRequired();

                entity.HasOne(oi => oi.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(oi => oi.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(oi => oi.Item)
                    .WithMany(i => i.OrderItems)
                    .HasForeignKey(oi => oi.ItemId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Discount)
                    .HasColumnType("decimal(5, 2)");

                entity.Property(e => e.PriceAtOrder)
                    .HasColumnType("decimal(18, 2)");

                entity.Property(e => e.Tax)
                    .HasColumnType("decimal(5, 2)");
            }); 

            // Configure Review Table
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewId);
                entity.HasOne(r => r.Order)
                    .WithMany(o => o.Reviews)
                    .HasForeignKey(r => r.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Customer)
                    .WithMany(o => o.Reviews)
                    .HasForeignKey(r => r.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(tb => tb.HasCheckConstraint("CK_Reviews_Rating",
                    "Rating BETWEEN 1 AND 5"));
            });
 
            // Configure DeliveryTask Table
            modelBuilder.Entity<DeliveryTask>(entity =>
            {
                entity.HasKey(dt => dt.TaskId);
                entity.HasOne(dt => dt.DeliveryWorker)
                    .WithMany(dw => dw.DeliveryTasks)
                    .HasForeignKey(dt => dt.WorkerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.ToTable(tb => tb.HasCheckConstraint("CK_DeliveryTask_Status",
                    "Status IN ('Assigned', 'Completed', 'Failed')"));
            });

            // Configure WorkerLocation Table
            modelBuilder.Entity<WorkerLocation>(entity =>
            {
                entity.HasKey(wl => wl.WorkerLocationId);
                entity.HasOne(wl => wl.DeliveryWorker)
                    .WithOne(dw => dw.WorkerLocation)
                    .HasForeignKey<WorkerLocation>(wl => wl.WorkerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(wl => wl.Latitude)
                    .HasColumnType("decimal(11, 8)");

                entity.Property(wl => wl.Longitude)
                    .HasColumnType("decimal(11, 8)");
            });

            // Configure Address Table
            modelBuilder.Entity<AddressModel>(entity =>
            {
                entity.HasKey(a => a.AddressId);

                entity.HasIndex(a => new { a.UserId, a.Type }).IsUnique();

                entity.HasOne(a => a.User)
                    .WithMany(u => u.Addresses)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
