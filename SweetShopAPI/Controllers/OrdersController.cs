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
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Create a clean Order object (Ignore frontend total/status)
                var newOrder = new Order
                {
                    CustomerName = order.CustomerName,
                    CustomerPhone = order.CustomerPhone,
                    CustomerAddress = order.CustomerAddress,
                    OrderDate = DateTime.UtcNow,
                    Status = "Pending",
                    OrderItems = new List<OrderItem>(),
                    TotalAmount = 0 // We will calculate this ourselves!
                };

                // 2. Loop through items to validate Stock & Price
                if (order.OrderItems != null && order.OrderItems.Any())
                {
                    foreach (var item in order.OrderItems)
                    {
                        // FETCH REAL DATA FROM DB
                        var sweet = await _context.Sweets.FindAsync(item.SweetId);

                        if (sweet == null)
                        {
                            return BadRequest($"❌ Error: Sweet with ID {item.SweetId} does not exist!");
                        }

                        // STOCK CHECK
                        if (sweet.StockQuantity < item.Quantity)
                        {
                            return BadRequest($"❌ Error: Not enough stock for {sweet.Name}. Only {sweet.StockQuantity} left.");
                        }

                        // DEDUCT STOCK
                        sweet.StockQuantity -= item.Quantity;

                        // ADD ITEM WITH REAL PRICE
                        var orderItem = new OrderItem
                        {
                            SweetId = item.SweetId,
                            Quantity = item.Quantity,
                            Price = sweet.Price // <--- SECURITY FIX: Using DB Price, not Frontend Price
                        };

                        newOrder.OrderItems.Add(orderItem);

                        // UPDATE TOTAL
                        newOrder.TotalAmount += (sweet.Price * item.Quantity);
                    }
                }

                // 3. Save Everything
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync(); // Saves Order, Items, and updates Stock
                await transaction.CommitAsync();

                return CreatedAtAction(nameof(GetOrders), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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

        // PUT: api/orders/5/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // validate status
            var validStatuses = new[] { "Pending", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(newStatus))
            {
                return BadRequest("Invalid Status");
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
