using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public class MistakeDataStorageService : IMistakeDataStorageService
{
    public async Task SaveMistakeData(MistakeDataModel[] mistakeData)
    {
        throw new System.NotImplementedException();
    }

    public async Task<MistakeDataModel[]> LoadTestData()
    {
        throw new System.NotImplementedException();
    }
}