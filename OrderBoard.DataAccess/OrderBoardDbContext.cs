﻿using Microsoft.EntityFrameworkCore;
using OrderBoard.DataAccess.Configurations;

namespace OrderBoard.DataAccess
{
    public class OrderBoardDbContext : DbContext
    {
        public OrderBoardDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new ItemConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new FileContentConfiguration());
        }
    }
}
