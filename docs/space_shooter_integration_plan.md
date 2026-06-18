# SpaceShooter + Framework/App 集成子计划

## 目标

以 YooAsset 官方 `SpaceShooter` demo 为基础，完成超级休闲游戏项目的第一条可运行内容线。项目不是大厅加多个子模式的架构，而是一款以轻量玩法快速迭代为目标的单游戏。当前方向不再是“直接引用 sample 原样运行”，而是采用 **Framework 抽取版 + App 主流程 + Game 内容目录**：

- 将 YooAsset patch、状态机、事件桥接等公共能力抽取到主工程 `Framework` 层。
- 将 Boot、Login、场景流转等应用主流程放到 `App` 层。
- 将 SpaceShooter 的玩法、资源、场景和 UI 迁移到正式游戏内容目录。
- 保留官方 sample 作为来源对照和回溯基线，但不再作为长期开发目录。

最终目标是：调整代码架构后，直接运行主工程入口即可完成 `Boot -> Patch(资源版本校验与下载) -> Login -> Game` 的基础流程。

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
   - 放 Boot/Startup、Login、SceneNavigator、主流程编排和固定公共配置。
   - 负责串起补丁更新、登录、进入正式游戏内容。

3. `Assets/Games/SpaceShooter/`
   - 第一条轻量休闲游戏内容线。
   - 放从 sample 迁出的 SpaceShooter 玩法脚本、Home/Battle、游戏 UI、游戏资源、游戏场景。
   - 后续玩法迭代、资源替换、游戏逻辑修改以这里为准。

## 设计原则

1. **公共能力抽到 Framework**
   - Patch 流程、事件定义、状态机节点、资源加载封装属于公共能力。
   - Framework 层应保持可复用，避免绑定 SpaceShooter 的 Home/Battle/飞船战斗等概念。

2. **主流程放到 App**
   - Boot、Login、场景跳转不是底层框架，也不是玩法独有逻辑。
   - 它们属于主应用壳层，统一放到 `Assets/Scripts/App/`。
   - 当前默认 App 层随包发布，不依赖 HybridCLR 逻辑热更。

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
   - App 和 Games/SpaceShooter 的 C# 业务逻辑默认随包发布。
   - 游戏 UI prefab、动画、图片、音效、配置资源走 YooAsset 资源分组更新。
   - HybridCLR/AOT 工程能力保留，可作为线上逻辑热更通道按开关启用，但不作为默认主流程依赖。

## 目标目录规划

