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

        // GET: api/Lesson
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lesson>>> GetLessons()
        {
            return await _context.Lessons.ToListAsync();
        }

        // GET: api/Lesson/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lesson>> GetLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            return lesson == null ? NotFound() : lesson;
        }

        // PUT: api/Lesson/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLesson(int id, Lesson lesson)
        {
            if (id != lesson.Id) return BadRequest();

            var teacherCanTeachSubject = await _context.TeacherSubjects
                .AnyAsync(ts => ts.Tid == lesson.Tid && ts.Sid == lesson.Sid);

            if (!teacherCanTeachSubject)
            {
                throw new Exception("Dieser Lehrer unterrichtet dieses Fach nicht!");
            }
            
            var classHasSubject = await _context.ClassSubjects
                .AnyAsync(cs => cs.Cid == lesson.Cid && cs.Sid == lesson.Sid);

            if (!classHasSubject)
            {
                throw new Exception("Dieses Fach ist für die gewählte Klasse nicht vorgesehen!");
            }
            
            var isSlotTaken = await _context.Lessons
                .AnyAsync(l => l.Cid == lesson.Cid && l.WeekDay == lesson.WeekDay && l.Hour == lesson.Hour);

            if (isSlotTaken)
            {
                throw new Exception($"Die Klasse hat am {lesson.WeekDay} in der {lesson.Hour} bereits Unterricht!");
            }

            _context.Entry(lesson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LessonExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // POST: api/Lesson
        [HttpPost]
        public async Task<ActionResult<Lesson>> PostLesson(Lesson lesson)
        {
            var teacherCanTeachSubject = await _context.TeacherSubjects
                .AnyAsync(ts => ts.Tid == lesson.Tid && ts.Sid == lesson.Sid);

            if (!teacherCanTeachSubject)
            {
                throw new Exception("Dieser Lehrer unterrichtet dieses Fach nicht!");
            }
            
            var classHasSubject = await _context.ClassSubjects
                .AnyAsync(cs => cs.Cid == lesson.Cid && cs.Sid == lesson.Sid);

            if (!classHasSubject)
            {
                throw new Exception("Dieses Fach ist für die gewählte Klasse nicht vorgesehen!");
            }
            
            var isSlotTaken = await _context.Lessons
                .AnyAsync(l => l.Cid == lesson.Cid && l.WeekDay == lesson.WeekDay && l.Hour == lesson.Hour);

            if (isSlotTaken)
            {
                throw new Exception($"Die Klasse hat am {lesson.WeekDay} in der {lesson.Hour} bereits Unterricht!");
            }

            _context.Lessons.Add(lesson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLesson", new { id = lesson.Id }, lesson);
        }

        // DELETE: api/Lesson/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLesson(int id)
        {
            var lesson = await _context.Lessons.FindAsync(id);
            if (lesson == null) return NotFound();

            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LessonExists(int id)
        {
            return _context.Lessons.Any(e => e.Id == id);
        }
    }
}