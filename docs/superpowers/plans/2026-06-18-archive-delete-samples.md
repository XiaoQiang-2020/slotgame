# Archive Delete Samples Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Remove the migrated YooAsset SpaceShooter sample from the active Unity project while preserving the formal `Framework + App + Games/SpaceShooter` runtime path.

**Architecture:** The active project uses `Assets/Scripts/Framework`, `Assets/Scripts/App`, `Assets/Games/SpaceShooter`, `Assets/Scenes`, and `Assets/Resources/PatchWindow.prefab` as authoritative paths. The old sample subtree is deleted from `Assets/Samples` after confirming no runtime, Build Settings, YooAsset collector, or documentation dependency still treats it as active source. Historical comparison moves to git history or a clean external YooAsset sample import outside the active project.

**Tech Stack:** Unity 2022.3 LTS, YooAsset 3.0.2-beta, UniFramework.Event, UniFramework.Machine, HybridCLR retained as optional future capability, UGUI, C#.

## Global Constraints

- Delete only `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/` and its `.meta`.
- If `Assets/Samples/YooAsset/3.0.2-beta/` becomes empty, remove the empty parent folders and their `.meta` files.
- Do not delete unrelated YooAsset samples if they exist under `Assets/Samples/YooAsset/3.0.2-beta/`.
- Do not enable or move `Assets/Games/SpaceShooter/GameScriptSource~` in this cleanup pass.
- Do not include unrelated dirty files such as `Assets/Editor/HotfixPipeline.cs` or `docs/plans.md` in commits.
- Keep formal scene order as `Assets/Scenes/Boot.unity`, `Assets/Scenes/Login.unity`, `Assets/Scenes/Game.unity`.
- Keep `Assets/Resources/PatchWindow.prefab` as the only approved `Resources.Load` fallback UI for Boot/Patch.
- Framework must not reference App or Games concrete business classes.
- Default C# strategy remains bundled logic for Framework, App/Login, and Games/SpaceShooter.
- YooAsset remains the normal update path for UI prefabs, images, audio, animation, config, level tables, and SpaceShooter content resources.
- HybridCLR/AOT tooling remains available, but default startup must not load `HotUpdate.dll` or AOT metadata.

---

## File Structure

### Delete

- `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/`
  - Migrated sample subtree removed from the active Unity project.
- `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter.meta`
  - Unity folder metadata removed with the folder.
- Empty parents only if they become empty:
  - `Assets/Samples/YooAsset/3.0.2-beta/`
  - `Assets/Samples/YooAsset/3.0.2-beta.meta`
  - `Assets/Samples/YooAsset/`
  - `Assets/Samples/YooAsset.meta`
  - `Assets/Samples/`
  - `Assets/Samples.meta`

### Modify

- `docs/space_shooter_integration_plan.md`
  - Replace "sample retained as source baseline in active project" language with "migration complete; active project no longer keeps the sample; use git history or external YooAsset package for comparison."
- `docs/unity_client_development_standard_v2_revision.md`
  - Update project directory rules so `Assets/Samples/.../SpaceShooter` is not listed as a long-term in-project baseline.
- `docs/superpowers/plans/2026-06-18-space-shooter-integration.md`
  - Add execution note that the archive deletion plan supersedes previous "keep sample source read-only" follow-up.
- `docs/superpowers/specs/2026-06-18-archive-delete-samples-design.md`
  - Stage with the plan so the approved design is preserved in git history.

### Preserve

- `Assets/Games/SpaceShooter/GameRes/`
  - Formal YooAsset-managed SpaceShooter resources.
- `Assets/Games/SpaceShooter/GameSetting/`
  - Formal YooAsset collector configuration.
- `Assets/Games/SpaceShooter/Scripts/README.md`
  - Active scripts directory note.
- `Assets/Games/SpaceShooter/GameScriptSource~/`
  - Non-compiled migrated source parking area. This cleanup does not activate it.
- `Assets/Resources/PatchWindow.prefab`
  - Built-in patch fallback UI.
- `Assets/Scenes/Boot.unity`, `Assets/Scenes/Login.unity`, `Assets/Scenes/Game.unity`
  - Formal scene flow.

---

### Task 1: Capture Pre-Delete State

