using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public interface IMistakeDataStorageService
{
    public Task SaveMistakeData(MistakeDataModel[] mistakeData);
    public Task<MistakeDataModel[]> LoadTestData();
}