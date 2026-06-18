# Archive Delete Samples Design

## Goal

Remove the migrated YooAsset SpaceShooter sample from the active Unity project after its runtime responsibilities have moved into formal project directories.

This is an archive-style deletion: the project will no longer keep the sample in `Assets/Samples`, and historical reference will come from git history or the original YooAsset package.

## Current State

- Runtime startup uses `Assets/Scenes/Boot.unity`, `Login.unity`, and `Game.unity`.
- Patch fallback UI uses `Assets/Resources/PatchWindow.prefab`.
- Patch framework code lives under `Assets/Scripts/Framework/Patch`.
- App flow code lives under `Assets/Scripts/App`.
- SpaceShooter resources and YooAsset collector settings live under `Assets/Games/SpaceShooter`.
- Sample `GameSetting` collector files have already been removed because they conflicted with the formal `Assets/Games/SpaceShooter/GameSetting` configuration.

## Deletion Scope

Delete the migrated sample subtree:

```text
Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/
```

If `Assets/Samples/YooAsset/3.0.2-beta/` becomes empty after deleting `SpaceShooter`, remove empty parent folders and their `.meta` files. Do not delete unrelated YooAsset samples if they exist under the same parent.

## What Remains Authoritative

- Formal app architecture: `Assets/Scripts/Framework`, `Assets/Scripts/App`, `Assets/Games/SpaceShooter`.
- Formal resource update config: `Assets/Games/SpaceShooter/GameSetting`.
- Formal scenes: `Assets/Scenes`.
- Historical sample reference: git history and external YooAsset sample package.

## Documentation Updates

Update project docs to replace "sample is retained as baseline" with:

- The YooAsset SpaceShooter sample has completed migration.
- `Assets/Samples/.../SpaceShooter` is no longer kept in the active project.
- Future development must use `Assets/Games/SpaceShooter`.
- If original sample comparison is needed, use git history or re-import the official YooAsset sample outside the active project.

## Validation

After deletion:

1. Run a reference scan:

```powershell
rg -n "Assets/Samples|Samples/YooAsset|SpaceShooter/Resources/PatchWindow.prefab|SpaceShooter/GameScript" Assets/Scripts Assets/Editor Assets/Games ProjectSettings docs
```

Expected result: no runtime or ProjectSettings references. Documentation references may remain only when explicitly describing historical migration.

2. Run boundary scans:

```powershell
rg -n "Framework\\.Scene|Framework\\.Login|Framework\\.Game|LoadingTarget|SceneNames\\.Loading|AddNode<FsmLoadMetadata>|ChangeState<FsmLoadMetadata>" Assets/Scripts Assets/Editor Assets/Games ProjectSettings
rg -n "Resources\\.Load" Assets/Scripts Assets/Games
```

Expected result: no old Loading/default HybridCLR path references, and only `AppBoot` loads `PatchWindow`.

3. Verify in Unity Editor:

- C# compile passes.
- YooAsset package build still succeeds.
- Play from `Assets/Scenes/Boot.unity` reaches Patch, Login, and Game.

## Risks

- Some copied SpaceShooter scripts are currently parked in `Assets/Games/SpaceShooter/GameScriptSource~` to avoid duplicate global classes while sample scripts exist. After deleting the sample, a follow-up can decide whether to enable those scripts directly or keep them as archived source until gameplay entry is wired.
- Deleting sample assets removes convenient side-by-side comparison inside Unity. Git history becomes the comparison source.
