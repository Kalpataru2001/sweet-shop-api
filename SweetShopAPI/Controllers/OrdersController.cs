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


        [HttpPost]
        public async Task<ActionResult<Order>> PlaceOrder(Order order)
        {
            try
            {
                // 1. Create a clean Order object
                var newOrder = new Order
                {
                    CustomerName = order.CustomerName,
                    CustomerPhone = order.CustomerPhone,
                    CustomerAddress = order.CustomerAddress,
                    TotalAmount = order.TotalAmount,
                    OrderDate = DateTime.UtcNow,
                    OrderItems = new List<OrderItem>()
                };

                // 2. Map Items safely
                if (order.OrderItems != null)
                {
                    foreach (var item in order.OrderItems)
                    {
                        // VALIDATION: Check if Sweet exists before adding!
                        var sweetExists = await _context.Sweets.AnyAsync(s => s.Id == item.SweetId);
                        if (!sweetExists)
                        {
                            return BadRequest($"❌ Error: Sweet with ID {item.SweetId} does not exist in the database!");
                        }

                        newOrder.OrderItems.Add(new OrderItem
                        {
                            SweetId = item.SweetId,
                            Quantity = item.Quantity,
                            Price = item.Price
                        });
                    }
                }

                // 3. Save
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                // THIS IS THE FIX: Return the REAL error to Swagger
                var innerMessage = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, $"Database Error: {innerMessage}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            // We use .Include to fetch the "OrderItems" and the "Sweet" details inside them
            return await _context.Orders
                                 .Include(o => o.OrderItems)
                                 .ThenInclude(oi => oi.Sweet) // So we get the Sweet Name
                                 .OrderByDescending(o => o.OrderDate)
                                 .ToListAsync();
        }
    }
}
