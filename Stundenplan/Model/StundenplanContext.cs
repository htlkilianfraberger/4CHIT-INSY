using Microsoft.EntityFrameworkCore;
using Model;

namespace Model;

public partial class StundenplanContext(DbContextOptions<StundenplanContext> options) : DbContext(options)
{
    public StundenplanContext() : this(new DbContextOptions<StundenplanContext>()) { }

    public virtual DbSet<Classes> Classes { get; set; }
    public virtual DbSet<Subjects> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Stundenplan");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Classes>()
            .HasMany(c => c.S)
            .WithMany(s => s.C)
            .UsingEntity<Dictionary<string, object>>(
                "ClassesSubjects",
                j => j.HasOne<Subjects>().WithMany().HasForeignKey("SId"),
                j => j.HasOne<Classes>().WithMany().HasForeignKey("CId")
            );
        
        modelBuilder.Entity<Classes>().HasData(
            new Classes { Id = 1, Description = "1CHIT" },
            new Classes { Id = 2, Description = "2CHIT" },
            new Classes { Id = 3, Description = "3CHIT" },
            new Classes { Id = 4, Description = "4CHIT" },
            new Classes { Id = 5, Description = "5CHIT" }
        );
        
        modelBuilder.Entity<Subjects>().HasData(
            new Subjects { Id = 1, Description = "AM" },
            new Subjects { Id = 2, Description = "SEW" },
            new Subjects { Id = 3, Description = "INSY" },
            new Subjects { Id = 4, Description = "D" },
            new Subjects { Id = 5, Description = "E" }
        );
        
        modelBuilder.Entity("ClassesSubjects").HasData(
            new { CId = 5, SId = 2 }, 
            new { CId = 5, SId = 3 },
            new { CId = 1, SId = 2 }
        );
    }
}