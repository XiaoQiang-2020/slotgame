# Setup: HybridCLR + YooAsset 集成快速指南

注意：以下步骤为概览，具体以各官方仓库 README 为准。

1) 环境准备
- 安装 Unity 2022.3 LTS（使用 Unity Hub 管理版本）。
- 确保已安装对应平台的构建支持（Windows/Mac/Android/iOS）。

2) 下载官方 demo
- 从官方 GitHub 或发布页面获取 `HybridCLR` 与 `YooAsset` 的官方 demo/示例工程（通常包含 `Example` 或 `Samples` 目录）。

示例（请替换为官方地址）：
```bash
# 克隆示例仓库（替换为真实 URL）
git clone <HYBRIDCLR_OFFICIAL_REPO_URL>
git clone <YOOASSET_OFFICIAL_REPO_URL>
```

3) 在 Unity 中导入 demo
- 打开 Unity Hub，Create -> New Project -> 选择 Unity 2022.3 LTS。
- 在新建项目中，通过 `Assets/Import Package/Custom Package...` 或直接把 demo 的 `Assets`/`Packages` 文件夹拷贝到项目中。
- 打开 demo 场景并运行，确认示例可在编辑器运行。

4) 验证 HybridCLR AOT 与热更流程
- 按 demo 指南运行 AOT 元数据导出（GenerateMD）与热更 DLL 打包步骤。
- 在 IL2CPP 平台上验证补丁 DLL 能被 HybridCLR 正确加载。注意：某些平台需要额外的 AOT 元数据配置，参见 demo 的 `AOT` 目录。

5) 验证 YooAsset 资源打包与加载
- 按 demo 指南构建资源包并通过 YooAsset 在运行时加载资源，测试在线下发与增量更新流程。

6) 在 demo 基础上裁剪为单游戏结构
- 简化场景为：`Startup`（热更与资源初始化）、`Login`（登录与基础 UI）、`Game`（实际 slot 游戏场景）。
- 在 `Assets/Scripts` 下建立 MVC 目录：
  - `MVC/Model`
  - `MVC/View`
  - `MVC/Controller`

7) AOT/构建注意事项（快速提示）
- 在构建前确认 HybridCLR 的 AOT 元数据已生成并打包随 APK/包体或单独下发，避免运行时报 MissingMethodException。
- 保持热更 DLL 的命名、版本管理与签名策略（本地测试可使用简单版本号方案）。

8) 下一步
- 我可以为你在仓库中生成一个 MVC 的 C# 脚手架、示例单例管理类与简单场景文件结构（不包含 Unity 二进制工程文件）。

```
# 若需下载真实 demo，请把它们的官方 URL 发给我，或允许我使用我推荐的官方版本地址来生成详细的拉取/集成脚本。
```
