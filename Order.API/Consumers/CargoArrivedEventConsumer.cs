
using MassTransit;
using Order.API.Models;
using Shared;

namespace Order.API.Consumers
{
    public class CargoArrivedEventConsumer : IConsumer<CargoArrivedEvent>
    {
        private readonly AppDbContext _context;

        private readonly ILogger<CargoArrivedEventConsumer> _logger;

        public CargoArrivedEventConsumer(AppDbContext context, ILogger<CargoArrivedEventConsumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CargoArrivedEvent> context)
        {
            var order = await _context.Orders.FindAsync(context.Message.OrderId);

            if (order != null)
            {
                order.OrderStatus = OrderStatus.Complete;
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