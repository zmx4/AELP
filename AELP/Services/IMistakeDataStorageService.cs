using AELP.Data;

namespace AELP.Services;

public interface IMistakeDataStorageService
{
    public void SaveMistakeData(MistakeDataModel[] mistakeData);
    public MistakeDataModel[] LoadTestData();
}