# SpaceShooter + Framework/App 集成子计划

## 目标

以 YooAsset 官方 `SpaceShooter` demo 为基础，完成超级休闲游戏项目的第一条可运行内容线。项目不是大厅加多个子模式的架构，而是一款以轻量玩法快速迭代为目标的单游戏。当前方向不再是“直接引用 sample 原样运行”，而是采用 **Framework 抽取版 + App 主流程 + Game 内容目录**：

- 将 YooAsset patch、状态机、事件桥接等公共能力抽取到主工程 `Framework` 层。
- 将 Boot、Loading、Login、场景流转等应用主流程放到 `App` 层。
- 将 SpaceShooter 的玩法、资源、场景和 UI 迁移到正式游戏内容目录。
- 保留官方 sample 作为来源对照和回溯基线，但不再作为长期开发目录。

最终目标是：调整代码架构后，直接运行主工程入口即可完成 `Boot -> Patch -> Loading -> Login -> Loading -> Game` 的基础流程。

## 当前结论

采用 **A 方案：Framework 抽取版**。

`Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter` 不再承担主工程运行入口职责。它的定位是官方 demo 来源、迁移对照、后续差异回查基线。

主工程正式维护以下三层：

1. `Assets/Scripts/Framework/`
   - 公共底座。
   - 放 YooAsset patch 流程、状态机、事件、资源封装、基础 MVC、通用工具。
   - 不依赖具体玩法内容，不出现 SpaceShooter 专属玩法概念。

2. `Assets/Scripts/App/`
   - 应用壳层和主流程。
   - 放 Boot/Startup、Loading、Login、SceneNavigator、LoadingTarget、主流程编排。
   - 负责串起补丁更新、登录、加载、进入正式游戏内容。

3. `Assets/Games/SpaceShooter/`
   - 第一条轻量休闲游戏内容线。
   - 放从 sample 迁出的 SpaceShooter 玩法脚本、Home/Battle、游戏 UI、游戏资源、游戏场景。
   - 后续玩法迭代、资源替换、游戏逻辑修改以这里为准。

## 设计原则

1. **公共能力抽到 Framework**
   - Patch 流程、事件定义、状态机节点、资源加载封装属于公共能力。
   - Framework 层应保持可复用，避免绑定 SpaceShooter 的 Home/Battle/飞船战斗等概念。

2. **主流程放到 App**
   - Loading、Login、Boot、场景跳转不是底层框架，也不是玩法独有逻辑。
   - 它们属于主应用壳层，统一放到 `Assets/Scripts/App/`。
   - App 层需要继续区分随包 Core 和可热更 HotUpdate。

3. **游戏内容迁到 Games**
   - SpaceShooter 不只是参考代码，而是第一条轻量休闲游戏内容线。
   - 玩法相关脚本、资源、场景、UI 迁到 `Assets/Games/SpaceShooter/` 后继续演进。

4. **Sample 保持可回溯**
   - 官方 sample 保留在 `Assets/Samples/...` 作为来源对照。
   - 不在 sample 目录里继续做长期业务开发。
   - 如需对照官方 demo 行为，优先回查 sample。

5. **可运行优先**
   - 每次目录迁移或边界调整后，都要保证主工程入口可以直接运行。
   - 先跑通完整流程，再逐步优化内部结构。

6. **热更边界清晰**
   - Framework 是稳定公共模块，原则上随包发布，不作为常规热更目标。
   - 玩法逻辑、数值配置、游戏 UI 逻辑走 HybridCLR DLL 全量更新。
   - 游戏 UI prefab、动画、图片、音效、配置资源走 YooAsset 资源分组更新。
   - App/Login 默认作为可热更模块，但必须保留随包兜底流程。

## 目标目录规划

```text
Assets/
  Scenes/
    Boot.unity
    Loading.unity
    Login.unity
    Game.unity

  Scripts/
    Framework/
      Patch/
      MVC/
      Resource/
      Utils/

    App/
      Core/
        Boot/
        Scene/
        FallbackUI/
      HotUpdate/
        Loading/
        Login/
      Config/

  Games/
    SpaceShooter/
      Scripts/
        GameLogic/
        BattleLogic/
        WindowLogic/
        Behaviour/
      Resources/
      GameRes/
      GameSetting/
      Scenes/

  Samples/
    YooAsset/
      3.0.2-beta/
        SpaceShooter/
```

说明：

