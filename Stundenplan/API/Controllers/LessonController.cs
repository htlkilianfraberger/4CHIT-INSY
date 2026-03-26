using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly StundenplanContext _context;

        public LessonController(StundenplanContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
            => await _context.Lessons.ToListAsync();

        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            return lesson == null ? NotFound() : lesson;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutLesson(int id, Lesson lesson)
        {
            if (id != lesson.Id) return BadRequest();

            var validationError = await ValidateLesson(lesson, excludeId: id);
            if (validationError != null) return BadRequest(validationError);

            _context.Entry(lesson).State = EntityState.Modified;

            try { await _context.SaveChangesAsync(); }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            var validationError = await ValidateLesson(lesson, excludeId: null);
            if (validationError != null) return BadRequest(validationError);

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLesson", new { id = lesson.Id }, lesson);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private async Task<string?> ValidateLesson(Lesson lesson, int? excludeId)
        {
            var teacherCanTeach = await _context.TeacherSubjects
                .AnyAsync(ts => ts.Tid == lesson.Tid && ts.Sid == lesson.Sid);
            if (!teacherCanTeach)
                return "Dieser Lehrer unterrichtet dieses Fach nicht!";

            var classHasSubject = await _context.ClassSubjects
                .AnyAsync(cs => cs.Cid == lesson.Cid && cs.Sid == lesson.Sid);
            if (!classHasSubject)
                return "Dieses Fach ist für die gewählte Klasse nicht vorgesehen!";

            var isSlotTaken = await _context.Lessons
                .AnyAsync(l => l.Cid == lesson.Cid
                            && l.WeekDay == lesson.WeekDay
                            && l.Hour == lesson.Hour
                            && (excludeId == null || l.Id != excludeId));
            if (isSlotTaken)
                return $"Die Klasse hat am {lesson.WeekDay} in der {lesson.Hour}. Stunde bereits Unterricht!";

            return null;
        }

        private bool LessonExists(int id)
            => _context.Lessons.Any(e => e.Id == id);
    }
}