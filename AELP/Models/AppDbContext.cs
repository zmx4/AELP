using Microsoft.EntityFrameworkCore;

namespace AELP.Models;

/// <summary>
/// 词典数据库上下文。
/// </summary>
public partial class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    /// <summary>
    /// CET4 词表。
    /// </summary>
    public virtual DbSet<Cet4> Cet4S { get; set; }

    /// <summary>
    /// CET6 词表。
    /// </summary>
    public virtual DbSet<Cet6> Cet6S { get; set; }

    /// <summary>
    /// 高中词表。
    /// </summary>
    public virtual DbSet<HighSchool> HighSchools { get; set; }

    /// <summary>
    /// 小学词表。
    /// </summary>
    public virtual DbSet<PrimarySchool> PrimarySchools { get; set; }

    /// <summary>
    /// 综合词典表。
    /// </summary>
    public virtual DbSet<Dictionary> Dictionaries { get; set; }

    /// <summary>
    /// 额外词表。
    /// </summary>
    public virtual DbSet<Re> Res { get; set; }

    /// <summary>
    /// 托福词表。
    /// </summary>
    public virtual DbSet<Tf> Tfs { get; set; }

    /// <summary>
    /// 雅思词表。
    /// </summary>
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

    /// <summary>
    /// 供局部类扩展模型配置。
    /// </summary>
    /// <param name="modelBuilder">模型构建器。</param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