**Files:**
- Read: `Assets/Samples/YooAsset/3.0.2-beta/`
- Read: `Assets/Games/SpaceShooter/`
- Read: `ProjectSettings/EditorBuildSettings.asset`

**Interfaces:**
- Consumes: current Unity project tree and git worktree.
- Produces: verified deletion scope and a list of unrelated dirty files to avoid staging.

- [ ] **Step 1: Check git status**

Run:

```powershell
git status --short
```

Expected:

```text
?? docs/superpowers/specs/
?? docs/superpowers/plans/2026-06-18-archive-delete-samples.md
```

If additional files appear, write their paths into the task notes and do not stage them unless they are part of this plan.

- [ ] **Step 2: Confirm sample subtree exists**

Run:

```powershell
Get-ChildItem -Force Assets\Samples\YooAsset\3.0.2-beta
```

Expected: `SpaceShooter` directory and `SpaceShooter.meta` exist. If other directories exist, they are unrelated samples and must remain.

- [ ] **Step 3: Confirm formal SpaceShooter paths exist**

Run:

```powershell
Get-ChildItem -Force Assets\Games\SpaceShooter
```

Expected includes:

```text
GameRes
GameSetting
GameScriptSource~
Scripts
```

- [ ] **Step 4: Confirm Build Settings do not point to sample scenes**

Run:

```powershell
rg -n "Assets/Samples|Boot\.unity|Login\.unity|Game\.unity" ProjectSettings\EditorBuildSettings.asset
```

Expected includes only:

```text
Assets/Scenes/Boot.unity
Assets/Scenes/Login.unity
Assets/Scenes/Game.unity
```

Expected: no `Assets/Samples/.../Boot.unity` entry.

---

### Task 2: Scan Runtime and Configuration References

**Files:**
- Read: `Assets/Scripts/`
- Read: `Assets/Editor/`
- Read: `Assets/Games/SpaceShooter/`
- Read: `ProjectSettings/`
- Read: `docs/`

**Interfaces:**
- Consumes: current runtime, editor, config, and docs references.
- Produces: a pass/fail gate before deleting sample files.

- [ ] **Step 1: Scan for active sample path references**

Run:

```powershell
rg -n "Assets/Samples|Samples/YooAsset|SpaceShooter/Resources/PatchWindow.prefab|SpaceShooter/GameScript" Assets/Scripts Assets/Editor Assets/Games ProjectSettings docs
```

Expected before doc cleanup: matches may exist in docs and historical plans only.

Expected failure condition: any match in `Assets/Scripts`, `Assets/Editor`, `Assets/Games`, or `ProjectSettings` that treats `Assets/Samples/.../SpaceShooter` as an active runtime path.

- [ ] **Step 2: Scan old startup and HybridCLR default path references**

Run:

```powershell
rg -n "Framework\.Scene|Framework\.Login|Framework\.Game|LoadingTarget|SceneNames\.Loading|AddNode<FsmLoadMetadata>|ChangeState<FsmLoadMetadata>" Assets/Scripts Assets/Editor Assets/Games ProjectSettings
```

Expected: no matches.

- [ ] **Step 3: Scan Resources.Load usage**

Run:

```powershell
rg -n "Resources\.Load" Assets/Scripts Assets/Games
```

Expected: only `Assets/Scripts/App/Boot/AppBoot.cs` loads `PatchWindow`.

- [ ] **Step 4: Stop if active references remain**

If Step 1, Step 2, or Step 3 finds an active runtime/config reference to the sample subtree, do not delete files. Fix the reference first or write a focused follow-up plan for that reference.

---

### Task 3: Delete the Archived Sample Subtree

**Files:**
- Delete: `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/`
- Delete: `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter.meta`
- Delete only if empty: `Assets/Samples/YooAsset/3.0.2-beta/`
- Delete only if empty: `Assets/Samples/YooAsset/3.0.2-beta.meta`
- Delete only if empty: `Assets/Samples/YooAsset/`
- Delete only if empty: `Assets/Samples/YooAsset.meta`
- Delete only if empty: `Assets/Samples/`
- Delete only if empty: `Assets/Samples.meta`

**Interfaces:**
- Consumes: pass result from Task 2.
- Produces: active Unity project without the migrated sample subtree.

