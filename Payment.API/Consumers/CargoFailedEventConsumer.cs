using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.API.Models;
using Shared;

namespace Payment.API.Consumers
{
    public class CargoFailedEventConsumer : IConsumer<CargoFailedEvent>
    {
        private readonly ILogger<CargoFailedEventConsumer> _logger;
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public CargoFailedEventConsumer(AppDbContext context,ILogger<CargoFailedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<CargoFailedEvent> context)
        {
        
        _logger.LogInformation($"Cargo failed for user id={context.Message.BuyerId}");

        var payment = await _context.Payment.FirstOrDefaultAsync(x => x.id == Convert.ToInt32(context.Message.BuyerId));

        if (payment != null)
              {
              payment.Balance += context.Message.Price;
              await _context.SaveChangesAsync();
        }
         
        await _publishEndpoint.Publish(new PaymentFailedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, Message = "The delivery of the cargo was unsuccessful.", orderItems = context.Message.orderItems });
            
        }
    }
}
