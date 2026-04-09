using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassesController : ControllerBase
    {
        private readonly StundenplanContext _context;

        public ClassesController(StundenplanContext context)
        {
            _context = context;
        }

        // GET: api/Classes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SchoolClass>>> GetClasses()
        {
            return await _context.Classes.ToListAsync();
        }

        // GET: api/Classes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SchoolClass>> GetSchoolClass(int id)
        {
            var schoolClass = await _context.Classes.FindAsync(id);

            if (schoolClass == null)
            {
                return NotFound();
            }

            return schoolClass;
        }

        // PUT: api/Classes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSchoolClass(int id, SchoolClass schoolClass)
        {
            if (id != schoolClass.Id)
            {
                return BadRequest();
            }

            _context.Entry(schoolClass).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SchoolClassExists(id))
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

        // POST: api/Classes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SchoolClass>> PostSchoolClass(SchoolClass schoolClass)
        {
            _context.Classes.Add(schoolClass);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSchoolClass", new { id = schoolClass.Id }, schoolClass);
        }

        // DELETE: api/Classes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchoolClass(int id)
        {
            var schoolClass = await _context.Classes.FindAsync(id);
            if (schoolClass == null)
            {
                return NotFound();
            }

            _context.Classes.Remove(schoolClass);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SchoolClassExists(int id)
        {
            return _context.Classes.Any(e => e.Id == id);
        }
    }
}