- [ ] **Step 1: Remove the sample folder and folder meta**

Run:

```powershell
git rm -r Assets\Samples\YooAsset\3.0.2-beta\SpaceShooter Assets\Samples\YooAsset\3.0.2-beta\SpaceShooter.meta
```

Expected: git stages deletions for the sample subtree and `SpaceShooter.meta`.

- [ ] **Step 2: Check whether `3.0.2-beta` is empty**

Run:

```powershell
Get-ChildItem -Force Assets\Samples\YooAsset\3.0.2-beta
```

Expected: no output if `SpaceShooter` was the only sample.

If there is output, skip Steps 3 through 5 and preserve remaining samples.

- [ ] **Step 3: Remove empty `3.0.2-beta` folder and meta**

Run only if Step 2 produced no output:

```powershell
git rm -r Assets\Samples\YooAsset\3.0.2-beta Assets\Samples\YooAsset\3.0.2-beta.meta
```

Expected: git stages deletion for the empty version folder and meta.

- [ ] **Step 4: Remove empty `YooAsset` folder and meta**

Run:

```powershell
Get-ChildItem -Force Assets\Samples\YooAsset
```

If there is no output, run:

```powershell
git rm -r Assets\Samples\YooAsset Assets\Samples\YooAsset.meta
```

Expected: git stages deletion only when `YooAsset` is empty.

- [ ] **Step 5: Remove empty `Samples` folder and meta**

Run:

```powershell
Get-ChildItem -Force Assets\Samples
```

If there is no output, run:

```powershell
git rm -r Assets\Samples Assets\Samples.meta
```

Expected: git stages deletion only when `Assets/Samples` is empty.

- [ ] **Step 6: Verify no unstaged sample files remain**

Run:

```powershell
git status --short Assets\Samples
```

Expected: only staged deletions under `Assets/Samples`, or no output if the parent folder no longer exists.

---

### Task 4: Update Architecture Documentation

**Files:**
- Modify: `docs/space_shooter_integration_plan.md`
- Modify: `docs/unity_client_development_standard_v2_revision.md`
- Modify: `docs/superpowers/plans/2026-06-18-space-shooter-integration.md`

**Interfaces:**
- Consumes: archive-delete design decision.
- Produces: docs that no longer instruct developers to keep or use `Assets/Samples/.../SpaceShooter` as an active in-project baseline.

- [ ] **Step 1: Update `docs/space_shooter_integration_plan.md` sample positioning**

Replace the earlier "Sample 保持可回溯" position with:

```markdown
4. **Sample 已归档删除**
   - YooAsset SpaceShooter sample 的迁移已经完成，`Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter` 不再保留在活跃工程中。
   - 后续开发统一在 `Assets/Games/SpaceShooter/` 进行。
   - 如需对照官方 demo 行为，使用 git 历史或在工程外重新导入 YooAsset 官方 sample。
```

- [ ] **Step 2: Update `docs/space_shooter_integration_plan.md` directory tree**

Remove this block from the directory tree:

```text
  Samples/
    YooAsset/
      3.0.2-beta/
        SpaceShooter/
```

Add this note below the directory tree:

```markdown
`Assets/Samples/.../SpaceShooter` 已从活跃工程删除；它只作为 git 历史或外部官方包中的历史参考存在。
```

- [ ] **Step 3: Update `docs/space_shooter_integration_plan.md` implementation notes**

Append this note to the implementation progress section:

```markdown
- Archive deletion decision: after migration and Unity/YooAsset validation, `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter` is removed from the active project. Future SpaceShooter work must use `Assets/Games/SpaceShooter`; original sample comparison comes from git history or an external YooAsset sample import.
```

- [ ] **Step 4: Update `docs/unity_client_development_standard_v2_revision.md` directory rules**

Replace the `Assets/Samples/.../SpaceShooter` rule with:

```markdown
- `Assets/Samples/.../SpaceShooter/`：迁移完成后不再保留在活跃工程中。需要对照官方 sample 时，使用 git 历史或工程外的 YooAsset 官方 sample 包。
```

- [ ] **Step 5: Update `docs/unity_client_development_standard_v2_revision.md` current implementation note**

