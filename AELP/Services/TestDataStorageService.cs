using AELP.Data;

namespace AELP.Services;

public class TestDataStorageService : ITestDataStorageService
{
    public void SaveTestData(TestDataModel[] testData)
    {
        throw new System.NotImplementedException();
    }

    public TestDataModel[] LoadTestData()
    {
        throw new System.NotImplementedException();
    }

    public TestDataModel[] GetRecentTests(int count = 10)
    {
        throw new System.NotImplementedException();
    }
}