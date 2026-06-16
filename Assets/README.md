Assets 用法说明

- 将本目录复制到 Unity 项目根目录下的 `Assets/` 中。
- `Assets/Scripts/MVC` 包含基础的 MVC 顶层抽象：`BaseModel`、`BaseView`、`BaseController`。
- `Assets/Scripts/Utils` 包含一个 `MonoSingleton<T>`（供需要挂载到场景的管理器使用）和一个非 Mono 的 `Singleton<T>`（用于纯逻辑单例）。
- `Assets/Scripts/Game/GameEntry.cs` 为游戏入口示例，请挂载到场景中的空 GameObject 上，以便在 `Start` 时执行初始化。
- 示例 `GameManager` 演示如何继承非 Mono 单例并在 `GameEntry` 中调用初始化。

下一步我可以：
- 生成示例 Controller/View/Model 的简单实现; 或
- 创建场景文件（仅文件结构和说明，实际 Unity 场景需在 Unity 编辑器中创建）。

请回复你想要下一步做什么：`示例MVC` 或 `场景说明`。