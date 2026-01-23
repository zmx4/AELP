using System;
using AELP.Helper;
using Avalonia.Controls.Shapes;

namespace AELP.Services;

public class UserDbService : IUserDbService
{
    public const string DbName = "userdb.splite";
    
    public static readonly string DbPath = PathHelper.GetLocalFilePath(DbName);
    
    
    
}