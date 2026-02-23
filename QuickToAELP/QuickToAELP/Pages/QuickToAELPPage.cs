// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using QuickToAELP.Helper;

namespace QuickToAELP.Pages;

internal sealed partial class QuickToAELPPage : ListPage
{
    private readonly string _programPath;
    
    public QuickToAELPPage()
    {
        // Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Icon = new("\ud83d\udcd6");
        Title = "AELP";
        Name = "Open";
        _programPath = PathHelper.GetAppFolderPath();
    }

    public override IListItem[] GetItems()
    {
        // var command = new OpenUrlCommand("https://learn.microsoft.com");
        // var command = ;
        return [
            new ListItem(new Command.StartAppCommand(_programPath))
            {
                Icon =  new IconInfo("\ud83d\ude80"), // 🚀 图标
                Title = "Start AELP",
            },
            new ListItem(new Command.StartAppCommand(_programPath,"test"))
            {
                Icon = new IconInfo("📝"), // 📝 图标
                Title = "Start AELP with test",
            }
        ];
    }
}