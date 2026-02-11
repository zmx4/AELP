namespace AELP.Helper;

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
                        .LocalApplicationData), nameof(AELP));

            if (!Directory.Exists(_localFolder)) {
                Directory.CreateDirectory(_localFolder);
            }

            return _localFolder;
        }
    }

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
    
    public static string GetAppFilePath(string fileName) {
        return Path.Combine(AppFolder, fileName);
    }
}