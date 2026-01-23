using AELP.Helper;
using Microsoft.EntityFrameworkCore;

namespace AELP.Data;

public class UserDbContext : DbContext
{
    private const string DbName = "userdata.sqlite";
    
    public DbSet<WordDataModel> Words { get; set; }
    public DbSet<FavoritesDataModel> Favorites { get; set; }
    public DbSet<TestDataModel> Tests { get; set; }
    public DbSet<MistakeDataModel> Mistakes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=" + PathHelper.GetLocalFilePath(DbName));
    }
}