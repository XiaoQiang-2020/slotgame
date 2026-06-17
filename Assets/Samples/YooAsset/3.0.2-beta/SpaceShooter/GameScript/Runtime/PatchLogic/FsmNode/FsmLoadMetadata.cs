using UniFramework.Machine;
using YooAsset;
using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;


internal class FsmLoadMetadata : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        Debug.Log("OnEnter 补充AOT元数据");
        GameManager.Instance.StartCoroutine(IEnumLoadMetadataForAOTAssemblies());
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private IEnumerator IEnumLoadMetadataForAOTAssemblies()
    {
        PatchStepChangedEvent.SendEventMessage("补充AOT元数据.");
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        //判断是否下载成功
        var assets = new List<string> { "HotUpdate.dll" }.Concat(AOTMetaAssemblyFiles);
        foreach (var asset in assets)
        {
            var handle = package.LoadAssetAsync<TextAsset>(asset);
            yield return handle;
            var assetObj = handle.AssetObject as TextAsset;
            s_assetDatas[asset] = assetObj;
            Debug.Log($"dll:{asset}   {assetObj == null}");
        }
        yield return null;

        GameManager.Instance.SetAssetData(s_assetDatas);


        Debug.Log($"FullName:{_machine.PreviousNode.GetType().FullName}");
        if (_machine.PreviousNode.GetType().FullName == "FsmCreateDownloader")
        {
            _machine.ChangeState<FsmStartGame>();
        }
        else
        {
            _machine.ChangeState<FsmClearCacheBundle>();
        }
    }

    #region 补充元数据

    //补充元数据dll的列表
    //通过RuntimeApi.LoadMetadataForAOTAssembly()函数来补充AOT泛型的原始元数据
    private static List<string> AOTMetaAssemblyFiles { get; } = new() { "mscorlib.dll", "System.dll", "System.Core.dll", };
    private static Dictionary<string, TextAsset> s_assetDatas = new Dictionary<string, TextAsset>();

    #endregion

}