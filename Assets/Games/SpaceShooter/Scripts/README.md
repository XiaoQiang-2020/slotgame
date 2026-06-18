# SpaceShooter Scripts

The migrated sample scripts are parked in `Assets/Games/SpaceShooter/GameScriptSource~` for the first pass.

Unity ignores `~` folders, which prevents duplicate global classes while the original sample scripts under
`Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/GameScript/Runtime` are still present.

Enable this folder only after one of these follow-up decisions is complete:

- remove the sample scripts from Unity compilation after all scene and prefab bindings are migrated; or
- add formal namespaces to the copied scripts and rebind copied prefabs/scenes in Unity Editor.
