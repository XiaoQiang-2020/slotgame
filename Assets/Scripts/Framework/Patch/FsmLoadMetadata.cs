using UniFramework.Machine;
using YooAsset;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;

namespace Framework.Patch
{
    internal class FsmLoadMetadata : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter()
        {
            PatchStepChangedEvent.SendEventMessage("Loading metadata for AOT assemblies.");
            PatchBoot.Instance.RunCoroutine(IEnumLoadMetadataForAOTAssemblies());
        }

        void IStateNode.OnUpdate() { }

        void IStateNode.OnExit() { }

        private IEnumerator IEnumLoadMetadataForAOTAssemblies()
        {
            var packageName = (string)_machine.GetBlackboardValue("PackageName");
            var package = YooAssets.GetPackage(packageName);
            var assets = new List<string> { "HotUpdate.dll" }.Concat(AOTMetaAssemblyFiles);

            foreach (var asset in assets)
            {
                var handle = package.LoadAssetAsync<TextAsset>(asset);
                yield return handle;

                var assetObj = handle.AssetObject as TextAsset;
                if (assetObj != null)
                {
                    s_assetDatas[asset] = assetObj;
                    Debug.Log($"Loaded patch metadata asset: {asset}");
                }
                else
                {
                    Debug.LogWarning($"Failed to load patch metadata asset: {asset}");
                }
            }

            LoadMetadataForAOTAssemblies();
            yield return null;

            if (_machine.PreviousNode.GetType() == typeof(FsmCreateDownloader))
            {
                _machine.ChangeState<FsmStartGame>();
            }
            else
            {
                _machine.ChangeState<FsmClearCacheBundle>();
            }
        }

        private static List<string> AOTMetaAssemblyFiles { get; } = new() { "mscorlib.dll", "System.dll", "System.Core.dll" };
        private static readonly Dictionary<string, TextAsset> s_assetDatas = new();

        public static byte[] ReadBytesFromAsset(string dllName)
        {
            if (s_assetDatas.TryGetValue(dllName, out var textAsset))
                return textAsset.bytes;

            return Array.Empty<byte>();
        }

        public static void LoadMetadataForAOTAssemblies()
        {
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in AOTMetaAssemblyFiles)
            {
                byte[] dllBytes = ReadBytesFromAsset(aotDllName);
                if (dllBytes.Length == 0)
                {
                    Debug.LogWarning($"Metadata asset not found: {aotDllName}");
                    continue;
                }

                LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                Debug.Log($"LoadMetadataForAOTAssembly: {aotDllName} -> {err}");
            }
        }
    }
}
