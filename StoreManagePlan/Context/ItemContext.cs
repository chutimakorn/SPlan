using Microsoft.EntityFrameworkCore;
using StoreManagePlan.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreManagePlan.Context
{
    public class ItemContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
    }
}