Replace the note about `GameScriptSource~` being temporary while sample scripts exist with:

```markdown
- `Assets/Games/SpaceShooter/GameScriptSource~` is a temporary non-compiled migration source. Since the active sample subtree is archived out of the project, a later task can decide whether to enable these scripts, namespace them, or keep them parked until SpaceShooter gameplay entry wiring is finalized.
```

- [ ] **Step 6: Update previous integration plan execution notes**

Append to `docs/superpowers/plans/2026-06-18-space-shooter-integration.md`:

```markdown
- Follow-up archive cleanup plan created: `docs/superpowers/plans/2026-06-18-archive-delete-samples.md`. It supersedes the earlier "keep sample as in-project baseline" assumption after the user confirmed archive-style deletion.
```

- [ ] **Step 7: Verify no docs describe the sample as active**

Run:

```powershell
rg -n "保留.*sample|sample.*保留|来源基线|Assets/Samples/.*/SpaceShooter|长期开发目录" docs\space_shooter_integration_plan.md docs\unity_client_development_standard_v2_revision.md
```

Expected: matches only describe the deleted sample as historical reference through git history or external package.

---

### Task 5: Post-Delete Static Verification

**Files:**
- Read: `Assets/Scripts/`
- Read: `Assets/Editor/`
- Read: `Assets/Games/SpaceShooter/`
- Read: `ProjectSettings/`
- Read: `docs/`

**Interfaces:**
- Consumes: deleted sample subtree and updated docs.
- Produces: static confidence that Unity runtime and build configuration no longer depend on deleted files.

- [ ] **Step 1: Verify active code has no sample path references**

Run:

```powershell
rg -n "Assets/Samples|Samples/YooAsset|SpaceShooter/Resources/PatchWindow.prefab|SpaceShooter/GameScript" Assets/Scripts Assets/Editor Assets/Games ProjectSettings
```

Expected: no matches.

- [ ] **Step 2: Verify docs only contain historical references**

Run:

```powershell
rg -n "Assets/Samples|Samples/YooAsset" docs
```

Expected: matches only in historical plan/design docs or explicit notes saying the sample was archived out of the active project.

- [ ] **Step 3: Verify old flow references remain absent**

Run:

```powershell
rg -n "Framework\.Scene|Framework\.Login|Framework\.Game|LoadingTarget|SceneNames\.Loading|AddNode<FsmLoadMetadata>|ChangeState<FsmLoadMetadata>" Assets/Scripts Assets/Editor Assets/Games ProjectSettings
```

Expected: no matches.

- [ ] **Step 4: Verify only AppBoot uses Resources.Load**

Run:

```powershell
rg -n "Resources\.Load" Assets/Scripts Assets/Games
```

Expected: one match in `Assets/Scripts/App/Boot/AppBoot.cs` for `PatchWindow`.

- [ ] **Step 5: Verify formal SpaceShooter settings do not point to sample paths**

Run:

```powershell
rg -n "Assets/Samples|Samples/YooAsset" Assets\Games\SpaceShooter\GameSetting
```

Expected: no matches.

- [ ] **Step 6: Verify Build Settings scene order**

Run:

```powershell
rg -n "path:" ProjectSettings\EditorBuildSettings.asset
```

Expected includes:

```text
path: Assets/Scenes/Boot.unity
path: Assets/Scenes/Login.unity
path: Assets/Scenes/Game.unity
```

Expected: no `Assets/Samples` scene paths.

---

### Task 6: Unity Editor Verification

**Files:**
- Verify in Unity Editor: `Assets/Scenes/Boot.unity`
- Verify in Unity Editor: `Assets/Scenes/Login.unity`
- Verify in Unity Editor: `Assets/Scenes/Game.unity`
- Verify in Unity Editor: `Assets/Games/SpaceShooter/GameSetting/BundleCollectorSetting.asset`

**Interfaces:**
- Consumes: post-delete project tree.
- Produces: manual validation that Unity import, compile, scene flow, and YooAsset packaging still work.

- [ ] **Step 1: Let Unity import deletions**

Open or focus Unity Editor and wait for asset refresh.

Expected: Unity removes the deleted `Assets/Samples` subtree from the Project window.

- [ ] **Step 2: Check C# compile**

