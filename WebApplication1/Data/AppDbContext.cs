using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets
        public DbSet<Product> products { get; set; }
        public DbSet<Stock> stocks { get; set; }
        public DbSet<Order> orders { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet<OrderDetail> orderDetail { get; set; }
        public DbSet<Review> reviews { get; set; }
        public DbSet<ShippingAddress> shippingAddresses { get; set; }
        public DbSet<User> users { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<Promotion> promotions { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Message> Messages => Set<Message>();
        // 🔴 removed the duplicate: public DbSet<CartItem> carItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== products =====
            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");

                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(250);
                e.Property(x => x.Description).HasColumnName("description"); // MySQL TEXT
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
                e.Property(x => x.QtyInStock).HasColumnName("QtyInstock");
                e.Property(x => x.Image).HasColumnName("image").HasMaxLength(100);
                e.Property(x => x.CodeOrBarcode).HasColumnName("code_or_barcode");
                e.Property(x => x.CreatedDate).HasColumnName("created_date");
                e.Property(x => x.CategoryId).HasColumnName("category_id");

                e.HasOne(x => x.Category)
                 .WithMany(c => c.Products)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== categories =====
            modelBuilder.Entity<Category>(e =>
            {
                e.ToTable("categories");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.CategoryName).HasColumnName("category_name").HasMaxLength(100);
                e.Property(x => x.Description).HasColumnName("description"); // TEXT
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");
                e.Property(x => x.IsActive).HasColumnName("is_active");
            });

            // ===== users =====
         modelBuilder.Entity<User>(e =>
{
    e.ToTable("users");
    e.HasKey(x => x.Id);
    e.Property(x => x.Id).HasColumnName("id");
    e.Property(x => x.Name).HasColumnName("name");
    e.Property(x => x.Email).HasColumnName("email");
    e.Property(x => x.Password).HasColumnName("password");
    e.Property(x => x.Phone).HasColumnName("phone");
    e.Property(x => x.RegisteredDate).HasColumnName("registered_date");
    e.Property(x => x.Address).HasColumnName("address");

    // if the DB column is NOT literally "role", put the exact name here:
  // <- change to the real name if different
});


            // ===== shipping_addresses =====
            modelBuilder.Entity<ShippingAddress>(e =>
            {
                e.ToTable("shipping_addresses");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.RecipientName).HasColumnName("recipient_name").HasMaxLength(200);
                e.Property(x => x.Address).HasColumnName("address").HasColumnType("longtext");
           

                e.HasOne(d => d.User)
                 .WithMany(p => p.shipping_address)
                 .HasForeignKey(d => d.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== orders =====
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(10,2)");
                e.Property(x => x.OrderDate).HasColumnName("order_date");
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(50);

                e.HasOne(o => o.User)
                 .WithMany(u => u.orders)
                 .HasForeignKey(o => o.UserId)
                 .OnDelete(DeleteBehavior.Restrict);

                e.HasMany(x => x.OrderDetails)
                 .WithOne(d => d.Order!)
                 .HasForeignKey(d => d.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== order_detail =====
            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.ToTable("order_detail");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.OrderId).HasColumnName("order_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Qty).HasColumnName("qty");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(10,2)");

                e.HasOne(d => d.Product)
                 .WithMany(p => p.OrderDetails)
                 .HasForeignKey(d => d.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== cart =====
            modelBuilder.Entity<Cart>(e =>
            {
                e.ToTable("cart");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.CreateAt).HasColumnName("create_at");

                e.HasMany(x => x.CartItems)
                 .WithOne(i => i.Cart!)
                 .HasForeignKey(i => i.CartId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== cart_items =====
            modelBuilder.Entity<CartItem>(e =>
            {
                e.ToTable("cart_items");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.CartId).HasColumnName("cart_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Quantity).HasColumnName("quantity");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(10,2)");

                e.HasOne(i => i.Product)
                 .WithMany(p => p.CartItems)
                 .HasForeignKey(i => i.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });

            // ===== stocks =====
            modelBuilder.Entity<Stock>(e =>
            {
                e.ToTable("stock");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Qty).HasColumnName("qty");
                e.Property(x => x.UpdatedAt).HasColumnName("updated_at");

                e.HasOne(s => s.Product)
                 .WithMany(p => p.Stocks)
                 .HasForeignKey(s => s.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== reviews =====
            modelBuilder.Entity<Review>(e =>
            {
                e.ToTable("reviews");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Rating).HasColumnName("rating");
                e.Property(x => x.Comment).HasColumnName("comment"); // TEXT
                e.Property(x => x.ReviewDate).HasColumnName("review_date");

                e.HasOne(r => r.User)
                 .WithMany(u => u.reviews)
                 .HasForeignKey(r => r.UserId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.HasOne(r => r.Product)
                 .WithMany(p => p.Reviews)
                 .HasForeignKey(r => r.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== promotions =====
            modelBuilder.Entity<Promotion>(e =>
            {
                e.ToTable("promotions");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.DiscountPercent).HasColumnName("discount_percent");
                e.Property(x => x.StartDate).HasColumnName("start_date");
                e.Property(x => x.EndDate).HasColumnName("end_date");

                e.HasOne(p => p.Product)
                 .WithMany(pr => pr.Promotions)
                 .HasForeignKey(p => p.ProductId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== messages =====
            modelBuilder.Entity<Message>(e =>
            {
                e.ToTable("messages");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.SenderId).HasColumnName("sender_id");
                e.Property(x => x.ReceiverId).HasColumnName("receiver_id");
                e.Property(x => x.Subject).HasColumnName("subject");
                e.Property(x => x.Body).HasColumnName("body");
                e.Property(x => x.CreatedAt).HasColumnName("created_at");
                e.Property(x => x.IsRead).HasColumnName("is_read");
                e.Property(x => x.IsArchived).HasColumnName("is_archived");
                e.Property(x => x.ParentId).HasColumnName("parent_id");
            });
        }
    }
}
