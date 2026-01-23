using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

public interface ITestDataStorageService
{
    public Task SaveTestData(TestDataModel[] testData);
    public Task<TestDataModel[]> LoadTestData();
    public Task<TestDataModel[]> GetRecentTests(int count = 10);
}