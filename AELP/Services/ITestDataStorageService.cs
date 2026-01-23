using AELP.Data;

namespace AELP.Services;

public interface ITestDataStorageService
{
    public void SaveTestData(TestDataModel[] testData);
    public TestDataModel[] LoadTestData();
    public TestDataModel[] GetRecentTests(int count = 10);
}