```text
Assets/
  Scenes/
    Boot.unity
    Login.unity
    Game.unity

  Resources/
    PatchWindow.prefab

  Scripts/
    Framework/
      Patch/
      MVC/
      Resource/
      Utils/

    App/
      Boot/
      Login/
      Scene/
      Config/

  Games/
    SpaceShooter/
      Scripts/
        GameLogic/
        BattleLogic/
        WindowLogic/
        Behaviour/
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
- `Assets/Resources/PatchWindow.prefab` 是 Boot/Patch 阶段随包兜底 UI，来源可参考 `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/Resources/PatchWindow.prefab`。
- `Assets/Scripts/App/` 负责启动、登录、场景跳转、公共配置和主流程。
- `Assets/Scripts/App/Config/` 负责公共配置定义，配置数据可区分内置兜底和远端更新版本。
- `Assets/Games/SpaceShooter/` 是正式游戏内容目录，不再把 sample 目录作为开发主线。
- `Assets/Samples/.../SpaceShooter` 保留为官方导入版本和迁移来源。

## 模块职责

### Framework/Patch

来源于 YooAsset SpaceShooter demo 的 patch 流程抽取，负责：

- 使用 App/Boot 已初始化的 YooAssets，驱动资源包初始化和 patch 状态机。
- 创建和驱动 PatchManager 状态机。
- 保留 UniFramework.Event 事件机制。
- 处理包初始化、版本请求、Manifest 更新、下载、缓存清理。
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
- `FsmClearCacheBundle`
- `FsmStartGame`

HybridCLR 逻辑热更启用时可额外接入：

- `FsmLoadMetadata`
- 热更 DLL 加载状态或服务

边界要求：

- Framework/Patch 可以使用 YooAsset、UniEvent、UniMachine。
- Framework/Patch 不直接引用 SpaceShooter 的 Home/Battle/GameLogic。
- Patch 完成后只发出通用完成事件，不直接决定进入具体玩法场景。
- Framework/Patch 原则上随包发布，不作为常规热更目标。

### App

应用壳层，随包发布，负责：

- 正式启动入口 `Boot`。
- 设置帧率和后台运行。
- 初始化 UniEvent。
- 初始化 YooAssets。
- 创建并启动 Framework/Patch。
- 在 Boot 场景内通过随包 `PatchWindow.prefab` 挂载 patch UI。
- 提供 Login 固定流程。
- 提供场景跳转和主流程编排。
- 提供公共配置读取和兜底错误提示。

`Boot.unity` 应迁到 `Assets/Scenes/Boot.unity`，并加入 Build Settings 第一位。

Patch UI 不设计独立 Loading 场景。Boot 阶段直接从随包 `Resources` 加载 `PatchWindow.prefab`，参考 sample `Boot.cs` 中 `Resources.Load<GameObject>("PatchWindow")` 的方式；prefab 来源参考 sample 的 `SpaceShooter/Resources/PatchWindow.prefab`。该用法是 YooAsset 初始化前的兜底特例，不能扩展成正式游戏资源加载方式。

Login 逻辑当前随包发布，不放入 Framework，也不放进 SpaceShooter 游戏内容目录。Login 的 UI prefab、图片、动画、文案配置可走 YooAsset 资源分组更新。

### Games/SpaceShooter

SpaceShooter 是第一条游戏内容线，负责：

- Home/Battle 玩法逻辑。
- 游戏窗口 UI。
- 飞船、敌人、子弹、陨石、特效等玩法实体。
- 游戏资源、配置、场景。

迁移时应尽量保持 demo 原有结构，降低资源引用和脚本引用断裂风险。

玩法逻辑、数值配置读取、UI 控制逻辑当前随包发布。游戏 UI prefab、动画、图片、音效、关卡配置、数值表等资源走 YooAsset 资源分组更新。

### HybridCLR/AOT 保留策略

HybridCLR 不是删除项，也不是当前必须接入主流程的强依赖。项目现有的 `Assets/HotUpdate/`、`Assets/HybridCLRGenerate/`、`Assets/Editor/HotfixPipeline.cs`、AOT 元数据生成和 `HotUpdate.dll` 相关链路继续保留，作为随时可启用的逻辑热更能力。

默认发布策略是：Framework、App/Login、Games/SpaceShooter 的 C# 业务逻辑随包发布；Boot 主流程保持 `Boot -> Patch(资源版本校验与下载) -> Login -> Game`，不默认加载热更 DLL，也不把 AOT 元数据加载作为必经步骤。

如果线上出现必须改 C# 逻辑且重新发包成本过高的场景，可以按独立开关启用 HybridCLR。启用前需要单独设计并评审：

- 热更 DLL 加载入口。
- AOT 元数据加载。
- `link.xml` 和泛型实例保留策略。
- 热更层服务生命周期。
- 失败回滚和灰度策略。

当前 sample `GameManager.cs` 中已有 `RuntimeApi.LoadMetadataForAOTAssembly` 和 `HotUpdate.dll` 加载示例，应视为现有能力路径和参考实现。迁移到正式 `Assets/Games/SpaceShooter/` 时，如果默认随包逻辑即可满足需求，可以先用编译开关或入口开关让该路径不参与默认启动；如果确认启用逻辑热更，则应把这部分整理为 App 或 Framework 暴露的明确服务，而不是散落在玩法 `GameManager` 中。

## 全局服务与单例约束

项目允许使用少量全局服务或单例，但必须按层级限制职责，避免所有模块都通过 `Instance` 互相访问。

### 随包全局服务

随包全局服务放在 `Framework` 或 `App`，生命周期可以跨场景，允许使用 `DontDestroyOnLoad`。

适合放在随包层的服务：

- `PatchService` 或 `PatchFacade`
- `ResourceService`
- `SceneFlowService`
- `AppConfigService`
- `ErrorFallbackService`

约束：

- 必须能在资源更新失败、网络异常或资源包损坏时工作。
- 不直接依赖 `Games/SpaceShooter` 的具体玩法实现类。
- 可以通过接口、事件、Facade 或配置驱动游戏入口。
- 负责兜底流程、错误提示、资源更新等基础能力。

### 游戏运行期服务

游戏运行期服务放在 `App` 或 `Games/SpaceShooter`，由 App 主流程或游戏入口创建、初始化和销毁。

适合放在游戏运行期层的服务：

- `LoginService`
- `GameSession`
- `GameConfigService`
- `UIService`
- `AudioService`

约束：

- 可以使用单例或服务定位，但必须有显式 `Init` / `Dispose` 生命周期。
- 不允许被 `Framework` 直接引用。
- 不应承担 patch、资源更新、兜底错误提示等 Framework/App 基础职责。
- 退出游戏内容或重新加载热更域时，必须能清理事件监听、资源句柄和场景状态。

### 场景内控制器

场景内控制器默认不设计为单例，随场景创建和销毁。

适合保持场景生命周期的对象：

- `LoginController`
- `GameController`
- `BattleController`
- 各类 `View`

约束：

- 不使用 `DontDestroyOnLoad`。
- 不作为全局状态保存点。
- 跨场景数据通过 App 服务、游戏运行期服务或明确的数据模型传递。
- 销毁时要解绑按钮事件、UniEvent 监听和异步回调。

### 跨层通信原则

- `Framework` 可以暴露稳定接口和事件，但不引用 App 或 Games 的具体业务实现类。
- `App` 可以负责登录、主流程和游戏入口编排，但不写 SpaceShooter 玩法细节。
- `Games/SpaceShooter` 可以使用 App 和 Framework 暴露的公共能力，但玩法状态留在游戏内容层。
- 跨层通信优先使用接口、事件、Facade 或数据配置，不直接链式访问多个单例。

## 场景流程

目标流程：

```text
Boot
  -> Patch
  -> Login
  -> Game