Wait for compilation to finish.

Expected: Console has no C# compile errors.

If errors mention missing SpaceShooter sample classes, stop. The next plan must decide whether to enable `Assets/Games/SpaceShooter/GameScriptSource~` or restore a needed script into an active formal directory.

- [ ] **Step 3: Build YooAsset package**

Run the same YooAsset build action the user already validated.

Expected: package build succeeds and collectors point to `Assets/Games/SpaceShooter/GameRes` / `Assets/Games/SpaceShooter/GameSetting`.

- [ ] **Step 4: Play from Boot scene**

Open `Assets/Scenes/Boot.unity` and press Play.

Expected:

```text
Boot -> Patch -> Login -> Game
```

Expected: `Assets/Resources/PatchWindow.prefab` appears during patch.

- [ ] **Step 5: Confirm no deleted sample asset is referenced**

Watch Unity Console while entering Play Mode and while building YooAsset.

Expected: no missing asset warnings that mention `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter`.

---

### Task 7: Stage and Commit Archive Deletion

**Files:**
- Stage: deleted `Assets/Samples` subtree and empty parents if applicable.
- Stage: `docs/superpowers/specs/2026-06-18-archive-delete-samples-design.md`
- Stage: `docs/superpowers/plans/2026-06-18-archive-delete-samples.md`
- Stage: `docs/space_shooter_integration_plan.md`
- Stage: `docs/unity_client_development_standard_v2_revision.md`
- Stage: `docs/superpowers/plans/2026-06-18-space-shooter-integration.md`

**Interfaces:**
- Consumes: successful static and Unity verification.
- Produces: focused git commit for archive-style sample deletion.

- [ ] **Step 1: Review full status**

Run:

```powershell
git status --short
```

Expected: only sample deletions and docs from this plan are staged or unstaged for this commit.

- [ ] **Step 2: Stage only plan-owned files**

Run:

```powershell
git add docs\superpowers\specs\2026-06-18-archive-delete-samples-design.md docs\superpowers\plans\2026-06-18-archive-delete-samples.md docs\space_shooter_integration_plan.md docs\unity_client_development_standard_v2_revision.md docs\superpowers\plans\2026-06-18-space-shooter-integration.md
```

Sample deletions should already be staged by `git rm`. If not, stage them with:

```powershell
git add -u Assets\Samples
```

- [ ] **Step 3: Confirm unrelated files are not staged**

Run:

```powershell
git diff --cached --name-only
```

Expected: no `Assets/Editor/HotfixPipeline.cs`, no `docs/plans.md`, and no unrelated generated files.

- [ ] **Step 4: Commit**

Run:

```powershell
git commit -m "chore: archive delete spaceshooter sample"
```

Expected: commit succeeds.

---

## Self-Review

### Spec Coverage

- Archive-style deletion of `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/`: covered by Task 3.
- Remove empty parent folders only when empty: covered by Task 3 Steps 2 through 5.
- Do not delete unrelated YooAsset samples: covered by Global Constraints and Task 3 Step 2.
- Update docs away from "sample retained as baseline": covered by Task 4.
- Preserve formal `Assets/Games/SpaceShooter` structure: covered by File Structure and Task 1.
- Keep `GameScriptSource~` non-compiled for now: covered by Global Constraints and Task 6 Step 2 failure handling.
- Verify no runtime or ProjectSettings references remain: covered by Tasks 2 and 5.
- Unity Editor compile, YooAsset build, and Boot flow validation: covered by Task 6.
- Avoid unrelated dirty files in commit: covered by Global Constraints and Task 7.

### Placeholder Scan

No task uses placeholder language such as TBD, TODO, "implement later", or "write tests for the above." Each verification step includes exact commands and expected results.

### Type and Path Consistency

- Deleted sample path is consistently `Assets/Samples/YooAsset/3.0.2-beta/SpaceShooter/`.
- Formal game path is consistently `Assets/Games/SpaceShooter/`.
- Formal scene paths are consistently `Assets/Scenes/Boot.unity`, `Assets/Scenes/Login.unity`, and `Assets/Scenes/Game.unity`.
- Patch fallback path is consistently `Assets/Resources/PatchWindow.prefab`.
