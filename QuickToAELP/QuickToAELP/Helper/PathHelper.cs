using System;
using System.IO;

namespace QuickToAELP.Helper;

/// <summary>
/// 路径辅助工具，提供应用目录与本地数据目录路径。
/// </summary>
public static class PathHelper {
    private static string _localFolder = string.Empty;
    private static string _appFolder = string.Empty;

    private static string LocalFolder {
        get {
            if (!string.IsNullOrEmpty(_localFolder)) {
                return _localFolder;
            }

            _localFolder =
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder
                        .LocalApplicationData), "AELP");

            if (!Directory.Exists(_localFolder)) {
                Directory.CreateDirectory(_localFolder);
            }

            return _localFolder;
        }
    }

    /// <summary>
    /// 获取本地应用数据目录下的文件完整路径。
    /// </summary>
    /// <param name="fileName">文件名或相对路径。</param>
    /// <returns>完整文件路径。</returns>
    public static string GetLocalFilePath(string fileName) {
        return Path.Combine(LocalFolder, fileName);
    }

    private static string AppFolder {
        get {
            if (!string.IsNullOrEmpty(_appFolder)) {
                return _appFolder;
            }

            _appFolder =
                AppContext.BaseDirectory;

            return _appFolder;
        }
    }
    
    /// <summary>
    /// 获取应用程序目录下的文件完整路径。
    /// </summary>
    /// <param name="fileName">文件名或相对路径。</param>
    /// <returns>完整文件路径。</returns>
    public static string GetAppFilePath(string fileName) {
        return Path.Combine(AppFolder, fileName);
    }
}