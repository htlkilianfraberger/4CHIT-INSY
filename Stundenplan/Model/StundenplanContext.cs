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
        modelBuilder.Entity<TeacherSubject>()
            .HasKey(ts => new { ts.Tid, ts.Sid });

        modelBuilder.Entity<ClassSubject>()
            .HasKey(cs => new { cs.Cid, cs.Sid });
        
        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.TeacherSubject)
            .WithMany()
            .HasForeignKey(l => new { l.Tid, l.Sid });

        modelBuilder.Entity<Lesson>()
            .HasOne(l => l.ClassSubject)
            .WithMany()
            .HasForeignKey(l => new { l.Cid, l.Sid });
        
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
        
        
        modelBuilder.Entity<Teacher>().HasData(
            new Teacher { Id = 1, Abbr = "ALLI" },
            new Teacher { Id = 2, Abbr = "BIRN" },
            new Teacher { Id = 13, Abbr = "MACO" },
            new Teacher { Id = 14, Abbr = "NIGI" },
            new Teacher { Id = 19, Abbr = "STRO" }
        );
                
        modelBuilder.Entity<Subject>().HasData(
            new Subject { Id = 1, Desc = "AM" },
            new Subject { Id = 4, Desc = "D" },
            new Subject { Id = 5, Desc = "E" },
            new Subject { Id = 9, Desc = "INSY" },
            new Subject { Id = 18, Desc = "REL" },
            new Subject { Id = 19, Desc = "SEW" },
            new Subject { Id = 20, Desc = "SOPK" },
            new Subject { Id = 21, Desc = "SYT" }
        );

        modelBuilder.Entity<SchoolClass>().HasData(
            new SchoolClass { Id = 1, Abbr = "1CHIT" },
            new SchoolClass { Id = 2, Abbr = "2CHIT" },
            new SchoolClass { Id = 3, Abbr = "3CHIT" },
            new SchoolClass { Id = 4, Abbr = "4CHIT" },
            new SchoolClass { Id = 5, Abbr = "5CHIT" }
        );

        modelBuilder.Entity<TeacherSubject>().HasData(
            new { Tid = 1, Sid = 18 },
            new { Tid = 13, Sid = 9 },
            new { Tid = 13, Sid = 19 },
            new { Tid = 2, Sid = 19 }
        );

        modelBuilder.Entity<ClassSubject>().HasData(
            new { Cid = 1, Sid = 18 },
            new { Cid = 4, Sid = 18 },
            new { Cid = 4, Sid = 9 },
            new { Cid = 4, Sid = 19 },
            new { Cid = 5, Sid = 19 }
        );

        modelBuilder.Entity<Lesson>().HasData(
            new Lesson { Id = 203, Tid = 1, Sid = 18, Cid = 1, WeekDay = WeekDay.Mo, Hour = 1 },
            new Lesson { Id = 299, Tid = 1, Sid = 18, Cid = 4, WeekDay = WeekDay.Di, Hour = 2 },
            new Lesson { Id = 378, Tid = 13, Sid = 9, Cid = 4, WeekDay = WeekDay.Mi, Hour = 3 },
            new Lesson { Id = 379, Tid = 13, Sid = 19, Cid = 4, WeekDay = WeekDay.Do, Hour = 4 },
            new Lesson { Id = 399, Tid = 13, Sid = 19, Cid = 5, WeekDay = WeekDay.Fr, Hour = 5 },
            new Lesson { Id = 1277, Tid = 2, Sid = 19, Cid = 5, WeekDay = WeekDay.Mo, Hour = 6 }
        );
    }
}