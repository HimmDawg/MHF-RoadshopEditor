using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using RoadshopEditor.Models;

namespace RoadshopEditor.data
{
	public class RoadshopContext : DbContext
	{
		private readonly IConfiguration _configuration;

		public RoadshopContext(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public DbSet<RoadshopItem> RoadshopItems { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			modelBuilder
				.Entity<RoadshopItem>()
				.ToTable("normal_shop_items")
				.Ignore(item => item.Name)
				.HasKey(item => item.ItemHash);

			modelBuilder
				.Entity<RoadshopItem>()
				.Property(item => item.ShopId)
				.HasConversion(
					v => (int)v,
					v => (ShopCategory)v);
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			var connectionString = _configuration.GetConnectionString("RoadshopItemsDatabase");

			if (connectionString is null)
			{
				throw new Exception("ConnectionString to database was null. Please provide one.");
			}

			optionsBuilder.UseNpgsql(connectionString);
		}
	}
}