- `Assets/Scenes/Boot.unity` 是正式启动入口，可由 sample `Boot.unity` 迁出或重建。
- `Assets/Scripts/App/Core/` 负责启动、兜底、热更 DLL 加载前后的最小流程。
- `Assets/Scripts/App/HotUpdate/` 负责可热更的 Login、正式 Loading 和业务入口表现。
- `Assets/Scripts/App/Config/` 负责公共配置定义，配置数据可区分内置兜底和远端更新版本。
- `Assets/Games/SpaceShooter/` 是正式游戏内容目录，不再把 sample 目录作为开发主线。
- `Assets/Samples/.../SpaceShooter` 保留为官方导入版本和迁移来源。

## 模块职责

### Framework/Patch

来源于 YooAsset SpaceShooter demo 的 patch 流程抽取，负责：

- 初始化 YooAssets。
- 创建和驱动 PatchManager 状态机。
- 保留 UniFramework.Event 事件机制。
- 处理包初始化、版本请求、Manifest 更新、下载、元数据加载、缓存清理。
- 在 patch 完成后发布 `PatchCompletedEvent`。

建议保留或维护的核心类：

- `PatchBoot`
- `PatchFacade`
- `PatchModel`
- `PatchController`
- `PatchView`
- `PatchManager`
- `PatchEventDefine`
- `PatchEvents`
- `FsmInitializePackage`
- `FsmRequestPackageVersion`
- `FsmUpdatePackageManifest`
- `FsmCreateDownloader`
- `FsmDownloadPackageFiles`
- `FsmDownloadPackageOver`
- `FsmLoadMetadata`
- `FsmClearCacheBundle`
- `FsmStartGame`

边界要求：

- Framework/Patch 可以使用 YooAsset、UniEvent、UniMachine。
- Framework/Patch 不直接引用 SpaceShooter 的 Home/Battle/GameLogic。
- Patch 完成后只发出通用完成事件，不直接决定进入具体玩法场景。
- Framework/Patch 原则上随包发布，不作为常规热更目标。

### App/Core

应用随包核心层，负责：

- 正式启动入口 `Boot`。
- 设置帧率和后台运行。
- 初始化 UniEvent。
- 初始化 YooAssets。
- 创建并启动 Framework/Patch。
- 挂载或生成 patch UI。
- 加载 HybridCLR AOT 元数据和热更 DLL。
- 在 patch 完成和 DLL 加载完成后进入 App 热更流程。
- 提供兜底 Loading、兜底错误提示和失败恢复入口。

`Boot.unity` 应迁到 `Assets/Scenes/Boot.unity`，并加入 Build Settings 第一位。

App/Core 不应频繁修改，也不应依赖热更 DLL 才能显示关键错误信息。

### App/HotUpdate/Loading

可热更的应用级加载过渡层，负责：

- 显示加载进度和提示。
- 根据目标加载 `Login` 或游戏入口场景。
- 承接 patch 完成后的跳转，也承接 Login 后进入游戏的跳转。
- UI prefab、动画和图片资源走 YooAsset 资源分组更新。
- Loading 表现逻辑可随 HybridCLR DLL 全量更新。

### App/HotUpdate/Login

默认可热更的应用级登录层，负责：

- 提供当前阶段的登录 stub。
- 保存基础登录状态。
- 登录成功后进入 Loading，再进入目标游戏。
- 登录 UI 逻辑、登录流程、公告、轻量运营入口可随 HybridCLR DLL 全量更新。
- 登录 UI prefab、图片、动画、文案配置可走 YooAsset 资源分组更新。

登录逻辑不应写入 Framework，也不应放进 SpaceShooter 游戏内容目录。即使 Login 默认可热更，也必须由 App/Core 提供最小兜底界面，用于热更失败、DLL 加载失败或资源缺失时提示用户。

### Games/SpaceShooter

SpaceShooter 是第一条游戏内容线，负责：

- Home/Battle 玩法逻辑。
- 游戏窗口 UI。
- 飞船、敌人、子弹、陨石、特效等玩法实体。
- 游戏资源、配置、场景。

迁移时应尽量保持 demo 原有结构，降低资源引用和脚本引用断裂风险。

玩法逻辑、数值配置读取、UI 控制逻辑应放入 HybridCLR 热更 DLL。游戏 UI prefab、动画、图片、音效、关卡配置、数值表等资源走 YooAsset 资源分组更新。

## 场景流程

