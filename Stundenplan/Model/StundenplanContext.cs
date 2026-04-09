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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // --- Keys & Relations Konfiguration ---
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

    // --- DATA SEEDING ---

    // 1. Lehrer (Abbr als String)
    var teachers = new[] { 
        "ALLI", "MACO", "NIGI", "LEYV", "WART", "BRUN", "KUBI", 
        "JAGE", "WIEN", "ELSH", "WINN", "KIEN", "HAUP", "HAUL" 
    };
    modelBuilder.Entity<Teacher>().HasData(
        teachers.Select((abbr, i) => new Teacher { Id = i + 1, Abbr = abbr })
    );

    // 2. Fächer (Desc als String)
    var subjects = new[] { 
        "AM", "D", "E1", "INSY", "RK", "SEW", "SYTD", "GGPH", "GGPG", 
        "NW2P", "NW2C", "DSAI", "WIR", "ITPL", "ITPP", "BESP", "SYTI", "SYTS" 
    };
    modelBuilder.Entity<Subject>().HasData(
        subjects.Select((desc, i) => new Subject { Id = i + 1, Desc = desc })
    );

    // 3. Klassen (Abbr als String)
    var classes = new[] { "1CHIT", "2CHIT", "3CHIT", "4CHIT", "5CHIT" };
    modelBuilder.Entity<SchoolClass>().HasData(
        classes.Select((abbr, i) => new SchoolClass { Id = i + 1, Abbr = abbr })
    );

    // Referenz-ID für 4CHIT (Index 3 im Array -> ID 4)
    int cid = 4;

    // 4. TeacherSubject (Kombinationen aus Lehrer-ID und Fach-ID)
    modelBuilder.Entity<TeacherSubject>().HasData(
        new { Tid = 1,  Sid = 5  }, // ALLI - RK
        new { Tid = 7,  Sid = 13 }, // KUBI - WIR
        new { Tid = 12, Sid = 16 }, // KIEN - BESP
        new { Tid = 5,  Sid = 10 }, // WART - NW2P
        new { Tid = 5,  Sid = 11 }, // WART - NW2C
        new { Tid = 2,  Sid = 6  }, // MACO - SEW
        new { Tid = 2,  Sid = 4  }, // MACO - INSY
        new { Tid = 4,  Sid = 8  }, // LEYV - GGPH
        new { Tid = 4,  Sid = 2  }, // LEYV - D
        new { Tid = 8,  Sid = 14 }, // JAGE - ITPL
        new { Tid = 13, Sid = 17 }, // HAUP - SYTI
        new { Tid = 3,  Sid = 9  }, // NIGI - GGPG
        new { Tid = 9,  Sid = 15 }, // WIEN - ITPP
        new { Tid = 10, Sid = 3  }, // ELSH - E1
        new { Tid = 11, Sid = 7  }, // WINN - SYTD
        new { Tid = 14, Sid = 18 }, // HAUL - SYTS
        new { Tid = 6,  Sid = 12 }, // BRUN - DSAI
        new { Tid = 6,  Sid = 17 }, // BRUN - SYTI
        new { Tid = 3,  Sid = 1  }  // NIGI - AM
    );

    // 5. ClassSubject (Alle Fächer für die 4CHIT freischalten)
    modelBuilder.Entity<ClassSubject>().HasData(
        Enumerable.Range(1, 18).Select(sid => new { Cid = cid, Sid = sid })
    );

    // 6. Lessons (Der eigentliche Stundenplan)
    modelBuilder.Entity<Lesson>().HasData(
        // Montag
        new Lesson { Id = 1, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H1_0745, Tid = 1, Sid = 5 },
        new Lesson { Id = 2, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H2_0835, Tid = 7, Sid = 13 },
        new Lesson { Id = 3, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H3_0940, Tid = 7, Sid = 13 },
        new Lesson { Id = 4, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H4_1030, Tid = 1, Sid = 5 },
        new Lesson { Id = 5, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H5_1125, Tid = 12, Sid = 16 },
        new Lesson { Id = 6, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H7_1305, Tid = 5, Sid = 10 },
        new Lesson { Id = 7, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H8_1355, Tid = 2, Sid = 6 },
        new Lesson { Id = 8, Cid = cid, WeekDay = WeekDay.Mo, Hour = LessonHour.H9_1455, Tid = 2, Sid = 6 },

        // Dienstag
        new Lesson { Id = 9, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H1_0745, Tid = 4, Sid = 8 },
        new Lesson { Id = 10, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H2_0835, Tid = 8, Sid = 14 },
        new Lesson { Id = 11, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H3_0940, Tid = 4, Sid = 2 },
        new Lesson { Id = 12, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H4_1030, Tid = 4, Sid = 2 },
        new Lesson { Id = 13, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H5_1125, Tid = 13, Sid = 17 },
        new Lesson { Id = 14, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H7_1305, Tid = 8, Sid = 14 },
        new Lesson { Id = 15, Cid = cid, WeekDay = WeekDay.Di, Hour = LessonHour.H8_1355, Tid = 8, Sid = 14 },

        // Mittwoch
        new Lesson { Id = 16, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H1_0745, Tid = 3, Sid = 9 },
        new Lesson { Id = 17, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H2_0835, Tid = 9, Sid = 15 },
        new Lesson { Id = 18, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H3_0940, Tid = 10, Sid = 3 },
        new Lesson { Id = 19, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H4_1030, Tid = 11, Sid = 7 },
        new Lesson { Id = 20, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H5_1125, Tid = 11, Sid = 7 },
        new Lesson { Id = 21, Cid = cid, WeekDay = WeekDay.Mi, Hour = LessonHour.H6_1215, Tid = 9, Sid = 15 },

        // Donnerstag
        new Lesson { Id = 22, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H1_0745, Tid = 5, Sid = 11 },
        new Lesson { Id = 23, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H2_0835, Tid = 2, Sid = 4 },
        new Lesson { Id = 24, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H3_0940, Tid = 2, Sid = 4 },
        new Lesson { Id = 25, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H4_1030, Tid = 2, Sid = 4 },
        new Lesson { Id = 26, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H5_1125, Tid = 2, Sid = 4 },
        new Lesson { Id = 27, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H6_1215, Tid = 2, Sid = 6 },
        new Lesson { Id = 28, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H8_1355, Tid = 14, Sid = 18 },
        new Lesson { Id = 29, Cid = cid, WeekDay = WeekDay.Do, Hour = LessonHour.H9_1455, Tid = 14, Sid = 18 },

        // Freitag
        new Lesson { Id = 30, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H1_0745, Tid = 6, Sid = 12 },
        new Lesson { Id = 31, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H2_0835, Tid = 6, Sid = 12 },
        new Lesson { Id = 32, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H3_0940, Tid = 7, Sid = 13 },
        new Lesson { Id = 33, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H4_1030, Tid = 10, Sid = 3 },
        new Lesson { Id = 34, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H5_1125, Tid = 3, Sid = 1 },
        new Lesson { Id = 35, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H6_1215, Tid = 3, Sid = 1 },
        new Lesson { Id = 36, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H8_1355, Tid = 6, Sid = 17 },
        new Lesson { Id = 37, Cid = cid, WeekDay = WeekDay.Fr, Hour = LessonHour.H9_1455, Tid = 6, Sid = 17 }
    );
}
}