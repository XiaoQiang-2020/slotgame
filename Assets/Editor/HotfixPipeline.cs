#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using HybridCLR.Editor.Commands;

public static class HotfixPipeline
{
    private static readonly string HotUpdateDllSrc =
        Path.GetFullPath(Path.Combine(Application.dataPath, "../Library/ScriptAssemblies/HotUpdate.dll"));
    private static readonly string HotUpdateDllDest =
        Path.Combine(Application.dataPath, "HotUpdateRes/Codes/HotUpdate.dll.bytes");

    [MenuItem("Hotfix/One-Click: Copy DLL + GenerateAOT")]
    public static void OneClick()
    {
        CopyHotUpdateDll();
        GenerateAOT();
        AssetDatabase.Refresh();
        Debug.Log("[HotfixPipeline] One-click complete.");
    }

    [MenuItem("Hotfix/Copy HotUpdate.dll to HotUpdateRes")]
    public static void CopyHotUpdateDll()
    {
        if (!File.Exists(HotUpdateDllSrc))
        {
            Debug.LogError($"[HotfixPipeline] Source DLL not found: {HotUpdateDllSrc}\nMake sure the project compiles in the Editor first.");
            return;
        }

        var destDir = Path.GetDirectoryName(HotUpdateDllDest);
        if (!Directory.Exists(destDir))
            Directory.CreateDirectory(destDir);

        File.Copy(HotUpdateDllSrc, HotUpdateDllDest, overwrite: true);
        Debug.Log($"[HotfixPipeline] HotUpdate.dll copied to {HotUpdateDllDest}");
        AssetDatabase.Refresh();
    }

    [MenuItem("Hotfix/Generate AOT Metadata (HybridCLR)")]
    public static void GenerateAOT()
    {
        PrebuildCommand.GenerateAll();
        Debug.Log("[HotfixPipeline] HybridCLR GenerateAll done.");
    }
}
#endif
