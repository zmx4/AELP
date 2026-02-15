using System.Threading.Tasks;
using AELP.Data;

namespace AELP.Services;

/// <summary>
/// 提供错题数据的持久化读写与更新能力。
/// </summary>
public interface IMistakeDataStorageService
{
    /// <summary>
    /// 异步保存错误数据
    /// </summary>
    /// <param name="mistakeData">错误数组</param>
    /// <returns>表示异步操作完成的任务。</returns>
    public Task SaveMistakeData(MistakeDataModel[] mistakeData);
    /// <summary>
    /// 异步加载错误数据
    /// </summary>
    /// <returns>错题数据数组。</returns>
    public Task<MistakeDataModel[]> LoadMistakeData();
    /// <summary>
    /// 异步加载指定数量的错误数据
    /// </summary>
    /// <param name="count">要加载的数据数量</param>
    /// <returns>错题数据数组。</returns>
    public Task<MistakeDataModel[]> LoadMistakeData(int count);
    /// <summary>
    /// 异步更新错误数据
    /// </summary>
    /// <param name="mistakeData">要更新的错误数据</param>
    /// <returns>表示异步操作完成的任务。</returns>
    public Task UpdateMistakeData(MistakeDataModel[] mistakeData);
    /// <summary>
    /// 根据单词 ID 集合异步加载错题数据。
    /// </summary>
    /// <param name="wordIds">单词 ID 数组。</param>
    /// <returns>匹配到的错题数据数组。</returns>
    public Task<MistakeDataModel[]> LoadMistakeDataByWordIds(int[] wordIds);
}