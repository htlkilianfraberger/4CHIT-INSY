using Microsoft.EntityFrameworkCore;

namespace Model;

public partial class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public MyDbContext() { }

    public virtual DbSet<Class> Classes { get; set; }
    public virtual DbSet<Pupil> Pupils { get; set; }
    public virtual DbSet<School> Schools { get; set; }
    public virtual DbSet<Department> Departments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Schulverwaltung");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Pupil>()
            .HasOne(p => p.Class)
            .WithMany(c => c.Pupils)
            .HasForeignKey(p => p.ClassId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<School>().HasData(
            new School { Id = 1, Name = "HTL Krems" }
        );
        
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Name = "IT Krems", SchoolId = 1 },
            new Department { Id = 2, Name = "IT Zwettl", SchoolId = 1 }
        );
        
        modelBuilder.Entity<Class>().HasData(
            new Class { Id = 1, Name = "1CHIT" },
            new Class { Id = 2, Name = "2CHIT" },
            new Class { Id = 3, Name = "3CHIT" },
            new Class { Id = 4, Name = "4CHIT" },
            new Class { Id = 5, Name = "5CHIT" }
        );
        
        modelBuilder.Entity<Pupil>().HasData(
            new Pupil { Id = 1, Name = "Lukas Huber", ClassId = 4 },
            new Pupil { Id = 2, Name = "Sarah Maier", ClassId = 4 },
            new Pupil { Id = 3, Name = "Maximilian Gruber", ClassId = 5 },
            new Pupil { Id = 4, Name = "Julia Pichler", ClassId = 5 },
            new Pupil { Id = 5, Name = "Felix Müller", ClassId = 1 },
            new Pupil { Id = 6, Name = "Emma Wagner", ClassId = 1 },
            new Pupil { Id = 7, Name = "Tobias Berger", ClassId = 2 },
            new Pupil { Id = 8, Name = "Anna Fuchs", ClassId = 2 },
            new Pupil { Id = 9, Name = "Moritz Hofer", ClassId = 3 },
            new Pupil { Id = 10, Name = "Elena Steiner", ClassId = 3 }
        );
    }
}