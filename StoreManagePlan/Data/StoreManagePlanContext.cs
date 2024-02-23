using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Models;

namespace StoreManagePlan.Data
{
    public class StoreManagePlanContext : DbContext
    {
        public StoreManagePlanContext(DbContextOptions<StoreManagePlanContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Item { get; set; } 
        public DbSet<ItemFeature> ItemFeature { get; set; } 
        public DbSet<Store> Store { get; set; }
        public DbSet<StoreType> StoreType { get; set; }
        public DbSet<ImportLog> ImportLog { get; set; }
        public DbSet<Bom> Bom { get; set; }
        public DbSet<StoreRelation> StoreRelation { get; set; }
        public DbSet<SaleHistory> SaleHistory { get; set; }
        public DbSet<Week> Week { get; set; }
        public DbSet<PlanDetail> PlanDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Item");
            modelBuilder.Entity<ItemFeature>().ToTable("ItemFeature").HasKey(pf => new { pf.store_id, pf.item_id });
            modelBuilder.Entity<StoreRelation>().ToTable("StoreRelation");
            modelBuilder.Entity<Store>().ToTable("Store");
            modelBuilder.Entity<StoreType>().ToTable("StoreType");
            modelBuilder.Entity<ImportLog>().ToTable("ImportLog");
            modelBuilder.Entity<Week>().ToTable("Week");
            modelBuilder.Entity<SaleHistory>().ToTable("SaleHistory");
            modelBuilder.Entity<Bom>().ToTable("Bom").HasKey(pf => new { pf.sku_id, pf.ingredient_sku });
            modelBuilder.Entity<PlanDetail>().ToTable("PlanDetail");
        }
    }
}
