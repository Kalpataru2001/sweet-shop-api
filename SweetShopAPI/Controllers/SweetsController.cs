using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SweetShopAPI.Data;
using SweetShopAPI.Models;

namespace SweetShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SweetsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SweetsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/sweets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sweet>>> GetSweets()
        {
            // This goes to Supabase, grabs all sweets, and returns them as JSON
            return await _context.Sweets.ToListAsync();
        }
    }
}
