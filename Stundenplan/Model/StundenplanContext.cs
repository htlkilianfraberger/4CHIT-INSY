using Microsoft.EntityFrameworkCore;

namespace Model;

public class StundenplanContext : DbContext {
    public StundenplanContext() { }
    public StundenplanContext(DbContextOptions<StundenplanContext> options) : base(options) { }

    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SchoolClass> Classes => Set<SchoolClass>();
    public DbSet<TeacherSubject> TeacherSubjects => Set<TeacherSubject>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<Lesson> Lessons => Set<Lesson>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Stundenplan");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        // --- Keys & Relations ---
        modelBuilder.Entity<TeacherSubject>().HasKey(ts => new { ts.Tid, ts.Sid });
        modelBuilder.Entity<ClassSubject>().HasKey(cs => new { cs.Cid, cs.Sid });
        
        modelBuilder.Entity<TeacherSubject>(e => {
            e.HasOne(ts => ts.Teacher).WithMany(t => t.TeacherSubjects).HasForeignKey(ts => ts.Tid);
            e.HasOne(ts => ts.Subject).WithMany().HasForeignKey(ts => ts.Sid);
        });

        modelBuilder.Entity<ClassSubject>(e => {
            e.HasOne(cs => cs.SchoolClass).WithMany(c => c.ClassSubjects).HasForeignKey(cs => cs.Cid);
            e.HasOne(cs => cs.Subject).WithMany().HasForeignKey(cs => cs.Sid);
        });
        
