# Demo 导入指南（HybridCLR + YooAsset）

目标：在 Unity 2022.3 LTS 项目中导入官方 demo 并在编辑器中验证功能。

## 一、HybridCLR 官方 Demo 导入

### 1.1 下载
- 官方 GitHub：https://github.com/focus-creative-games/hybridclr
- 进入 `Releases` 页面，下载 Unity 2022.3 LTS 兼容的 v2.x 系列（或最新稳定版）。
- 或直接克隆：
```bash
git clone https://github.com/focus-creative-games/hybridclr.git
cd hybridclr
git checkout v2.x.x  # 替换为稳定版本
```

### 1.2 导入步骤
1. 在下载的 `hybridclr/samples` 或 `hybridclr/Demo` 目录下找到 Unity 示例工程（或 `.unitypackage` 文件）。
2. 若是完整工程，记下其目录路径，后续参考其 `Assets` 和 `Packages` 结构。
3. 若是 `.unitypackage`，在我们的项目中：`Assets > Import Package > Custom Package` 并选择该 `.unitypackage` 文件。
4. 等待导入完成（可能需要编译）。

### 1.3 验证
在 HybridCLR 导入后，检查以下内容是否存在：
- `Assets/HybridCLR/`（运行时库）
- `Packages/manifest.json` 中是否包含 HybridCLR 的包记录
- Unity 菜单中是否出现 `HybridCLR` 菜单项

### 1.4 示例场景运行
- 导入后应在 `Assets/Scenes` 或类似位置看到 HybridCLR 示例场景（如 `Demo.unity`）。
- 打开该场景并点击 Play，观察是否能正常运行（通常显示热更加载状态或示例 UI）。

---

## 二、YooAsset 官方 Demo 导入

### 2.1 下载
- 官方 GitHub：https://github.com/tuyoogame/YooAsset
- 进入 `Releases`，下载 Unity 2022.3 LTS 兼容的版本（通常 v2.x 系列）。
- 或直接克隆：
```bash
git clone https://github.com/tuyoogame/YooAsset.git
cd YooAsset
git checkout v2.x.x  # 替换为稳定版本
```

### 2.2 导入步骤
1. 在下载的 `YooAsset/Samples` 或 `YooAsset/Demo` 中找到 Unity 示例工程。
2. 如果是 `.unitypackage`：在我们的项目中 `Assets > Import Package > Custom Package` 并导入。
3. 如果是完整工程：参考其 `Assets/YooAsset` 和 `Packages/manifest.json` 结构，手动复制到我们的项目中。
4. 导入时可能需要重新编译（等待 Console 清空错误）。

### 2.3 验证
检查以下内容：
- `Assets/YooAsset/`（核心库）
- `Packages/manifest.json` 是否包含 YooAsset 包记录
- Unity 菜单中是否出现 `YooAsset` 菜单项

### 2.4 示例场景运行
- 应在 `Assets/Scenes` 或类似位置看到 YooAsset 示例场景。
- 打开场景并点击 Play，观察资源加载与下载逻辑是否正常（通常显示资源包下载进度）。

---

## 三、集成验证清单

完成以下步骤以确保两个 demo 均可在编辑器中运行：

- [ ] HybridCLR 官方 demo 已下载或 UnityPackage 已导入
- [ ] YooAsset 官方 demo 已下载或 UnityPackage 已导入
- [ ] Unity 菜单中同时出现 `HybridCLR` 和 `YooAsset` 菜单项
- [ ] 两个 demo 的示例场景均能在 Editor playmode 中运行无误
- [ ] Console 中无关键错误（可能有警告，属正常）
- [ ] 开启 `Profiler` 或 Console，观察热更 DLL 加载或资源加载日志

---

## 四、常见问题排查

**Q：导入后 Console 显示缺少脚本或命名空间错误？**  
A：检查 target framework 版本是否为 `.NET Standard 2.1` 或兼容 Unity 的版本。通常 demo 的 `.csproj` 或 `Assembly Definition` 会自动配置，若不行需手动调整。

**Q：菜单项未出现？**  
A：等待 Unity 编译完成（观察左下角进度条），或手动点击 `Assets > Reimport All`。

**Q：场景无法播放，提示 hotfix 或资源加载失败？**  
A：这是正常的——demo 通常需要额外配置（如提供热更 DLL、资源包）。我们后续任务会建立这些配置。

---

## 五、下一步指示

- 若两个 demo 均能正常导入并在 Editor 中运行，回复 "✓ 导入成功" 来确认第2项完成。
- 若导入过程中遇到问题，请提供具体错误信息（Console 中的错误或警告文本），我会协助定位。

---

## 参考文档

- HybridCLR 中文文档：https://hybridclr.doc.code-philosophy.com/
- YooAsset 中文文档：https://yooasset.tuyoogame.com/
- 两者官方 GitHub 的 Releases 和 README 通常包含详细的集成步骤。
