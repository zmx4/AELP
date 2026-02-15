using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

/// <summary>
/// 用户数据数据库上下文。
/// </summary>
public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    /// <summary>
    /// 用户数据库文件名。
    /// </summary>
    public const string DbName = "userdata.sqlite";
    
    /// <summary>
    /// 单词数据表。
    /// </summary>
    public DbSet<WordDataModel> Words { get; set; }

    /// <summary>
    /// 收藏数据表。
    /// </summary>
    public DbSet<FavoritesDataModel> Favorites { get; set; }

    /// <summary>
    /// 测试记录表。
    /// </summary>
    public DbSet<TestDataModel> Tests { get; set; }

    /// <summary>
    /// 错题数据表。
    /// </summary>
    public DbSet<MistakeDataModel> Mistakes { get; set; }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.UseSqlite("Data Source=" + PathHelper.GetLocalFilePath(DbName));
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FavoritesDataModel>()
            .Property(f => f.WordId)
            .ValueGeneratedNever();

        modelBuilder.Entity<TestDataModel>()
            .Property(e => e.Mistakes)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            );
    }
}