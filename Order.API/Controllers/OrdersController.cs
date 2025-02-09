using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.DTOs;
using Order.API.Models;
using Order.API.Repositories;
using Shared;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        public OrdersController(AppDbContext context,IPublishEndpoint publishEndpoint, IOrderRepository orderRepository, IMapper mapper)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _orderRepository = orderRepository;
            _mapper = mapper;
        }
        [HttpPost]
        public async Task<IActionResult> Create(OrderCreateDto orderCreate)
        {
            var newOrder = new Models.Order
            {
                BuyerId = orderCreate.BuyerId,
                OrderStatus = OrderStatus.Suspend,
                Address = new Address { Line = orderCreate.Address.Line, City = orderCreate.Address.Region },
                CreatedDate = DateTime.Now,
                FailureMesage= orderCreate.FailureMesage
            };
            orderCreate.orderItems.ForEach(item =>
            {
                newOrder.Items.Add(new OrderItem() { Price = item.Price, ProductId = item.ProductId, Count = item.Count });
            });

            await _orderRepository.CreateAsync(newOrder);

            var orderCreatedEvent = new OrderCreatedEvent()
            {
                Address =orderCreate.Address.Region,
                BuyerId = orderCreate.BuyerId,
                OrderId = newOrder.Id,
                Payment = new PaymentMessage
                {
                    CardName = orderCreate.Payment.CardName,
                    CardNumber = orderCreate.Payment.CardNumber,
                    CVV = orderCreate.Payment.CVV,
                    TotalPrice = orderCreate.orderItems.Sum(x => x.Price * x.Count),
                },
            };

            orderCreate.orderItems.ForEach(item =>
            {
                orderCreatedEvent.orderItems.Add(new OrderItemMessage
                {
                    Count = item.Count,
                    ProductId = item.ProductId,
                });
            });

            await _publishEndpoint.Publish(orderCreatedEvent);
            return Ok();
        }

    }
}
