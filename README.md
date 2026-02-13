# AELP - 英语学习辅助程序

AELP 是一个基于 Avalonia UI 框架开发的跨平台英语学习应用，专为学生群体设计，旨在通过单词测试、错题复习和数据统计等功能，帮助用户高效掌握英语词汇。

## ✨ 主要功能

*   **多模式测试**：
    *   支持**选择题**（根据翻译选单词）和**拼写填空**（根据翻译补全单词）两种模式。
    *   涵盖小学、高中、CET-4、CET-6、托福、雅思等多个难度级别的词汇库。
    *   支持自定义测试范围和题量。
*   **智能错题本**：
    *   系统自动记录测试中的错误单词。
    *   提供专门的错题复习功能，针对薄弱环节进行强化训练。
    *   支持按错误次数、时间等多种排序方式。
*   **数据可视化统计**：
    *   **历史准确率趋势**：通过折线图直观展示历次测试的正确率变化。
    *   **耗时分析**：通过饼图分析答题时间分布，帮助优化学习效率。
*   **词典与生词本**：
    *   内置本地词典查询功能（翻译查询 + 单词搜索双模式）。
    *   支持一键收藏生词到收藏夹，显示考试标签（CET4/CET6/高中/小学/托福/雅思）。
*   **个性化体验**：
    *   提供三种界面主题：暗色模式、明亮模式、护眼模式。
    *   暗色/亮色主题通过 `ThemeDictionaries` 实现自动切换，护眼主题提供强烈绿色调保护视力。
    *   支持自定义字体和选择题快捷键绑定。

## 🎨 主题系统

项目使用 **Semi.Avalonia + Ursa.Themes.Semi** 作为基础 UI 主题框架，在此基础上通过 `ResourceDictionary.ThemeDictionaries` 实现完整的明暗双色资源切换。

| 主题 | 说明 |
|------|------|
| 🌙 暗色 | 深色背景 + 蓝色强调色，默认主题 |
| ☀️ 亮色 | 浅色背景 + Material 蓝强调色 |
| 🌿 护眼 | 柔和绿色调全局覆盖，降低视觉疲劳 |

主题切换通过 `ThemeService` 管理，暗色/亮色使用 Avalonia 原生 `RequestedThemeVariant` 自动响应 `ThemeDictionaries`，护眼模式额外加载 `EyeCareTheme.axaml` 样式覆盖。

## 🛠️ 技术栈

*   **开发语言**: C# (.NET 10.0)
*   **UI 框架**: [Avalonia UI](https://avaloniaui.net/) 11.3
*   **UI 主题**: [Semi.Avalonia](https://github.com/irihitech/Semi.Avalonia) + [Ursa](https://github.com/irihitech/Ursa.Avalonia)
*   **MVVM 框架**: [CommunityToolkit.Mvvm](https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/)
*   **ORM 框架**: [Entity Framework Core](https://learn.microsoft.com/zh-cn/ef/core/) (SQLite)
*   **图表库**: [LiveCharts2](https://github.com/beto-rodriguez/LiveCharts2)
*   **测试框架**: xUnit

## 📂 项目结构概览

```
AELP/
├── AELP/                 # 核心逻辑与 UI 层
│   ├── Views/            # 用户界面 (.axaml)
│   ├── ViewModels/       # 视图模型 (MVVM)
│   ├── Models/           # 数据实体 (Word, CET4, Mistake 等)
│   ├── Services/         # 基础设施服务 (ThemeService, WordQuery 等)
│   ├── Data/             # 数据库上下文 & 数据模型
│   └── Styles/           # 自定义样式文件
│       ├── AppDefaultStyle.axaml   # 全局自定义样式（导航、卡片等）
│       ├── EyeCareTheme.axaml      # 护眼主题覆盖
│       └── LightTheme.axaml        # (已废弃 — 亮色主题由 ThemeDictionaries 处理)
├── AELP.Desktop/         # 桌面端宿主程序
└── AELP.UnitTest/        # 单元测试项目
```

## 🚀 快速开始

### 环境依赖
*   安装 .NET SDK (建议 .NET 10.0 或更高版本)
*   IDE 推荐: Visual Studio 2022, JetBrains Rider 或 VS Code

### 构建与运行

1.  **还原依赖包**
    ```bash
    dotnet restore
    ```

2.  **构建项目**
    ```bash
    dotnet build
    ```

3.  **启动程序**
    进入桌面端项目目录并运行：
    ```bash
    cd AELP.Desktop
    dotnet run
    ```
