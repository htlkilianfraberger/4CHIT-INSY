using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace WebAPISwagger.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DemoController : ControllerBase
    {
        private readonly SwaggerContext _context;

        public DemoController(SwaggerContext context)
        {
            _context = context;
        }

        // GET: api/Demo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Demos>>> GetDemos()
        {
            return await _context.Demos.ToListAsync();
        }

        // GET: api/Demo/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Demos>> GetDemo(int id)
        {
            var demo = await _context.Demos.FindAsync(id);

            if (demo == null)
            {
                return NotFound();
            }

            return demo;
        }

        // PUT: api/Demo/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDemo(int id, Demos demos)
        {
            if (id != demos.Id)
            {
                return BadRequest();
            }

            _context.Entry(demos).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DemoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Demo
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Demos>> PostDemo(Demos demos)
        {
            _context.Demos.Add(demos);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDemo", new { id = demos.Id }, demos);
        }

        // DELETE: api/Demo/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDemo(int id)
        {
            var demo = await _context.Demos.FindAsync(id);
            if (demo == null)
            {
                return NotFound();
            }

            _context.Demos.Remove(demo);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DemoExists(int id)
        {
            return _context.Demos.Any(e => e.Id == id);
        }
    }
}
