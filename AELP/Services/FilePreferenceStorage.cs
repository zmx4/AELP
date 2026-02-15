using System;
using System.IO;
using AELP.Helper;

namespace AELP.Services;

/// <summary>
/// 基于文件系统的偏好设置存储实现。
/// 每个键对应本地目录中的一个文件。
/// </summary>
public class FilePreferenceStorage : IPreferenceStorage {
    /// <summary>
    /// 保存整型偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">整型值。</param>
    public void Set(string key, int value) => Set(key, value.ToString());

    /// <summary>
    /// 读取整型偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中或解析失败时返回的默认值。</param>
    /// <returns>读取到的整型值。</returns>
    public int Get(string key, int defaultValue) =>
        int.TryParse(Get(key, string.Empty), out var value)
            ? value
            : defaultValue;

    /// <summary>
    /// 保存字符串偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">字符串值。</param>
    public void Set(string key, string value) {
        var path = PathHelper.GetLocalFilePath(key);
        File.WriteAllText(path, value);
    }

    /// <summary>
    /// 读取字符串偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中时返回的默认值。</param>
    /// <returns>读取到的字符串值。</returns>
    public string Get(string key, string defaultValue) {
        var path = PathHelper.GetLocalFilePath(key);
        return File.Exists(path) ? File.ReadAllText(path) : defaultValue;
    }

    /// <summary>
    /// 保存日期时间偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="value">日期时间值。</param>
    public void Set(string key, DateTime value) => Set(key, value.ToString());

    /// <summary>
    /// 读取日期时间偏好值。
    /// </summary>
    /// <param name="key">偏好键。</param>
    /// <param name="defaultValue">未命中或解析失败时返回的默认值。</param>
    /// <returns>读取到的日期时间值。</returns>
    public DateTime Get(string key, DateTime defaultValue) =>
        DateTime.TryParse(Get(key, ""), out var value) ? value : defaultValue;
}