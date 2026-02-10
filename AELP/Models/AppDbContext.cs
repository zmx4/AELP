using Microsoft.EntityFrameworkCore;

namespace AELP.Models;

public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public virtual DbSet<Cet4> Cet4S { get; set; }

    public virtual DbSet<Cet6> Cet6S { get; set; }

    public virtual DbSet<HighSchool> HighSchools { get; set; }

    public virtual DbSet<PrimarySchool> PrimarySchools { get; set; }

    public virtual DbSet<Dictionary> Dictionaries { get; set; }

    public virtual DbSet<Re> Res { get; set; }

    public virtual DbSet<Tf> Tfs { get; set; }

    public virtual DbSet<Y> Ys { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cet4>(entity =>
        {
            entity.HasKey(e => e.RawWord);

            entity.ToTable("CET4");
        });

        modelBuilder.Entity<Cet6>(entity =>
        {
            entity.HasKey(e => e.RawWord);

            entity.ToTable("CET6");
        });

        modelBuilder.Entity<HighSchool>(entity =>
        {
            entity.HasKey(e => e.RawWord);

            entity.ToTable("HighSchool");
        });

        modelBuilder.Entity<PrimarySchool>(entity =>
        {
            entity.HasKey(e => e.RawWord);

            entity.ToTable("PrimarySchool");
        });

        modelBuilder.Entity<Dictionary>(entity =>
        {
            entity.HasKey(e => e.RawWord);
            
            entity.ToTable("dictionary");
        });

        modelBuilder.Entity<Re>(entity =>
        {
            entity.HasKey(e => e.RawWord);
        });

        modelBuilder.Entity<Tf>(entity =>
        {
            entity.HasKey(e => e.RawWord);

            entity.ToTable("tf");
        });

        modelBuilder.Entity<Y>(entity =>
        {
            entity.HasKey(e => e.RawWord);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
