#if UNITY_EDITOR
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class HotfixPipeline
{
    private const string HotfixProjectPathPref = "HotfixProjectPath";

    [MenuItem("Hotfix/Build Hotfix DLL (dotnet)")]
    public static void BuildHotfixDll()
    {
        string hotfixPath = EditorPrefs.GetString(HotfixProjectPathPref, "");
        if (string.IsNullOrEmpty(hotfixPath))
        {
            UnityEngine.Debug.LogError("Please set Hotfix project path in EditorPrefs (HotfixProjectPath)");
            return;
        }

        var psi = new ProcessStartInfo("dotnet", $"build \"{hotfixPath}\" -c Release -o ./HotfixBuild")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        var p = Process.Start(psi);
        p.OutputDataReceived += (s, e) => { if (e.Data != null) UnityEngine.Debug.Log(e.Data); };
        p.BeginOutputReadLine();
        p.WaitForExit();

        if (p.ExitCode != 0) UnityEngine.Debug.LogError("dotnet build failed");
        else UnityEngine.Debug.Log("Hotfix build finished. Copy DLLs to Assets/HotfixDlls/");
    }

    [MenuItem("Hotfix/Generate AOT Metadata (HybridCLR)")]
    public static void GenerateAOT()
    {
        // Try call HybridCLR's GenerateMD by reflection if present
        var asm = System.AppDomain.CurrentDomain.GetAssemblies();
        foreach (var a in asm)
        {
            if (a.FullName.Contains("HybridCLR"))
            {
                UnityEngine.Debug.Log("HybridCLR assembly found: " + a.FullName);
                // Attempt to find GenerateMD static method (name differs by version)
                var type = a.GetType("HybridCLR.Editor.GenerateMD");
                if (type == null) type = a.GetType("HybridCLR.Editor.GenerateMetadata");
                if (type != null)
                {
                    var method = type.GetMethod("GenerateAll") ?? type.GetMethod("Run") ?? type.GetMethod("Generate");
                    if (method != null)
                    {
                        method.Invoke(null, null);
                        UnityEngine.Debug.Log("Called HybridCLR GenerateMD method via reflection.");
                        return;
                    }
                }
            }
        }
        UnityEngine.Debug.LogWarning("HybridCLR generate method not found. Please run the demo's GenerateMD method or use the provided PowerShell script.");
    }

    [MenuItem("Hotfix/Build YooAsset Packages (Open docs)")]
    public static void OpenYooAssetDocs()
    {
        Application.OpenURL("file://" + Path.GetFullPath("docs/windows_pipeline.md"));
    }
}
#endif