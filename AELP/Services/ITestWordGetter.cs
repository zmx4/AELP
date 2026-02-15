using System.Collections.Generic;
using System.Threading.Tasks;
using AELP.Data;
using AELP.Models;

namespace AELP.Services;

/// <summary>
/// 提供测试题目单词抽取能力。
/// </summary>
public interface ITestWordGetter
{
    /// <summary>
    /// 根据数量和范围获取用于测试的单词集合。
    /// </summary>
    /// <param name="count">需要获取的单词数量。</param>
    /// <param name="testRange">测试范围。</param>
    /// <returns>用于测试的单词列表。</returns>
    public Task<List<Word>> GetTestWords(int count, TestRange testRange);
}