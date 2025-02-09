using AutoMapper;
using Order.API.Models;
using Order.API.Repositories;
using Order.API.DTOs;

namespace Order.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public OrderService(IOrderRepository orderRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
        }

        public async Task<List<OrderCreateDto>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllAsync();
            var orderDtos = _mapper.Map<List<OrderCreateDto>>(orders); 
            return orderDtos;
        }

        public async Task CreateOrderAsync(OrderCreateDto orderCreateDto)
        {
            var order = _mapper.Map<Models.Order>(orderCreateDto); 
            await _orderRepository.CreateAsync(order);
        }
    }
}
