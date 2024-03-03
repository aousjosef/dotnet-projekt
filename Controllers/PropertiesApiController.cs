using Fastigheterse.Data;
using Fastigheterse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fastigheterse.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PropertiesApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/PropertiesApi
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperties()
        {
            return await _context.Properties
                                 .Include(p => p.Images) // This line includes the Images.
                                 .ToListAsync();
        }

        // GET: api/PropertiesApi/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Property>> GetProperty(int id)
        {
            var property = await _context.Properties
                                    .Include(p => p.Images) // This line includes the Images.
                                    .FirstOrDefaultAsync(p => p.Id == id);

            if (property == null)
            {
                return NotFound();
            }

            return property;
        }


        private bool PropertyExists(int id)
        {
            return _context.Properties.Any(e => e.Id == id);
        }
    }
}
