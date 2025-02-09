using MassTransit.Transports;
using Order.API.Models;

namespace Order.API.Repositories
{
    public interface IOrderRepository
    {
        Task CreateAsync(Models.Order order);

        Task<List<Models.Order>> GetAllAsync();
    }

}