        modelBuilder.Entity<Lesson>(e => {
            e.HasOne(l => l.TeacherSubject).WithMany().HasForeignKey(l => new { l.Tid, l.Sid }).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(l => l.ClassSubject).WithMany().HasForeignKey(l => new { l.Cid, l.Sid }).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Data Seeding ---

        // 1. Stammdaten (IDs 1-n)
        modelBuilder.Entity<Teacher>().HasData(Enum.GetValues<TeacherEnum>().Select((e, i) => new Teacher { Id = i + 1, Abbr = e }));
        modelBuilder.Entity<Subject>().HasData(Enum.GetValues<SubjectEnum>().Select((e, i) => new Subject { Id = i + 1, Desc = e }));
        modelBuilder.Entity<SchoolClass>().HasData(Enum.GetValues<SchoolClassEnum>().Select((e, i) => new SchoolClass { Id = i + 1, Abbr = e }));

        // Deine Klasse 4CHIT
        int cid = (int)SchoolClassEnum.CHIT4; 

        // 2. TeacherSubject (Alle Paare, die im 4CHIT Plan vorkommen)
        modelBuilder.Entity<TeacherSubject>().HasData(
            new { Tid = (int)TeacherEnum.ALLI + 1, Sid = (int)SubjectEnum.RK + 1 },
            new { Tid = (int)TeacherEnum.KUBI + 1, Sid = (int)SubjectEnum.WIR + 1 },
            new { Tid = (int)TeacherEnum.KIEN + 1, Sid = (int)SubjectEnum.BESP + 1 },
            new { Tid = (int)TeacherEnum.WART + 1, Sid = (int)SubjectEnum.NW2 + 1 },
            new { Tid = (int)TeacherEnum.MACO + 1, Sid = (int)SubjectEnum.SEW + 1 },
            new { Tid = (int)TeacherEnum.MACO + 1, Sid = (int)SubjectEnum.INSY + 1 },
            new { Tid = (int)TeacherEnum.LEYV + 1, Sid = (int)SubjectEnum.GGPH + 1 },
            new { Tid = (int)TeacherEnum.LEYV + 1, Sid = (int)SubjectEnum.D + 1 },
            new { Tid = (int)TeacherEnum.JAGE + 1, Sid = (int)SubjectEnum.ITPL + 1 },
            new { Tid = (int)TeacherEnum.HAUP + 1, Sid = (int)SubjectEnum.SYTI + 1 },
            new { Tid = (int)TeacherEnum.NIGI + 1, Sid = (int)SubjectEnum.GGPH + 1 },
            new { Tid = (int)TeacherEnum.WIEN + 1, Sid = (int)SubjectEnum.ITPP + 1 },
            new { Tid = (int)TeacherEnum.ELSH + 1, Sid = (int)SubjectEnum.E1 + 1 },
            new { Tid = (int)TeacherEnum.WINN + 1, Sid = (int)SubjectEnum.SYTD + 1 },
            new { Tid = (int)TeacherEnum.HAUL + 1, Sid = (int)SubjectEnum.SYTS + 1 },
            new { Tid = (int)TeacherEnum.BRUN + 1, Sid = (int)SubjectEnum.DSAI + 1 },
            new { Tid = (int)TeacherEnum.BRUN + 1, Sid = (int)SubjectEnum.SYTI + 1 },
            new { Tid = (int)TeacherEnum.NIGI + 1, Sid = (int)SubjectEnum.AM + 1 }
        );

        // 3. ClassSubject (Alle Fächer der 4CHIT zuordnen)
        modelBuilder.Entity<ClassSubject>().HasData(
            Enum.GetValues<SubjectEnum>().Select(s => new { Cid = cid, Sid = (int)s + 1 })
        );

        // 4. Lessons (Einzelstunden laut Plan)
        modelBuilder.Entity<Lesson>().HasData(
            // Montag
            new Lesson { Id = 1, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H1_0745, Tid = (int)TeacherEnum.ALLI+1, Sid = (int)SubjectEnum.RK+1 },
            new Lesson { Id = 2, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H2_0835, Tid = (int)TeacherEnum.KUBI+1, Sid = (int)SubjectEnum.WIR+1 },
            new Lesson { Id = 3, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H3_0940, Tid = (int)TeacherEnum.KUBI+1, Sid = (int)SubjectEnum.WIR+1 },
            new Lesson { Id = 4, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H4_1030, Tid = (int)TeacherEnum.ALLI+1, Sid = (int)SubjectEnum.RK+1 },
            new Lesson { Id = 5, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H5_1125, Tid = (int)TeacherEnum.KIEN+1, Sid = (int)SubjectEnum.BESP+1 },
            new Lesson { Id = 6, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H7_1305, Tid = (int)TeacherEnum.WART+1, Sid = (int)SubjectEnum.NW2+1 },
            new Lesson { Id = 7, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H8_1355, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.SEW+1 },
            new Lesson { Id = 8, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H9_1455, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.SEW+1 },

            // Dienstag
            new Lesson { Id = 9, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H1_0745, Tid = (int)TeacherEnum.LEYV+1, Sid = (int)SubjectEnum.GGPH+1 },
            new Lesson { Id = 10, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H2_0835, Tid = (int)TeacherEnum.JAGE+1, Sid = (int)SubjectEnum.ITPL+1 },
            new Lesson { Id = 11, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H3_0940, Tid = (int)TeacherEnum.LEYV+1, Sid = (int)SubjectEnum.D+1 },
            new Lesson { Id = 12, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H4_1030, Tid = (int)TeacherEnum.LEYV+1, Sid = (int)SubjectEnum.D+1 },
            new Lesson { Id = 13, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H5_1125, Tid = (int)TeacherEnum.HAUP+1, Sid = (int)SubjectEnum.SYTI+1 },
            new Lesson { Id = 14, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H7_1305, Tid = (int)TeacherEnum.JAGE+1, Sid = (int)SubjectEnum.ITPL+1 },
            new Lesson { Id = 15, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H8_1355, Tid = (int)TeacherEnum.JAGE+1, Sid = (int)SubjectEnum.ITPL+1 },

            // Mittwoch
            new Lesson { Id = 16, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H1_0745, Tid = (int)TeacherEnum.NIGI+1, Sid = (int)SubjectEnum.GGPH+1 },
            new Lesson { Id = 17, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H2_0835, Tid = (int)TeacherEnum.WIEN+1, Sid = (int)SubjectEnum.ITPP+1 },
            new Lesson { Id = 18, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H3_0940, Tid = (int)TeacherEnum.ELSH+1, Sid = (int)SubjectEnum.E1+1 },
            new Lesson { Id = 19, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H4_1030, Tid = (int)TeacherEnum.WINN+1, Sid = (int)SubjectEnum.SYTD+1 },
            new Lesson { Id = 20, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H5_1125, Tid = (int)TeacherEnum.WINN+1, Sid = (int)SubjectEnum.SYTD+1 },
            new Lesson { Id = 21, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H6_1215, Tid = (int)TeacherEnum.WIEN+1, Sid = (int)SubjectEnum.ITPP+1 },

            // Donnerstag
            new Lesson { Id = 22, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H1_0745, Tid = (int)TeacherEnum.WART+1, Sid = (int)SubjectEnum.NW2+1 },
            new Lesson { Id = 23, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H2_0835, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.INSY+1 },
            new Lesson { Id = 24, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H3_0940, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.INSY+1 }, 
            new Lesson { Id = 25, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H4_1030, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.INSY+1 },
            new Lesson { Id = 26, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H5_1125, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.INSY+1 },
            new Lesson { Id = 27, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H6_1215, Tid = (int)TeacherEnum.MACO+1, Sid = (int)SubjectEnum.SEW+1 },
            new Lesson { Id = 28, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H8_1355, Tid = (int)TeacherEnum.HAUL+1, Sid = (int)SubjectEnum.SYTS+1 },
            new Lesson { Id = 29, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H9_1455, Tid = (int)TeacherEnum.HAUL+1, Sid = (int)SubjectEnum.SYTS+1 },

            // Freitag
            new Lesson { Id = 30, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H1_0745, Tid = (int)TeacherEnum.BRUN+1, Sid = (int)SubjectEnum.DSAI+1 },
            new Lesson { Id = 31, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H2_0835, Tid = (int)TeacherEnum.BRUN+1, Sid = (int)SubjectEnum.DSAI+1 },
            new Lesson { Id = 32, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H3_0940, Tid = (int)TeacherEnum.KUBI+1, Sid = (int)SubjectEnum.WIR+1 },
            new Lesson { Id = 33, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H4_1030, Tid = (int)TeacherEnum.KUBI+1, Sid = (int)SubjectEnum.WIR+1 },
            new Lesson { Id = 34, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H5_1125, Tid = (int)TeacherEnum.NIGI+1, Sid = (int)SubjectEnum.AM+1 },
            new Lesson { Id = 35, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H6_1215, Tid = (int)TeacherEnum.NIGI+1, Sid = (int)SubjectEnum.AM+1 },
            new Lesson { Id = 36, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H8_1355, Tid = (int)TeacherEnum.BRUN+1, Sid = (int)SubjectEnum.SYTI+1 },
            new Lesson { Id = 37, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H9_1455, Tid = (int)TeacherEnum.BRUN+1, Sid = (int)SubjectEnum.SYTI+1 }
        );
    }
}