using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Model;

public partial class StundenplanContext : DbContext
{
    public StundenplanContext()
    {
    }

    public StundenplanContext(DbContextOptions<StundenplanContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Classes> Classes { get; set; }
    public virtual DbSet<Subjects> Subjects { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseMySQL("Server=127.0.0.1;uid=root;pwd=insy;database=Stundenplan");
}