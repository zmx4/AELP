using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
{
    public const string DbName = "userdata.sqlite";
    
    public DbSet<WordDataModel> Words { get; set; }
    public DbSet<FavoritesDataModel> Favorites { get; set; }
    public DbSet<TestDataModel> Tests { get; set; }
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