目标流程：

```text
Boot
  -> Patch
  -> Load HotUpdate DLL
  -> Loading(Login)
  -> Login
  -> Loading(Game)
  -> Game
```

阶段说明：

1. `Boot`
   - 正式入口场景。
   - 位于 `Assets/Scenes/Boot.unity`。
   - 负责启动 patch 流程。

2. `Patch`
   - 由 `Framework/Patch` 驱动。
   - 完成后发布 `PatchCompletedEvent`。

3. `Loading(Login)`
   - `App/Core` 或热更入口设置目标为 Login。
   - `App/HotUpdate/Loading` 加载 Login 场景。

4. `Login`
   - 应用级登录流程。
   - 默认作为 HybridCLR 可热更模块。

5. `Loading(Game)`
   - 登录成功后进入加载过渡。
   - 进入正式游戏内容入口。

6. `Game`
   - 当前阶段可以先进入统一 `Game` 场景。
   - SpaceShooter 作为当前游戏内容模板和玩法实现来源。

## 与旧文档方案的修正

旧文档中存在两类互相冲突的描述：

- 一部分描述“保留官方 demo 原样，通过桥接层接入 MVC”。
- 另一部分描述“将公共 Patch 框架迁移到 `Assets/Scripts/Framework/Patch/`”。

当前统一修正为：

- 不回退到 sample 原样桥接版。
- 承认并继续推进 Framework 抽取版。
- App 主流程独立于 Framework。
- SpaceShooter 游戏内容迁入 `Assets/Games/SpaceShooter/` 后继续开发。
- Sample 保留为来源基线，不作为长期业务开发目录。
- 项目定位为超级休闲单游戏，不按大厅多子模式架构设计。

## 实施步骤

### 1. 校准现有代码目录

- 保留 `Assets/Scripts/Framework/Patch/` 中已迁移的 patch 代码。
- 将现有 `Assets/Scripts/Framework/Login/` 迁到 `Assets/Scripts/App/HotUpdate/Login/`。
- 将现有 `Assets/Scripts/Framework/Scene/` 迁到 `Assets/Scripts/App/Core/Scene/` 或 `Assets/Scripts/App/HotUpdate/Loading/`。
- 评估 `Assets/Scripts/Framework/Game/`：
   - 若是通用游戏入口壳层，迁到 `Assets/Scripts/App/Core/Game/`。
   - 若是 SpaceShooter 玩法逻辑，迁到 `Assets/Games/SpaceShooter/Scripts/`。

### 2. 迁移 Boot 场景

- 将 sample `Boot.unity` 迁出或重建为 `Assets/Scenes/Boot.unity`。
- 绑定 App/Boot 或 Framework/Patch 启动脚本。
- 确认 Build Settings 中 `Boot` 是第一启动场景。

### 3. 迁移 SpaceShooter 游戏内容

- 创建 `Assets/Games/SpaceShooter/`。
- 迁移 SpaceShooter 的独有内容：
  - `GameScript/Runtime/GameLogic`
  - `GameScript/Runtime/BattleLogic`
  - `GameScript/Runtime/WindowLogic`
  - `GameScript/Runtime/Behaviour`
  - `GameRes`
  - `GameSetting`
  - 游戏场景
- 不把这些独有内容放入 Framework。

### 3.1 明确热更分组

- Framework/Patch、App/Core、兜底 UI 随包发布。
- App/HotUpdate/Login 和 App/HotUpdate/Loading 逻辑进入 HybridCLR 热更 DLL。
- Games/SpaceShooter 玩法逻辑、UI 控制、配置读取进入 HybridCLR 热更 DLL。
- Login、Loading、SpaceShooter 的 prefab、动画、图片、音效、配置表走 YooAsset 资源分组。

### 4. 保留 sample 来源

- 保留 `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter`。
- 不再把它作为主工程启动入口。
- 后续如需要精简 sample，可在确认迁移完整后单独规划。

### 5. 更新场景跳转

- `PatchCompletedEvent` 触发热更 DLL 加载和进入 `Loading(Login)`。
- `Login` 成功后进入 `Loading(Game)`。
- `Loading` 根据目标加载正式场景。
- 场景名统一在 App 场景常量中维护。

### 6. 验证完整运行链

