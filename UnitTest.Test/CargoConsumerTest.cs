using System.Collections.Generic;
using System.Threading.Tasks;
using Cargo.API.Consumers;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using Shared;
using Xunit;

namespace UnitTest.Test
{
    public class CargoConsumerTest
    {
        private readonly Mock<ILogger<PaymentCompletedEventConsumer>> _mockLogger;
        private readonly Mock<ISendEndpointProvider> _mockSendEndpointProvider;
        private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
        private readonly PaymentCompletedEventConsumer _consumer;

        public CargoConsumerTest()
        {
            _mockLogger = new Mock<ILogger<PaymentCompletedEventConsumer>>();
            _mockSendEndpointProvider = new Mock<ISendEndpointProvider>();
            _mockPublishEndpoint = new Mock<IPublishEndpoint>();
            _consumer = new PaymentCompletedEventConsumer(
                _mockSendEndpointProvider.Object,
                _mockLogger.Object,
                _mockPublishEndpoint.Object
            );
        }

        [Fact]
        public async Task cargoArrivedTestCase()
        {
            var paymentCompletedEvent = new PaymentCompletedEvent
            {
                BuyerId = "100",
                OrderId = 123,
                Address = "Domestic",  
                TotalPrice = 250,
                orderItems = new List<OrderItemMessage> { new OrderItemMessage { ProductId= 1, Count= 2,Address= "Domestic" } },
            };

            var consumeContext = new Mock<ConsumeContext<PaymentCompletedEvent>>();
            consumeContext.Setup(x => x.Message).Returns(paymentCompletedEvent);

            _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<CargoArrivedEvent>(), It.IsAny<CancellationToken>()))
                                .Returns(Task.CompletedTask);

           
            await _consumer.Consume(consumeContext.Object);

            _mockPublishEndpoint.Verify(x => x.Publish(It.Is<CargoArrivedEvent>(e =>
                e.BuyerId == paymentCompletedEvent.BuyerId &&
                e.OrderId == paymentCompletedEvent.OrderId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Theory]
        [InlineData("100", 123, "Domestic", 250)]
        [InlineData("100", 124, "Domestic", 500)]
        [InlineData("100", 125, "Domestic", 1000)]
        public async Task cargoArrivedTestCaseTheory(string buyerId, int orderId, string address, decimal totalPrice)
        {
            var paymentCompletedEvent = new PaymentCompletedEvent
            {
                BuyerId = buyerId,
                OrderId = orderId,
                Address = address, 
                TotalPrice = totalPrice,
                orderItems = new List<OrderItemMessage> { new OrderItemMessage { ProductId=1, Count=2, Address="Domestic" } },
            };

            var consumeContext = new Mock<ConsumeContext<PaymentCompletedEvent>>();
            consumeContext.Setup(x => x.Message).Returns(paymentCompletedEvent);

            _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<CargoArrivedEvent>(), It.IsAny<System.Threading.CancellationToken>()))
                                .Returns(Task.CompletedTask);

            await _consumer.Consume(consumeContext.Object);

            _mockPublishEndpoint.Verify(x => x.Publish(It.Is<CargoArrivedEvent>(e =>
                e.BuyerId == paymentCompletedEvent.BuyerId &&
                e.OrderId == paymentCompletedEvent.OrderId
            ), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task cargoFailedTestCase()
        {
            var paymentCompletedEvent = new PaymentCompletedEvent
            {
                BuyerId = "100",
                OrderId = 123,
                Address = "International",  
                TotalPrice = 250,
                orderItems = new List<OrderItemMessage> { new OrderItemMessage{ProductId = 1, Count = 2, Address = "Domestic"} },
            };

            var consumeContext = new Mock<ConsumeContext<PaymentCompletedEvent>>();
            consumeContext.Setup(x => x.Message).Returns(paymentCompletedEvent);

            _mockPublishEndpoint.Setup(x => x.Publish(It.IsAny<CargoFailedEvent>(), It.IsAny<CancellationToken>()))
                                .Returns(Task.CompletedTask);

            await _consumer.Consume(consumeContext.Object);

            _mockPublishEndpoint.Verify(x => x.Publish(It.Is<CargoFailedEvent>(e =>
                e.BuyerId == paymentCompletedEvent.BuyerId &&
                e.OrderId == paymentCompletedEvent.OrderId &&
                e.Price == paymentCompletedEvent.TotalPrice &&
                e.Message == "not enough balance" &&  
                e.orderItems == paymentCompletedEvent.orderItems
            ), It.IsAny<CancellationToken>()), Times.Once);

        }

       



    }
}
