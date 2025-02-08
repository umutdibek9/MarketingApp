using MassTransit;
using Shared;

namespace Payment.API.Consumers
{
    public class CargoArrivedEventConsumer : IConsumer<CargoArrivedEvent>
    {
        private readonly ILogger<CargoArrivedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        private readonly IPublishEndpoint _publishEndpoint;

        public CargoArrivedEventConsumer(ILogger<CargoArrivedEventConsumer> logger, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<CargoArrivedEvent> context)
        {
            await _publishEndpoint.Publish(new CargoArrivedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId });
        }
    }
    }
