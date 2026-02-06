using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetShopAPI.Data;
using SweetShopAPI.Models;

namespace SweetShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder(Order order)
        {
            // 1. Set the order date to now
            order.OrderDate = DateTime.UtcNow;

            // 2. Add to database
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 3. Return success
            return CreatedAtAction(nameof(PlaceOrder), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // Return all orders, newest first
            return await _context.Orders
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }
    }
}
