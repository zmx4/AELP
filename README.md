# AELP - 英语学习辅助程序

AELP 是一个基于 Avalonia UI 框架开发的跨平台英语学习应用，专为学生群体设计，旨在通过单词测试、错题复习和数据统计等功能，帮助用户高效掌握英语词汇。

## ✨ 主要功能

*   **多模式测试**：
    *   支持**选择题**（根据翻译选单词）和**拼写填空**（根据翻译补全单词）两种模式。
    *   涵盖小学、高中、CET-4、CET-6 等多个难度级别的词汇库。
    *   支持自定义测试范围和题量。
*   **智能错题本**：
    *   系统自动记录测试中的错误单词。
    *   提供专门的错题复习功能，针对薄弱环节进行强化训练。
*   **数据可视化统计**：
    *   **历史准确率趋势**：通过折线图直观展示历次测试的正确率变化。
    *   **耗时分析**：通过饼图分析答题时间分布，帮助优化学习效率。
*   **词典与生词本**：
    *   内置本地词典查询功能。
    *   支持一键收藏生词到收藏夹，方便随时查阅和背诵。
*   **个性化体验**：
    *   提供多种界面主题（如明亮模式、护眼模式），保护视力。

## 🛠️ 技术栈

本项目基于 .NET 平台开发，采用现代化的 MVVM 架构，主要技术选型如下：

*   **开发语言**: C# (.NET 10.0)
*   **UI 框架**: [Avalonia UI](https://avaloniaui.net/) (支持 Windows, Linux, macOS 等跨平台运行)
*   **MVVM 框架**: [CommunityToolkit.Mvvm](https://learn.microsoft.com/zh-cn/dotnet/communitytoolkit/mvvm/)
*   **ORM 框架**: [Entity Framework Core](https://learn.microsoft.com/zh-cn/ef/core/) (配合 SQLite 本地数据库)
*   **图表库**: [LiveCharts2](https://github.com/beto-rodriguez/LiveCharts2) (用于实现统计图表)
*   **测试框架**: xUnit

## 📂 项目结构概览

```
AELP/
├── AELP/                 # 核心逻辑与 UI 层
│   ├── Views/            # 用户界面 (.axaml)
│   ├── ViewModels/       # 视图模型 (处理业务逻辑与状态)
│   ├── Models/           # 数据实体 (Word, CET4, Mistake 等)
│   ├── Services/         # 基础设施服务 (数据存储, 单词查询等)
│   └── Data/             # 数据库上下文配置
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
