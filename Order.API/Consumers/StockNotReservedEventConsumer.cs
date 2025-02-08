using MassTransit;
using Microsoft.Extensions.Logging;
using Order.API.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReserved>
    {
        private readonly AppDbContext _context;

        private readonly ILogger<CargoArrivedEventConsumer> _logger;

        public StockNotReservedEventConsumer(AppDbContext context, ILogger<CargoArrivedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<StockNotReserved> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Fail;
                order.FailureMesage = context.Message.Message;
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Order (Id={context.Message.OrderId}) status changed : {order.OrderStatus}");
            }
            else
            {
                _logger.LogError($"Order (Id={context.Message.OrderId}) not found");
            }
        }
    }
}