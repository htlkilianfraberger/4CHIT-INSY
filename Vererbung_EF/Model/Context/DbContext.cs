using Microsoft.EntityFrameworkCore;

namespace Model.Context;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public MyDbContext()
    {
    }

    //Standardmäßig Table per Hierachy (TPH) / Single Table
    public virtual DbSet<Animal> Animals { get; set; }
    public virtual DbSet<Dog> Dogs { get; set; }
    public virtual DbSet<Bird> Birds { get; set; }
    public virtual DbSet<a> A { get; set; }
    public virtual DbSet<b> B { get; set; }
    public virtual DbSet<c> C { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Vererbung_EF");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        /*modelBuilder.Entity<Animal>()
            .HasDiscriminator<string>("animal_type") //Discriminator Spalte umbenennen (nur TPH)
            .HasValue<Animal>("Common_animal")
            .HasValue<Dog>("Wauwau")
            .HasValue<Bird>("Beep");*/

        //modelBuilder.Entity<Animal>().UseTptMappingStrategy(); //TPT
        modelBuilder.Entity<Animal>().UseTpcMappingStrategy(); //TPC
        //modelBuilder.Entity<Animal>().ToTable("Animals"); //(optional)
        modelBuilder.Entity<Dog>().ToTable("Dogs");
        modelBuilder.Entity<Bird>().ToTable("Birds");
    }
    
    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<a>().HasMany<b>().WithMany().UsingEntity<c>(
            c => c.HasOne<b>().WithMany().HasForeignKey(c => c.BId),
            c => c.HasOne<a>().WithMany().HasForeignKey(c => c.AId));
    }*/
}