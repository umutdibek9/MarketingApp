using Microsoft.EntityFrameworkCore;
using Order.API.Models;

namespace Order.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly Models.AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Models.Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }
    }
}
