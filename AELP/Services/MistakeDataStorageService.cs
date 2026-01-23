using System.Linq;
using System.Threading.Tasks;
using AELP.Data;
using Microsoft.EntityFrameworkCore;

namespace AELP.Services;

public class MistakeDataStorageService : IMistakeDataStorageService
{
    public async Task SaveMistakeData(MistakeDataModel[] mistakeData)
    {
        using var context = new UserDbContext();
        await context.Database.EnsureCreatedAsync();
        
        foreach (var item in mistakeData)
        {
            if (item.Id == 0)
            {
                await context.Mistakes.AddAsync(item);
            }
            else
            {
                context.Mistakes.Update(item);
            }
        }
        await context.SaveChangesAsync();
    }

    public async Task<MistakeDataModel[]> LoadTestData()
    {
        using var context = new UserDbContext();
        await context.Database.EnsureCreatedAsync();
        return await context.Mistakes.ToArrayAsync();
    }
}