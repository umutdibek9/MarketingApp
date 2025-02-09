using Order.API.DTOs;

namespace Order.API.Services
{
    public interface IOrderService
    {
        Task<List<OrderCreateDto>> GetAllOrdersAsync();
        Task CreateOrderAsync(OrderCreateDto orderCreateDto);
    }
}
