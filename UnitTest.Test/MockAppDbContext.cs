using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace UnitTest.Test
{
    public class MockAppDbContext: AppDbContext
    {
        public MockAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public override DbSet<Order.API.Models.Order> Orders { get; set; }
    }
}
