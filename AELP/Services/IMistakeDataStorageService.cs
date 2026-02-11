using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public interface IMistakeDataStorageService
{
    public Task SaveMistakeData(MistakeDataModel[] mistakeData);
    public Task<MistakeDataModel[]> LoadMistakeData();
    public Task<MistakeDataModel[]> LoadMistakeData(int count);
    public Task UpdateMistakeData(MistakeDataModel[] mistakeData);
    public Task<MistakeDataModel[]> LoadMistakeDataByWordIds(int[] wordIds);
}