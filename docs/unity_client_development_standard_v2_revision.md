# Unity 客户端开发规范 V2.0 修订稿

适用项目：超级休闲游戏 / SpaceShooter 模板项目 / 美国市场 IAA 变现

本文基于 `Unity客户端开发规范V2.0(草稿).docx` 初稿和当前 `space_shooter_integration_plan.md` 的架构结论同步修订。目标是把通用客户端规范改成适配当前项目的可执行版本。

## 一、项目定位

当前项目不是大厅加多个子模式的结构，而是一款以轻量玩法快速迭代为核心的超级休闲游戏。

核心技术栈：

- Unity 2022.3 LTS
- YooAsset：资源更新、资源分组、Manifest、Patch 流程
- HybridCLR：现有 AOT/HotUpdate 工程能力保留，默认主流程不依赖，可按开关启用逻辑热更
- UniFramework.Event / UniFramework.Machine：事件和状态机
- MVC + 事件驱动：应用层和玩法层的主要组织方式
- Superpowers / Codex / AI Agent：用于设计、计划、代码生成、审查和文档维护

核心目标：

- 主工程入口可直接运行：`Boot -> Patch(资源版本校验与下载) -> Login -> Game`
- Framework 稳定随包，避免频繁热更
- App/Login 默认随包发布
- 游戏玩法 C# 逻辑默认随包发布
- UI prefab、动画、图片、音效、配置表走 YooAsset 资源分组更新

## 二、工程目录结构

当前项目目录规范以 `Framework + App + Games` 为主线。

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

  ThirdParty/
