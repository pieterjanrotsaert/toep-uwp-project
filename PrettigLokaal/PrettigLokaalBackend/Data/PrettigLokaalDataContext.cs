using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrettigLokaalBackend.DomainModels;
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
        }

        private static void MapAccount(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Account");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Merchant).WithOne(a => a.Account).HasForeignKey("Account", "MerchantId");
        }

        private static void MapMerchant(EntityTypeBuilder<Merchant> builder)
        {
            builder.ToTable("Merchant");
            builder.HasKey(a => a.Id);
            builder.HasMany(a => a.Images);
            builder.HasMany(a => a.Events).WithOne(a => a.Organizer);
            builder.HasMany(a => a.Promotions).WithOne(a => a.Organizer);
        }

        private static void MapMerchantSubs(EntityTypeBuilder<MerchantSubscription> builder)
        {
            builder.ToTable("MerchantSubscription");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Merchant).WithMany(a => a.Subscriptions);
            builder.HasOne(a => a.Account).WithMany(a => a.Subscriptions);
        }

        private static void MapEvent(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Image);
        }

        private static void MapPromotion(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotion");
            builder.HasKey(a => a.Id);
            builder.HasOne(a => a.Image);
        }

        private static void MapImage(EntityTypeBuilder<Image> builder)
        {
            builder.ToTable("Image");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Data).HasColumnType("nvarchar(MAX)");
        }
    }
}
