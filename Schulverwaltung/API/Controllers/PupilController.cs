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
    public class PupilController : ControllerBase
    {
        private readonly MyDbContext _context;

        public PupilController(MyDbContext context)
        {
            _context = context;
        }

        // GET: api/Pupil
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pupil>>> GetPupils()
        {
            return await _context.Pupils.ToListAsync();
        }

        // GET: api/Pupil/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pupil>> GetPupil(int id)
        {
            var pupil = await _context.Pupils.FindAsync(id);

            if (pupil == null)
            {
                return NotFound();
            }

            return pupil;
        }

        // PUT: api/Pupil/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPupil(int id, Pupil pupil)
        {
            if (id != pupil.Id)
            {
                return BadRequest();
            }

            _context.Entry(pupil).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PupilExists(id))
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

        // POST: api/Pupil
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pupil>> PostPupil(Pupil pupil)
        {
            _context.Pupils.Add(pupil);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPupil", new { id = pupil.Id }, pupil);
        }

        // DELETE: api/Pupil/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePupil(int id)
        {
            var pupil = await _context.Pupils.FindAsync(id);
            if (pupil == null)
            {
                return NotFound();
            }

            _context.Pupils.Remove(pupil);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PupilExists(int id)
        {
            return _context.Pupils.Any(e => e.Id == id);
        }
    }
}