```

目录职责：

- `Assets/Scripts/Framework/`：公共底座，原则上随包发布，不作为常规热更目标。
- `Assets/Scripts/App/`：应用壳层，负责 Boot、Login、场景跳转、公共配置和兜底流程。
- `Assets/Scripts/App/Config/`：公共配置定义，区分内置兜底配置和远端更新配置。
- `Assets/Resources/PatchWindow.prefab`：Boot/Patch 阶段随包兜底 UI，来源可参考 sample `SpaceShooter/Resources/PatchWindow.prefab`，仅用于 YooAsset 初始化前的 patch 反馈和失败重试。
- `Assets/Games/SpaceShooter/`：正式游戏内容目录，承载玩法脚本、UI、资源、配置、场景。
- `Assets/Samples/.../SpaceShooter/`：官方 sample 来源基线，只做对照和回溯，不作为长期开发目录。
- `Assets/ThirdParty/`：第三方库和 SDK，只读，禁止直接修改源码。

强制规则：

- 新代码必须放入对应层级目录，禁止直接放在 `Assets/` 根目录。
- `Framework` 不允许引用 `App` 或 `Games/SpaceShooter` 的具体业务实现。
- `App` 可以包含 Login 和主流程，但不写 SpaceShooter 玩法细节。
- `Games/SpaceShooter` 可以使用 Framework 和 App 暴露的稳定能力，但玩法状态留在游戏内容层。

## 三、热更边界

### 随包发布

默认策略是所有 C# 业务逻辑随包发布，不作为常规热更目标：

- `Assets/Scripts/Framework/Patch/`
- `Assets/Scripts/Framework/Resource/`
- `Assets/Scripts/Framework/Utils/`
- `Assets/Scripts/App/`
- `Assets/Games/SpaceShooter/Scripts/`
- Boot/Patch 兜底 UI 和兜底错误提示 UI

这些模块必须在资源更新失败、资源包损坏、网络异常时仍能工作。

### YooAsset 资源更新

当前常规热更只启用资源热更。以下内容走 YooAsset 资源分组更新：

- UI prefab
- 图片、图集、字体
- 动画、Animator Controller
- 音效、BGM
- 关卡配置、数值表、公告配置
- SpaceShooter 游戏资源
- Login 的表现资源

约束：

- 禁止使用 `Resources.Load` 作为正式资源加载方式。
- 唯一例外是 Boot/Patch 阶段在 YooAsset 初始化前加载随包 `PatchWindow.prefab`。
- 资源必须进入明确的 YooAsset 包和 Collector 分组。
- Patch 流程必须能显示下载进度、失败提示和重试入口。
- 关键兜底资源应随包保留，避免更新失败后无法显示错误 UI。

### HybridCLR/AOT 保留能力

HybridCLR/AOT 能力在工程中保留，不删除现有目录、生成产物和工具链。它不是当前默认启动链路的强依赖，但可以在需要线上 C# 逻辑修复时通过独立开关启用。

默认主流程保持 `Boot -> Patch(资源版本校验与下载) -> Login -> Game`，Patch 只承担资源版本校验、Manifest 更新和 YooAsset 资源下载，不默认阻塞在热更 DLL 加载上。

App 启动链路不规划独立 `Loading.unity`。Boot 场景内直接实例化随包 `PatchWindow.prefab`，Patch 完成后进入 Login；游戏内容层如需过渡表现，可以保留自己的加载 UI prefab。

保留范围包括：

- `Assets/HotUpdate/` 热更代码目录。
- `Assets/HybridCLRGenerate/` AOT 泛型和 `link.xml` 生成结果。
- `Assets/Editor/HotfixPipeline.cs` 等打包辅助工具。
- AOT 元数据和 `HotUpdate.dll` 的生成、加载参考链路。

如果线上逻辑热更收益明确，再单独设计并评审：

- 热更 DLL 加载入口。
- AOT 元数据加载。
- `link.xml` 和泛型实例保留策略。
- 热更层服务生命周期。
- 失败回滚和灰度策略。

## 四、架构模式

项目采用 MVC + 事件驱动，并按目录边界区分公共底座、应用壳层和游戏内容层。

### Model

- 负责数据存储和状态管理。
- 不持有 View 引用。
- 不直接调用 SDK。
- 可序列化、可重建，适合随包逻辑和资源配置更新。

### View

- 负责 UI 渲染和用户输入。
- 不直接修改 Model。
- 不直接持有 Controller 的跨场景状态。
- 事件订阅必须在销毁时解绑。

### Controller

- 负责业务流程编排。
- 订阅 View 或事件，更新 Model，再驱动 View。
- 场景内 Controller 默认随场景销毁，不做全局单例。

### Framework

- 提供稳定公共能力。
- 可以暴露接口、事件、Facade。
- 不引用 App 或 Games 的具体业务实现。

## 五、全局服务与单例约束

允许使用少量全局服务，但必须按层级限制职责。

### 随包全局服务

放在 `Framework` 或 `App`，允许跨场景，允许 `DontDestroyOnLoad`。

适合：

- `PatchService` / `PatchFacade`
- `ResourceService`
- `SceneFlowService`
- `AppConfigService`
- `ErrorFallbackService`

要求：

- 必须能在资源更新失败时工作。
- 不直接依赖 `Games/SpaceShooter` 的具体玩法类。
- 通过接口、事件、Facade 或配置驱动游戏入口。

### 游戏运行期服务

放在 `App` 或 `Games/SpaceShooter`，由 App 主流程或游戏入口创建和销毁。

适合：

- `LoginService`
- `GameSession`
- `GameConfigService`
- `UIService`
- `AudioService`
- `AdStrategyService`

要求：

- 必须有显式 `Init` / `Dispose` 生命周期。
- 不允许被 `Framework` 直接引用。
- 退出场景或重新加载热更域时，必须清理事件监听、资源句柄和回调。

### 禁止事项

- 禁止所有系统都继承同一个宽泛 `Singleton<T>`。
- 禁止 View / Controller 做跨场景单例。
- 禁止链式访问多个单例，例如 `GameManager.Instance.UIManager.Config.X`。
- 禁止游戏运行期单例承担 Patch、资源更新、失败恢复职责。

## 六、AI / Superpowers 工作流

### Brainstorming 阶段

任何新系统、新功能、新目录迁移、新热更边界调整，都先进入 brainstorming：

- 先阅读项目上下文和现有文档。
- 一次只问一个澄清问题。
- 先提出 2-3 个方案和取舍。
- 用户确认设计后再进入实现计划。

### Writing Plans 阶段

设计确认后，生成实施计划：

- 明确文件路径。
- 明确随包代码和 YooAsset 资源分组边界。
- 明确 Unity Editor 手工步骤。
- 明确测试和验证命令。
- 明确 Prefab / Scene / Inspector 绑定清单。

### Unity 边界

AI 可以直接处理：

- `.cs` 代码
- `.json` / `.csv` 配置
- Markdown 文档
- 测试代码
- 构建脚本

AI 不应直接处理或需谨慎处理：

- `.unity` 场景
- `.prefab`
- Inspector 绑定
- YooAsset Collector Editor 配置
- HybridCLR Editor 菜单操作（保留工具链，启用逻辑热更时人工确认）
- 真机性能和广告 SDK 验证

这些内容由 AI 输出操作清单，人工在 Unity Editor 中执行和确认。

### 项目级约束文件

当前仓库未发现 `CLAUDE.md` / `AGENTS.md` / `GEMINI.md`。如果后续需要多 Agent 共用约束，建议创建中性的 `AGENTS.md`，内容同步本规范的强制规则。

建议写入 `AGENTS.md` 的核心约束：

- Unity 版本固定为 2022.3 LTS。
- 架构采用 `Framework + App + Games/SpaceShooter`。
- Framework 不依赖 App 或 Games 的具体业务实现。
- App/Login 默认随包发布。
- 游戏 C# 逻辑默认随包发布，资源走 YooAsset。
- Controller/View 默认不做跨场景单例。
- HybridCLR/AOT 工程能力保留，默认主流程不依赖热更 DLL，可按开关启用。

## 七、自定义 Skill 建议

### unity-mvc-codegen

触发：生成或修改 Model / View / Controller / Service 脚本。

规则：

- 必须说明目标路径和所属层级。
- View 不直接持有 Model。
- Controller 的事件订阅必须在销毁时解绑。
- `Update()` 内禁止 `new`、字符串拼接、LINQ、频繁 `GetComponent`。
- 生成 MonoBehaviour 后必须输出 Inspector 绑定清单。

### unity-hybridclr

默认不作为强制 Skill。修改现有 HybridCLR/AOT 工具链、热更 DLL 目录或启用逻辑热更时再打开。

触发：修改热更 DLL 目录、新增逻辑热更模块、调整 AOT 元数据生成或启用逻辑热更开关。

规则：

- 检查 AOT 泛型实例。
- 检查 `link.xml` 是否需要新增保留。
- 检查是否需要重新执行 HybridCLR Generate。
- 禁止热更代码重复初始化随包全局服务。
- 禁止热更代码承担 Patch 或失败恢复职责。

### unity-yooasset-handoff

触发：新增或修改资源、配置、UI prefab、动画、音频、关卡表。

规则：

- 输出 YooAsset 包和 Collector 建议。
- 标明资源是随包兜底还是远端更新。
- 输出资源加载路径和引用清单。
- 输出 Unity Editor 中需要人工确认的分组、标签、构建步骤。

### unity-prefab-handoff

触发：生成涉及 Prefab / Scene 绑定的脚本。

规则：

- 不直接改 Prefab / Scene。
- 输出挂载目标。
- 输出 `[SerializeField]` 字段绑定清单。
- 输出依赖组件和默认值。
- 输出运行前检查项。

## 八、广告与埋点

广告系统和埋点系统当前随包发布，SDK Adapter 和兜底错误处理应保持稳定。广告配置、频控参数和展示资源可以通过 YooAsset 配置资源更新。

建议分层：

- `Framework` 或 `App`：SDK Adapter 基础封装、初始化状态、错误兜底。
- `App`：广告展示策略、频控、运营参数、入口表现。
- `Games/SpaceShooter`：具体玩法内广告触发点。

广告策略：

- 激励视频由业务主动触发。
- 插屏频控可热更。
- D1 用户保护、冷却时间、每日上限应配置化。
- 广告展示、点击、奖励结果必须统一埋点。

埋点规则：

- 所有埋点统一走 `AnalyticsManager` 或等价 Adapter。
- 禁止业务层直接调用 Firebase / Adjust / AppsFlyer SDK。
- 事件名和参数保持集中定义，便于热更和数据分析。

## 九、性能与发布检查

性能目标：

- 真机帧率：目标 60 FPS。
- 启动时间：目标 2 秒内进入可反馈界面。
- 崩溃率：目标小于 0.3%。
- Patch 失败必须有用户可见提示和重试入口。
- 资源更新失败必须能通过随包 PatchWindow 或兜底错误界面提示用户。

发布前检查：

- `Boot -> Patch -> Login -> Game` 完整链路通过。
- Boot 场景能实例化随包 `PatchWindow.prefab`，不依赖独立 Loading 场景。
- YooAsset Manifest 更新、下载、失败重试通过。
- Login 随包逻辑可用，表现资源可通过 YooAsset 更新。
- SpaceShooter 资源引用无缺失。
- Controller/View 没有跨场景单例。
- 游戏运行期服务具备 `Init` / `Dispose`。
- 真机验证无主要 GC 尖峰、NullReferenceException、广告回调异常。

## 十、Git 与文档规范

- 架构和热更边界变化必须同步文档。
- 文档修改单独提交，不混入代码迁移。
- Unity 场景、Prefab、资源迁移需要单独提交或清晰拆分。
- AI 生成代码必须经过人工 Review。
- 提交前必须确认暂存区只包含本次目标文件。

建议提交类型：

- `docs:` 文档
- `feat:` 新功能
- `fix:` 修复
- `refactor:` 重构
- `config:` 配置
- `asset:` 资源
- `build:` 构建和管线

## 十一、与初稿的主要差异

初稿中的以下内容需要按本修订稿替换：

- `Assets/GameLauncher` 改为 `Assets/Scripts/App`。
- `Assets/Hotfix` 不作为默认业务开发主线；现有 HybridCLR/AOT 能力保留，游戏 C# 逻辑默认放在 `Assets/Scripts/App` 和 `Assets/Games/SpaceShooter/Scripts` 并随包发布。
- `Assets/Config` 改为 `Assets/Scripts/App/Config` 加 YooAsset 配置资源分组。
- `ArtAssets` 改为 YooAsset 管理下的游戏内容资源目录。
- `Addressables` 相关描述改为 YooAsset。
- 宽泛的 `Scripts/Core/Models/Views/Controllers` 改为 `Framework/App/Games` 分层。
- 单例规则改为随包全局服务、游戏运行期服务、场景控制器三类。
- `CLAUDE.md` 改为可选项目级约束文件，建议优先使用 `AGENTS.md` 作为多 Agent 通用入口。

## 十二、结语

这份修订稿的重点不是追求大而全，而是把当前项目真实采用的 YooAsset 资源热更 + Framework/App/Game 分层写清楚。HybridCLR/AOT 作为现有工程能力保留，默认主流程不依赖热更 DLL，但需要时可以按独立开关启用逻辑热更。

后续所有实现计划、AI Prompt、代码生成和 Unity Editor 操作，都应以本修订稿和 `space_shooter_integration_plan.md` 为准。
