using MassTransit;
using MassTransit.Transports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Payment.API.Models;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(AppDbContext context,ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
        {
            _logger = logger;
            _context = context;
            _publishEndpoint = publishEndpoint;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var payment = await _context.Payment.Where(x => x.id == Convert.ToInt32(context.Message.BuyerId)).FirstOrDefaultAsync();

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettingsConst.PaymentStockReservedEventQueueName}"));
          
            if (payment != null && payment.Balance > context.Message.Payment.TotalPrice)
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from credit card for user id= {context.Message.BuyerId}");
                payment.Balance -= context.Message.Payment.TotalPrice;
                await _context.SaveChangesAsync();
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent()
                {
                    Address = context.Message.Address,
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.Payment.TotalPrice,
                    orderItems = context.Message.OrderItems
                };
                await sendEndpoint.Send(paymentCompletedEvent);

            }
            else
            {
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from credit card for user id={context.Message.BuyerId}");

                await _publishEndpoint.Publish(new PaymentFailedEvent { BuyerId = context.Message.BuyerId, OrderId = context.Message.OrderId, Message = "not enough balance", orderItems = context.Message.OrderItems });
            }
        }
    }
}