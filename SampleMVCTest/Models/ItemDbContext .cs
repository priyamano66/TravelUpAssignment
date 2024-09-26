using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SampleMVCTest.Models
{
    public class ItemDbContext : DbContext
    {
        public virtual DbSet<Item> Items { get; set; }

        // Specify the connection string name
        public ItemDbContext() : base("ItemDbContext")
        {
        }
    }
}