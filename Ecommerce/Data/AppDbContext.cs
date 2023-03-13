using Ecommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Data
{
	public class AppDbContext : DbContext
	{
		public DbSet<Product>? Products { get; set; }
		public DbSet<MyOrder>? MyOrders { get; set; }
		public DbSet<ShippingAddress>? ShippingAddresses { get; set; }
		public DbSet<MyItem>? MyItems { get; set; }
		public DbSet<User>? Users { get; set; }
		public DbSet<MyImage>? MyImages { get; set; }


		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }


		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<MyItem>()
				.HasOne(it => it.MyOrder)
				.WithMany(or => or.MyItems)
				.HasForeignKey(it => it.MyOrderId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MyItem>()
				.HasOne(it => it.User)
				.WithMany(us => us.MyItems)
				.HasForeignKey(it => it.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<MyItem>()
				.HasOne(it => it.Product)
				.WithMany(pr => pr.MyItems)
				.HasForeignKey(it => it.ProductId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<ShippingAddress>()
				.HasOne(sa => sa.MyOrder)
				.WithOne(or => or.ShippingAddress)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<MyOrder>()
				.HasOne(or => or.User)
				.WithMany(us => us.MyOrders)
				.HasForeignKey(or => or.UserId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<MyImage>()
				.HasOne(img => img.Product)
				.WithMany(pr => pr.MyImages)
				.HasForeignKey(img => img.ProductId)
				.OnDelete(DeleteBehavior.NoAction);

		}

	}
}
