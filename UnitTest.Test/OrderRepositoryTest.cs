using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Order.API.Repositories;
using Xunit;
using System.Net.Sockets;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace UnitTest.Test
{
    public class OrderRepositoryTest
    {
        private Mock<DbSet<Order.API.Models.Order>> _mockOrderSet;
        private DbContextOptions<AppDbContext> _dbContextOptions;
        private Mock<AppDbContext> _mockContext;
        private OrderRepository _repository;

        public OrderRepositoryTest()
        {
            _mockOrderSet = new Mock<DbSet<Order.API.Models.Order>>();

            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());

            _mockContext.Setup(m => m.Orders).Returns(_mockOrderSet.Object);

            _repository = new OrderRepository(_mockContext.Object);

            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                            .UseInMemoryDatabase(databaseName: "TestDb")
                            .Options;
        }

        [Fact]
        public async Task createTestCase()
        {
            var order = new Order.API.Models.Order
            {
                Id = 1,
                BuyerId = "Buyer1",
                OrderStatus = OrderStatus.Suspend,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 101, Count = 2, Price = 10}
                }
            };

            await _repository.CreateAsync(order);

            _mockContext.Verify(m => m.Orders.AddAsync(order, default), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }

       
        [Fact]
        public async Task getAllTestCase()
        {
            var orders = new List<Order.API.Models.Order>
        {
            new Order.API.Models.Order
            {
                Id = 1,
                BuyerId = "1",
                OrderStatus = OrderStatus.Suspend,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Count = 2, Price = 10 }
                }
            },
            new Order.API.Models.Order
            {
                Id = 2,
                BuyerId = "2",
                OrderStatus = OrderStatus.Success,
                Items = new List<OrderItem>
                {
                    new OrderItem { ProductId = 2, Count = 1, Price = 20 }
                }
            }
        };

            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_dbContextOptions))
            {
                var repository = new OrderRepository(context);
                var result = await repository.GetAllAsync();

                Assert.NotNull(result);  
                Assert.Equal(2, result.Count);  
                Assert.Equal("1", result[0].BuyerId);
                Assert.Equal("2", result[1].BuyerId);  
            }
        }
    }
     
}
