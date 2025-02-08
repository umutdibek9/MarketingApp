using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payment.API.Models;

namespace Payment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var payment = await _context.Payment.ToListAsync();
            return Ok(payment);
        }
    }
}
