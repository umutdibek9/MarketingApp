using MassTransit;
using Shared;

namespace Cargo.API.Consumers
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        private readonly ILogger<PaymentCompletedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentCompletedEventConsumer(ISendEndpointProvider sendEndpointProvider,ILogger<PaymentCompletedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            var address = "International";

            if (!context.Message.Address.Equals(address))
            {
                _logger.LogInformation($"Cargo arrived.");

                await _publishEndpoint.Publish(new CargoArrivedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
            }
            else
            {
                await _publishEndpoint.Publish(new CargoFailedEvent()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Price = context.Message.TotalPrice,
                    Message = "not enough balance",
                    orderItems = context.Message.orderItems
                });

                _logger.LogInformation($"Fail");
  }
        }
    }
}