- 验证主工程从 `Assets/Scenes/Boot.unity` 启动。
- 验证 patch 流程可正常完成。
- 验证 HybridCLR DLL 和 AOT 元数据加载成功。
- 验证 patch 完成后进入 Loading，再进入 Login。
- 验证 Login 后进入 Loading，再进入 Game。
- 验证 SpaceShooter 资源和脚本引用不因迁移断裂。

## 最小可行任务拆分

- 任务 1：确认 `Framework/Patch` 已迁移代码与 sample patch 流程差异。
- 任务 2：创建 `Assets/Scripts/App/Core/`，迁移 Boot、Scene、兜底 UI 和热更 DLL 加载入口。
- 任务 3：创建 `Assets/Scripts/App/HotUpdate/`，迁移 Login 和正式 Loading 逻辑。
- 任务 4：创建 `Assets/Games/SpaceShooter/` 并迁移游戏内容。
- 任务 5：将正式 Boot 场景迁到 `Assets/Scenes/Boot.unity`。
- 任务 6：修正 SceneNavigator/SceneNames/LoadingTarget 等引用。
- 任务 7：更新 Build Settings 场景顺序。
- 任务 8：配置 HybridCLR DLL 全量更新和 YooAsset 资源分组。
- 任务 9：运行验证完整流程：Boot -> Patch -> Load HotUpdate DLL -> Loading -> Login -> Loading -> Game。
- 任务 10：记录 sample 与主工程迁移后代码的差异点。

## 场景生成后绑定检查清单

- [ ] `Assets/Scenes/Boot.unity` 已存在，并已加入 Build Settings 第一位。
- [ ] `Assets/Scenes/Loading.unity` 已存在，并已加入 Build Settings。
- [ ] `Assets/Scenes/Login.unity` 已存在，并已加入 Build Settings。
- [ ] `Assets/Scenes/Game.unity` 或 SpaceShooter 入口场景已存在，并已加入 Build Settings。
- [ ] Boot 场景能启动 Framework/Patch。
- [ ] App/Core 能在热更失败时显示兜底错误提示。
- [ ] HybridCLR AOT 元数据和热更 DLL 能加载成功。
- [ ] Patch UI 能显示状态、错误、下载进度和确认按钮。
- [ ] Patch 完成后先加载热更 DLL，再进入 Loading，而不是直接进入 Login。
- [ ] Loading 能根据目标进入 Login 或游戏入口。
- [ ] Login 场景包含 LoginController 和 LoginView。
- [ ] Login 默认来自可热更 DLL，且有随包兜底失败提示。
- [ ] LoginView 已绑定输入框、按钮和消息文本。
- [ ] 游戏入口场景能进入 SpaceShooter 玩法逻辑。
- [ ] SpaceShooter 资源引用迁移后无缺失。
- [ ] Sample 目录仍可作为来源对照。

## 风险与注意点

- Unity 资源迁移容易造成 prefab、scene、material、sprite atlas 引用断裂，需要通过 Unity Editor 执行或校验迁移。
- `Assets/Scripts/Framework/Game/` 当前命名可能混淆，需要判断它是 App 壳层还是 SpaceShooter 玩法逻辑。
- Patch 层不能继续吸收 Login、Loading、SpaceShooter 玩法逻辑，否则 Framework 会失去复用性。
- App 层不应包含 SpaceShooter 玩法细节，只负责选择和进入目标内容。
- Login 默认走热更，但不能没有随包兜底，否则热更 DLL 或资源包损坏时无法提示用户。
- Framework 作为热更流程核心，不应依赖 HotUpdate DLL 才能完成 patch 和失败恢复。
- Sample 目录如果继续被误用为开发目录，会导致主工程结构再次分叉。

## 结论

当前最合适的策略是：

- 继续保留并维护 `Assets/Scripts/Framework/Patch/` 作为公共 Patch 框架。
- 新增 `Assets/Scripts/App/Core/` 承载 Boot、兜底 UI、场景流和热更 DLL 加载入口。
- 新增 `Assets/Scripts/App/HotUpdate/` 承载默认可热更的 Login 和正式 Loading。
- 将 SpaceShooter 游戏内容正式迁到 `Assets/Games/SpaceShooter/`。
- 玩法逻辑和 App/Login 逻辑走 HybridCLR DLL 全量更新，UI、动画、配置等走 YooAsset 资源分组更新。
- 将 sample 目录作为来源基线保留，而不是长期开发目录。
- 以主工程 `Assets/Scenes/Boot.unity` 为正式运行入口，保证调整架构后可以直接运行游戏。
