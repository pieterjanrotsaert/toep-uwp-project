using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrettigLokaalBackend.Models.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PrettigLokaalBackend.Data
{
    public class PrettigLokaalDataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Merchant> Merchants { get; set; }
        public DbSet<MerchantSubscription> Subscriptions { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Coupon> Coupons { get; set; }

        public PrettigLokaalDataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<Account>(MapAccount);
            builder.Entity<Merchant>(MapMerchant);
            builder.Entity<MerchantSubscription>(MapMerchantSubs);
            builder.Entity<Event>(MapEvent);
            builder.Entity<Promotion>(MapPromotion);
            builder.Entity<Image>(MapImage);
            builder.Entity<Coupon>(MapCoupon);
        }

        private static void MapAccount(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Merchant).WithOne(p => p.Account).HasForeignKey("Account", "MerchantId");
            builder.HasMany(p => p.Coupons).WithOne(p => p.Account);
        }

        private static void MapMerchant(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchant");
            builder.HasKey(p => p.Id);
            builder.HasMany(p => p.Images);
            builder.HasMany(p => p.Events).WithOne(p => p.Organizer);
            builder.HasMany(p => p.Promotions).WithOne(p => p.Organizer);
        }

        private static void MapMerchantSubs(EntityTypeBuilder<MerchantSubscription> builder)
        {
            builder.ToTable("MerchantSubscription");
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Merchant).WithMany(p => p.Subscriptions);
            builder.HasOne(p => p.Account).WithMany(p => p.Subscriptions);
        }

        private static void MapEvent(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Image);
        }

        private static void MapPromotion(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotion");
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Image);
        }

        private static void MapImage(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Image");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Data).HasColumnType("nvarchar(MAX)");
        }

        private static void MapCoupon(EntityTypeBuilder<Coupon> builder)
        {
            builder.ToTable("Coupon");
            builder.HasKey(p => p.Id);
            builder.HasOne(p => p.Promotion);
        }
    }
}
