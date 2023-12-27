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

        public DbSet<StoreManagePlan.Models.Item> Item { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>().ToTable("Item");

        }
    }
}
