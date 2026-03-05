using Microsoft.EntityFrameworkCore;

namespace Model.Context;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    public MyDbContext() { }

    //Standardmäßig Table per Hierachy (TPH) / Single Table
    public virtual DbSet<Animal> Animals { get; set; }
    public virtual DbSet<Dog> Dogs { get; set; }
    public virtual DbSet<Bird> Birds { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Vererbung_EF");
        }
    }
}