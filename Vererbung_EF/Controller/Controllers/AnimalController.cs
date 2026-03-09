using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Context;

namespace YourProject.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnimalsController : ControllerBase
{
    private readonly MyDbContext _context;

    public AnimalsController(MyDbContext context)
    {
        _context = context;
    }

    // GET: api/Animals
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
    {
        return await _context.Animals.ToListAsync();
    }

    // GET: api/Animals/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Animal>> GetAnimal(int id)
    {
        var animal = await _context.Animals.FindAsync(id);

        if (animal == null)
        {
            return NotFound();
        }

        return animal;
    }
    
}