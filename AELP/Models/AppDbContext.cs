using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace AELP.Models;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<CET4> CET4s { get; set; }

    public virtual DbSet<CET6> CET6s { get; set; }

    public virtual DbSet<HighSchool> HighSchools { get; set; }

    public virtual DbSet<PrimarySchool> PrimarySchools { get; set; }

    public virtual DbSet<dictionary> dictionaries { get; set; }

    public virtual DbSet<re> res { get; set; }

    public virtual DbSet<tf> tfs { get; set; }

    public virtual DbSet<y> ys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CET4>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("CET4");
        });

        modelBuilder.Entity<CET6>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("CET6");
        });

        modelBuilder.Entity<HighSchool>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("HighSchool");
        });

        modelBuilder.Entity<PrimarySchool>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("PrimarySchool");
        });

        modelBuilder.Entity<dictionary>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("dictionary");
        });

        modelBuilder.Entity<re>(entity =>
        {
            entity.HasKey(e => e.word);
        });

        modelBuilder.Entity<tf>(entity =>
        {
            entity.HasKey(e => e.word);

            entity.ToTable("tf");
        });

        modelBuilder.Entity<y>(entity =>
        {
            entity.HasKey(e => e.word);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
