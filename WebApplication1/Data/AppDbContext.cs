using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext>options):base(options) { }
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
        public DbSet<CartItem> carItems { get; set; }
        public DbSet<Message> Messages => Set<Message>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(e =>
            {
                e.ToTable("products"); // Table name exactly in DB
                e.HasKey(x => x.Id);

                e.Property(x => x.Id)
                    .HasColumnName("id");

                e.Property(x => x.Name)
      .HasColumnName("name")
      .HasMaxLength(250)
      .IsUnicode(true);

                e.Property(x => x.Description)
                 .HasColumnName("description")
                 .IsUnicode(true);

                e.Property(x => x.Price)
                    .HasColumnName("price")
                    .HasColumnType("decimal(10,2)");

        

                e.Property(x => x.Image)
                    .HasColumnName("image")
                    .HasMaxLength(100);



                e.Property(x => x.CodeOrBarcode)
                    .HasColumnName("Code_or_barcode");

                e.Property(x => x.CreatedDate)
                    .HasColumnName("CreatedDate");

                e.Property(x => x.CategoryId)
                    .HasColumnName("Category_ID"); // ✅ underscore matches DB
            });

            // AppDbContext.cs (OnModelCreating)
            modelBuilder.Entity<Order>(e =>
            {
                e.ToTable("orders");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.TotalPrice).HasColumnName("total_price").HasColumnType("decimal(10,2)");
                e.Property(x => x.OrderDate).HasColumnName("order_date");
                e.Property(x => x.Status).HasColumnName("status").HasMaxLength(50);

                e.HasMany(x => x.OrderDetails)
                 .WithOne(d => d.Order!)
                 .HasForeignKey(d => d.OrderId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<OrderDetail>(e =>
            {
                e.ToTable("Order_Detail");     // exact table name from your diagram
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.OrderId).HasColumnName("order_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");
                e.Property(x => x.Qty).HasColumnName("qty");
                e.Property(x => x.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
                e.HasOne(i => i.Product)
               .WithMany(p => p.OrderDetails)
               .HasForeignKey(i => i.ProductId)
               .OnDelete(DeleteBehavior.Restrict);
            });
            // AppDbContext.OnModelCreating
            modelBuilder.Entity<Cart>(e =>
            {
                e.ToTable("cart");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.UserId).HasColumnName("user_id");
                e.Property(x => x.CreateAt).HasColumnName("Create_At"); // ប្តូរ "CreatedAt" បើ column ឈ្មោះផ្សេង
                e.HasMany(x => x.CartItems)
                 .WithOne(i => i.Cart!)
                 .HasForeignKey(i => i.CartId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(e =>
            {
                e.ToTable("cart_items");
                e.HasKey(x => x.Id);

                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.CartId).HasColumnName("cart_id");
                e.Property(x => x.ProductId).HasColumnName("product_id");   // ✅ important
                e.Property(x => x.Quantity).HasColumnName("quantity");
                e.Property(x => x.Price).HasColumnName("price");
                e.HasOne(i => i.Product)
                 .WithMany(p => p.CartItems)
                 .HasForeignKey(i => i.ProductId)
                 .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("users");
                e.HasKey(x => x.Id);
                e.Property(x => x.Id).HasColumnName("id");
                e.Property(x => x.Name).HasColumnName("name").HasMaxLength(100);
                e.Property(x => x.Email).HasColumnName("email").HasMaxLength(100);
                e.Property(x => x.Password).HasColumnName("password").HasMaxLength(100);
                e.Property(x => x.Phone).HasColumnName("phone").HasMaxLength(20);
                e.Property(x => x.RegisteredDate).HasColumnName("RegisteredDate");
                e.Property(x => x.Address).HasColumnName("Address");
                e.Property(x => x.Role).HasColumnName("role").HasMaxLength(50);
            });
            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("categories");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id");

                entity.Property(e => e.CategoryName)
                      .HasColumnName("category_name")
                      .HasMaxLength(100);

                entity.Property(e => e.Description)
                      .HasColumnName("description");

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at");

                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at");

                entity.Property(e => e.IsActive)
                      .HasColumnName("is_active");
            });
            modelBuilder.Entity<ShippingAddress>(entity =>
            {
                entity.ToTable("shipping_addresses");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id");

                entity.Property(e => e.UserId)
                      .HasColumnName("user_id");

                entity.Property(e => e.RecipientName)
                      .HasColumnName("recipient_name")
                      .HasMaxLength(200);

                entity.Property(e => e.Address)
                      .HasColumnName("address")
                      .HasColumnType("varchar(max)");

                entity.Property(e => e.City)
                      .HasColumnName("city")
                      .HasMaxLength(100);

                entity.Property(e => e.ZipCode)
                      .HasColumnName("zip_code")
                      .HasMaxLength(20);

                entity.Property(e => e.Country)
                      .HasColumnName("country")
                      .HasMaxLength(50);

                // 🔗 Relationship with Users
                entity.HasOne(d => d.User)
                      .WithMany(p => p.shipping_address)
                      .HasForeignKey(d => d.UserId)
                      .HasConstraintName("FK_shipping_addresses_users");
            });


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
