# Slot 子游戏（Unity 2022 LTS + HybridCLR + YooAsset）

目标：快速搭建单游戏（非金币大厅）示例工程，包含启动加载场景、登录模块、游戏场景，并使用 HybridCLR + YooAsset 实现热更能力。

推荐版本（基线，按需微调）：
- Unity: 2022.3 LTS
- HybridCLR: 官方稳定版（兼容 Unity 2022.3 LTS 的最新 v2.x 系列）
- YooAsset: 官方稳定版（v2.x 系列或最新稳定发行）
- 脚本语言: 纯 C#（.NET Standard / Unity 自带运行时）

为什么从官方 demo 起步：
- 官方 demo 已包含热更流水线、AOT 元数据处理示例、资源打包范例，能显著缩短集成时间并降低踩坑概率。

快速开始流程（概要）：
1. 使用 Unity Hub 创建 Unity 2022.3 LTS 项目（项目类型: 3D 或 URP，根据需求）。
2. 在项目中按官方步骤导入 `HybridCLR` 与 `YooAsset` 的 demo 或 UnityPackage。
3. 运行 demo 场景，确认在编辑器/目标平台下 demo 可运行。
4. 在 demo 基础上裁剪场景为 `启动加载`、`登录`、`游戏` 三个场景，并在 `Assets/Scripts` 下搭建 MVC 基础目录。
5. 配置 HybridCLR 的 AOT 元数据导出与热更 DLL 打包流程（参考 demo 的 AOT/GenerateMD 步骤）。
6. 使用 YooAsset 打包资源并测试热更下发与加载流程。

本仓库将包含：
- 基础仓库脚手架与使用说明（`README.md`、`.gitignore`、`docs/setup.md`）。
- 后续任务：生成 Unity 项目模板、MVC 目录结构与示例脚本。

下一步：参照 `docs/setup.md` 下载并导入官方 demo，或允许我帮你在本仓库生成本地脚手架与 MVC 模板。