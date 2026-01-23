using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public class TestDataStorageService : ITestDataStorageService
{
    public async Task SaveTestData(TestDataModel[] testData)
    {
        throw new System.NotImplementedException();
    }

    public async Task<TestDataModel[]> LoadTestData()
    {
        throw new System.NotImplementedException();
    }

    public async Task<TestDataModel[]> GetRecentTests(int count = 10)
    {
        throw new System.NotImplementedException();
    }
}