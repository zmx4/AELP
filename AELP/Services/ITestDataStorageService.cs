using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

/// <summary>
/// 提供测试记录数据的持久化读写能力。
/// </summary>
public interface ITestDataStorageService
{
    /// <summary>
    /// 异步保存测试记录集合。
    /// </summary>
    /// <param name="testData">要保存的测试记录数组。</param>
    /// <returns>表示异步操作完成的任务。</returns>
    public Task SaveTestData(TestDataModel[] testData);

    /// <summary>
    /// 异步加载全部测试记录。
    /// </summary>
    /// <returns>测试记录数组。</returns>
    public Task<TestDataModel[]> LoadTestData();

    /// <summary>
    /// 获取最近的测试记录。
    /// </summary>
    /// <param name="count">返回记录数量，默认 10 条。</param>
    /// <returns>最近测试记录数组。</returns>
    public Task<TestDataModel[]> GetRecentTests(int count = 10);
}