```

阶段说明：

1. `Boot`
   - 正式入口场景。
   - 位于 `Assets/Scenes/Boot.unity`。
   - 负责实例化随包 `PatchWindow.prefab` 并启动 patch 流程。

2. `Patch`
   - 由 `Framework/Patch` 驱动。
   - 负责资源版本校验、Manifest 更新、资源下载和失败重试。
   - 完成后发布 `PatchCompletedEvent`。

3. `Login`
   - 应用级登录流程。
   - 当前随包发布。
   - UI prefab、图片、动画、文案配置可通过 YooAsset 更新。

4. `Game`
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
- 将现有 `Assets/Scripts/Framework/Login/` 迁到 `Assets/Scripts/App/Login/`。
- 将现有 `Assets/Scripts/Framework/Scene/` 迁到 `Assets/Scripts/App/Scene/`。
- 评估 `Assets/Scripts/Framework/Game/`：
   - 若是通用游戏入口壳层，迁到 `Assets/Scripts/App/Game/`。
   - 若是 SpaceShooter 玩法逻辑，迁到 `Assets/Games/SpaceShooter/Scripts/`。

### 2. 迁移 Boot 场景

- 将 sample `Boot.unity` 迁出或重建为 `Assets/Scenes/Boot.unity`。
- 绑定 App/Boot 或 Framework/Patch 启动脚本。
- 将 sample `Resources/PatchWindow.prefab` 迁到主工程随包 Resources 目录，作为 Boot/Patch 兜底 UI。
- Boot 阶段直接挂载 `PatchWindow.prefab`，不新增独立 Loading 场景。
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

### 3.1 明确资源更新分组

- 所有 C# 业务逻辑随包发布，包括 Framework、App/Login、Games/SpaceShooter。
- Login、SpaceShooter 的 prefab、图片、音效、动画、配置表走 YooAsset 资源分组。
- `PatchWindow.prefab` 和关键错误提示资源随包保留。
- 为资源更新设计分组、版本、下载失败重试和回滚策略。

### 3.2 规范全局服务

- 盘点现有 `Instance`、`DontDestroyOnLoad` 和静态全局入口。
- 将 patch、资源更新、兜底错误提示保留在 Framework 或 App。
- 将 UI、Audio、GameSession 等运行期服务放到 App 或 Games/SpaceShooter。
- 场景 Controller 和 View 不做全局单例。
- 为游戏运行期服务补充显式 `Init` / `Dispose` 生命周期。

### 4. 保留 sample 来源

- 保留 `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter`。
- 不再把它作为主工程启动入口。
- 后续如需要精简 sample，可在确认迁移完整后单独规划。

### 5. 更新场景跳转

- `PatchCompletedEvent` 触发进入 Login。
- `Login` 成功后进入 Game。
- 场景名统一在 App 场景常量中维护。
- App 启动链路不引入 `Loading.unity`；如 SpaceShooter 内部需要过渡 UI，可保留游戏内容层自己的 `UILoading` prefab。

### 6. 验证完整运行链

- 验证主工程从 `Assets/Scenes/Boot.unity` 启动。
- 验证 Boot 场景能实例化随包 `PatchWindow.prefab`。
- 验证 patch 流程可正常完成。
- 验证 YooAsset 资源版本校验、下载、失败重试可正常工作。
- 验证 patch 完成后进入 Login。
- 验证 Login 后进入 Game。
- 验证 SpaceShooter 资源和脚本引用不因迁移断裂。

## 最小可行任务拆分

- 任务 1：确认 `Framework/Patch` 已迁移代码与 sample patch 流程差异。
- 任务 2：创建 `Assets/Scripts/App/`，迁移 Boot、Login、Scene 和主流程代码。
- 任务 3：将现有 Login 场景干净嵌入 App 层。
- 任务 4：创建 `Assets/Games/SpaceShooter/` 并迁移游戏内容。
- 任务 5：将正式 Boot 场景迁到 `Assets/Scenes/Boot.unity`，并挂载随包 `PatchWindow.prefab`。
- 任务 6：移除 App 级 Loading 场景和 LoadingTarget 设计，修正 SceneNavigator/SceneNames 等引用。
- 任务 7：更新 Build Settings 场景顺序。
- 任务 8：建立 YooAsset 资源分组，支持 UI、图片、音效、动画、配置表资源更新。
- 任务 9：规范全局服务和单例生命周期。
- 任务 10：运行验证完整流程：Boot -> Patch -> Login -> Game。
- 任务 11：记录 sample 与主工程迁移后代码的差异点。

## 场景生成后绑定检查清单

- [ ] `Assets/Scenes/Boot.unity` 已存在，并已加入 Build Settings 第一位。
- [ ] `Assets/Scenes/Login.unity` 已存在，并已加入 Build Settings。
- [ ] `Assets/Scenes/Game.unity` 或 SpaceShooter 入口场景已存在，并已加入 Build Settings。
- [ ] Boot 场景能启动 Framework/Patch。
- [ ] Boot 场景能实例化随包 `PatchWindow.prefab`。
- [ ] App 能在资源更新失败时通过 PatchWindow 显示兜底错误提示。
- [ ] Patch UI 能显示状态、错误、下载进度和确认按钮。
- [ ] Patch 完成后进入 Login。
- [ ] Login 场景包含 LoginController 和 LoginView。
- [ ] LoginView 已绑定输入框、按钮和消息文本。
- [ ] 游戏入口场景能进入 SpaceShooter 玩法逻辑。
- [ ] SpaceShooter 资源引用迁移后无缺失。
- [ ] YooAsset 资源分组覆盖 UI、图片、音效、动画、配置表。
- [ ] 随包全局服务不依赖 SpaceShooter 具体玩法实现。
- [ ] 游戏运行期服务具备 Init/Dispose 生命周期。
- [ ] 场景 Controller/View 未设计为跨场景单例。
- [ ] Sample 目录仍可作为来源对照。

## 风险与注意点

- Unity 资源迁移容易造成 prefab、scene、material、sprite atlas 引用断裂，需要通过 Unity Editor 执行或校验迁移。
- `Assets/Scripts/Framework/Game/` 当前命名可能混淆，需要判断它是 App 壳层还是 SpaceShooter 玩法逻辑。
- Patch 层不能继续吸收 Login、App 级 Loading、SpaceShooter 玩法逻辑，否则 Framework 会失去复用性。
- App 层不应包含 SpaceShooter 玩法细节，只负责选择和进入目标内容。
- 资源更新失败时必须能使用随包兜底资源提示用户。
- 单例如果跨层互相直接引用，会破坏 Framework/App/Game 的边界，后续维护和资源更新会变难。
- Sample 目录如果继续被误用为开发目录，会导致主工程结构再次分叉。

## 结论

当前最合适的策略是：

- 继续保留并维护 `Assets/Scripts/Framework/Patch/` 作为公共 Patch 框架。
- 新增 `Assets/Scripts/App/` 承载 Boot、Login、场景流和公共配置。
- 不新增独立 Loading 场景；Boot 直接挂载随包 `PatchWindow.prefab` 完成 Patch 反馈和失败重试。
- 将 SpaceShooter 游戏内容正式迁到 `Assets/Games/SpaceShooter/`。
- C# 业务逻辑默认随包发布，UI、动画、音效、配置等走 YooAsset 资源分组更新。
- HybridCLR/AOT 工程能力保留，可随时按独立开关启用，但默认主流程不依赖热更 DLL。
- 将 sample 目录作为来源基线保留，而不是长期开发目录。
- 以主工程 `Assets/Scenes/Boot.unity` 为正式运行入口，保证调整架构后可以直接运行游戏。
