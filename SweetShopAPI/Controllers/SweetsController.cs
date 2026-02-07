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

        [HttpGet("{id}")]
        public async Task<ActionResult<Sweet>> GetSweet(int id)
        {
            var sweet = await _context.Sweets.FindAsync(id);
            if (sweet == null) return NotFound();
            return sweet;
        }

        // 3. POST: api/sweets (Add New Sweet)
        [HttpPost]
        public async Task<ActionResult<Sweet>> PostSweet(Sweet sweet)
        {
            _context.Sweets.Add(sweet);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetSweet), new { id = sweet.Id }, sweet);
        }

        // 4. PUT: api/sweets/5 (Update/Edit Sweet)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSweet(int id, Sweet sweet)
        {
            if (id != sweet.Id) return BadRequest();

            _context.Entry(sweet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Sweets.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. DELETE: api/sweets/5 (Delete Sweet)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSweet(int id)
        {
            var sweet = await _context.Sweets.FindAsync(id);
            if (sweet == null) return NotFound();

            _context.Sweets.Remove(sweet);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
