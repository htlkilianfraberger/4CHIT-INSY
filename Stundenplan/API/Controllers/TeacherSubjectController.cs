using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace API.Controllers;

public record TeacherSubjectDto(int Tid, int Sid);

[Route("api/[controller]")]
[ApiController]
public class TeacherSubjectController : ControllerBase
{
    private readonly StundenplanContext _context;

    public TeacherSubjectController(StundenplanContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TeacherSubjectDto>>> GetTeacherSubjects()
    {
        return await _context.TeacherSubjects
            .Select(ts => new TeacherSubjectDto(ts.Tid, ts.Sid))
            .ToListAsync();
    }

    [HttpPost]
    public async Task<IActionResult> PostTeacherSubject(TeacherSubjectDto dto)
    {
        if (_context.TeacherSubjects.Any(ts => ts.Tid == dto.Tid && ts.Sid == dto.Sid))
            return Conflict("Diese Zuordnung existiert bereits.");

        _context.TeacherSubjects.Add(new TeacherSubject { Tid = dto.Tid, Sid = dto.Sid });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteTeacherSubject(int tid, int sid)
    {
        var ts = await _context.TeacherSubjects
            .FirstOrDefaultAsync(x => x.Tid == tid && x.Sid == sid);
        if (ts == null) return NotFound();

        _context.TeacherSubjects.Remove(ts);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}