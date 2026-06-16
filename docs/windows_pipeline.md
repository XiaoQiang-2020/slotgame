# Windows 流水线（本地测试优先）

目标：在 Windows 上快速验证 HybridCLR + YooAsset 的热更链路，并形成可复用的本地流水线脚本。

概览步骤：
1. 本地快速验证（Editor 模式）
2. 生成热更 DLL（Hotfix 项目）并放入 Unity 项目
3. 在 Unity 中生成/打包 AOT 元数据（HybridCLR 的 GenerateMD 步骤）
4. 使用 YooAsset 在本地构建资源包（或编辑器 PlayMode 验证）
5. 本地 Windows Player 构建以验证 IL2CPP + HybridCLR 运行（可选，验证 AOT）

一、准备工作
- 安装 Unity 2022.3 LTS（通过 Unity Hub）并用该版本打开项目。
- 在 `Assets/` 中导入 `HybridCLR` 与 `YooAsset` 的官方 demo 或包（先以 demo 为准）。
- 在项目中约定热更 DLL 的放置路径（例如：`Assets/HotfixDlls/` 或 `StreamingAssets/Hotfix/`），并在 `HotfixManager` 中配置该路径。

二、快速 Editor 验证（推荐作为日常迭代）
1. 使用 `dotnet build` 或 IDE 编译 Hotfix 项目（目标为 `netstandard2.0` 或兼容 Unity 的运行时）
```powershell
# 示例：在 Hotfix 项目目录下执行
dotnet build -c Release -o ../HotfixBuild
```
2. 将生成的 hotfix DLL 复制到 Unity 项目（示例：`Assets/HotfixDlls/`）
3. 在 Unity 编辑器中，启用 YooAsset 的编辑器 PlayMode（PlayMode）并运行游戏场景，验证热更代码能被加载并执行。通常在 Editor 模式下可以直接用 `Assembly.LoadFrom` 的方式加载 DLL（demo 提供实现）。

三、生成 AOT 元数据（必做，针对 IL2CPP 发布）
- 在做 IL2CPP 发布或验证前，必须运行 HybridCLR 的 AOT 元数据生成步骤（demo 通常提供 `GenerateMD` 的 Editor 方法或脚本）。
- 可以通过 Unity 的 batchmode 调用：
```powershell
"C:\Program Files\Unity\Hub\Editor\2022.3.x\Editor\Unity.exe" -batchmode -projectPath "D:\workstudio\unity\slotgame" -executeMethod <HybridCLR_GenerateMD_Method> -logFile "AOT_Generate.log" -quit
```
- 注意：`<HybridCLR_GenerateMD_Method>` 请替换为你使用的 HybridCLR 版本提供的静态方法名（参见 demo README）。
- 生成后的 AOT 元数据文件通常需要随包体或热更下发（依照 HybridCLR 文档）。

四、YooAsset 本地打包（示例脚本）
- 开发阶段可使用 YooAsset 的编辑器模式直接加载 Asset 数据；发布阶段需要用 YooAsset 构建资源包并上传到资源服务器。
- 可通过 Unity batchmode 执行 YooAsset 的构建方法（示例方法名需替换）：
```powershell
"C:\Program Files\Unity\Hub\Editor\2022.3.x\Editor\Unity.exe" -batchmode -projectPath "D:\workstudio\unity\slotgame" -executeMethod YooAsset.Editor.BuildScript.BuildAll -logFile "YooAsset_Build.log" -quit
```
- 本地测试步骤：
  - 运行构建脚本生成包（输出到 `Assets/AssetBundles/Local/` 或 `Build/YooAsset/`）
  - 在 `HotfixManager` 或 `ResourceManager` 中配置本地资源路径为该输出目录
  - 运行 Editor 或 Windows Player，验证资源加载与增量更新逻辑

五、示例 PowerShell 模板（`Tools/Build/BuildHotfix.ps1`）
- 本仓库提供模板脚本文件，可按需修改参数后执行（脚本位于 `Tools/Build/`）。

六、本地 Windows Player 验证（可选但强烈建议）
- 构建 Windows Standalone Player（IL2CPP）并把 AOT 元数据与热更 DLL 放在正确位置，运行并验证热更 DLL 是否可加载并执行。
- 观察日志是否有 MissingMethodException 或 AOT 相关错误。

七、常见问题定位
- 若出现 MissingMethodException：回到 demo 确认 AOT 元数据是否完整生成并按 HybridCLR 文档放置到包中。
- 若资源加载失败：检查 YooAsset 的 manifest 路径配置、包签名/hash 与本地目录是否一致。

八、下一步（建议）
- 我可以把下面的模板脚本加入仓库：
  - `Tools/Build/BuildHotfix.ps1`（编译 hotfix 并拷贝到 `Assets/HotfixDlls/`）
  - `Tools/Build/BuildYooAsset_Local.ps1`（调用 Unity batchmode 触发 YooAsset 本地包构建）
  - `Assets/Editor/HotfixPipeline.cs`（Unity Editor 的菜单命令，辅助执行或打印上面命令）

请确认是否要我把这些模板脚本写入仓库（回答 “写入” 或 “不用”）。