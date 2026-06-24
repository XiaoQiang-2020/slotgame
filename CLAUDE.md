# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

This is a Unity project (no CLI build scripts are configured). All builds and play-mode runs go through the Unity Editor. Open the project in Unity and press Play from the Boot scene (`Assets/Scenes/Boot.unity`).

YooAsset play mode is set on the `AppBoot` component in the Boot scene. During editor development use `EditorSimulateMode`; set `HostPlayMode` for CDN-based hot-update testing.

## Architecture

### Boot flow

`AppBoot` (Boot scene) is the single entry point:

1. Initializes `UniEvent` and `YooAssets`.
2. Calls `AppGameManager.InitializeHost()` — sets screen/frame-rate, starts `SystemStartup`, initializes AppsFlyer SDK.
3. Calls `AppGameManager.RequestStartupAuthorize()` — runs `StartupAuthorize` async; on success triggers the patch flow.
4. `PatchFacade.Create/Start()` runs the YooAsset patch FSM (see `Assets/Scripts/Framework/Patch/`).
5. On `PatchCompletedEvent`, sets the YooAsset default package on `GameManager` and calls `SceneNavigator.LoadLoginScene()`.

Scene progression: **Boot → Login → Game**. Scene names are constants in [SceneNames.cs](Assets/Scripts/App/Scene/SceneNames.cs).

### Layer structure

| Path | Purpose |
|---|---|
| `Assets/Scripts/App/` | App-level orchestration: Boot, Login, Game controllers/models, SceneNavigator |
| `Assets/Scripts/Framework/Patch/` | YooAsset hot-update FSM (8 states from initialize to start-game) |
| `Assets/Scripts/Managers/` | `AppGameManager`, `SystemStartup`, `ResourceManager`, `AppsFlyerManager` |
| `Assets/Scripts/Engine/` | Core utilities: `Core.Singleton<T>`, `Core.Coroutine`, Timer, LoomMain (thread dispatch) |
| `Assets/Scripts/MVC/` | Thin MVC base classes: `BaseController`, `BaseView`, `BaseModel` |
| `Assets/Scripts/Game/` | Shared game utilities: UI helpers, effects, XML parsers, RC4 crypto, resource update threads |
| `Assets/Scripts/SDK/` | Third-party SDK wrappers: AppsFlyer, UniWebView, Umeng, iOS bridge |
| `Assets/Scripts/Utils/` | `MonoSingleton<T>` and `Singleton<T>` duplicates (prefer `Utils.*` in new code) |
| `Assets/Scripts/Log/` | `LogDebug` — project-level logging wrapper |
| `Assets/Games/SpaceShooter/` | SpaceShooter gameplay module (current active game) |

### Singleton patterns

- **`Core.Singleton<T>`** — thread-safe non-MonoBehaviour singleton; access via `T.Instance()` (method call).
- **`Utils.MonoSingleton<T>`** — MonoBehaviour singleton; access via `T.Instance` (property). Used for all manager classes that need Unity lifecycle.

### Key packages

- **YooAsset 3.0.2-beta** (`com.tuyoogame.yooasset`) — asset bundle management and hot-update. All runtime asset loading goes through `YooAssets.GetPackage("DefaultPackage")`.
- **HybridCLR** (`com.code-philosophy.hybridclr`) — hot code update support via IL2CPP.
- **UniFramework** (event + state machine) — used by the patch FSM and event bus (`UniEvent`).
- **AppsFlyer** — attribution/analytics SDK, initialized in `AppGameManager.InitializeHost()`.
- **UniWebView** — in-game webview, wrappers under `Assets/Scripts/SDK/UniWebView/`.

### Runtime config

`Assets/Resources/app_basic_config.json` — controls resource server URL, encryption flags, and debug toggles. Read at runtime; `is_res_mode_debug: true` disables encryption in dev.

### Scene root nodes

`AppGameManager` creates and holds references to three persistent root GameObjects (tagged by `GlobalVar` constants): a scene root, a UI canvas root, and a share-canvas root. All persistent GameObjects use the `DontDestroyObject` component.
