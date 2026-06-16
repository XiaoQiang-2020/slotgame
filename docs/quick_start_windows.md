快速启动（Windows 本地验证）

1. 用 Unity Hub 创建/打开项目：选择 Unity 2022.3 LTS，打开 `d:/workstudio/unity/slotgame` 目录。
2. 在 Unity 中导入 `Assets/` 文件夹内容（如果尚未导入）。
3. 在 `Assets/Scenes` 创建并保存 `Startup.unity`、`Login.unity`、`Game.unity`。把 `Startup` 设为默认场景（File > Build Settings > Scenes In Build）。
4. 打开 `HotfixPipeline` 菜单（`Hotfix`）来构建 hotfix DLL 或生成 AOT（需先配置 Hotfix 项目路径：EditorPrefs.SetString("HotfixProjectPath", "D:/path/to/Hotfix.csproj")）。
5. 在 `Assets/HotfixDlls/` 放入 hotfix DLL，运行 `Startup` 场景并验证热更代码与资源加载。
6. 若要本地构建 YooAsset 包，请运行 `Tools/Build/BuildYooAsset_Local.ps1` 或调用 `YooAsset` 的构建方法。

问题排查：
- 缺少方法/类型：确认已运行 HybridCLR 的 GenerateMD 步骤并把生成的 AOT 元数据放到正确位置。
- 资源加载失败：检查 YooAsset 的 manifest 路径与 `ResourceManager` 的本地目录配置。
