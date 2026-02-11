using System;

namespace AELP.Helper;

public static class StringNormalizeHelper
{
    public  static string? NormalizeTranslation(string? translation)
    {
        return translation?
            .Replace("\\r\\n", "\n", StringComparison.Ordinal)
            .Replace("\\n", "\n", StringComparison.Ordinal);
    }
    
    public  static string? ShortenString(string? translation)
    {
        return translation?
            .Split("\\n")[0];
    }
}