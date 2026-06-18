using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Event;
using System.Linq;
using System.Reflection;
using YooAsset;

public class GameManager
{
    private static GameManager s_instance;
    public static GameManager Instance
    {
        get
        {
            if (s_instance == null)
                s_instance = new GameManager();
            return s_instance;
        }
    }

    private readonly EventGroup _eventGroup = new EventGroup();
    private ResourcePackage _gamePackage;
    private MonoBehaviour _behaviour;

    /// <summary>
    /// Game package.
    /// </summary>
    public ResourcePackage GamePackage
    {
        get
        {
            if (_gamePackage == null)
                throw new InvalidOperationException("Game package has not been set. Call SetGamePackage before loading game assets.");
            return _gamePackage;
        }
    }

    /// <summary>
    /// Sets the game package.
    /// </summary>
    public void SetGamePackage(ResourcePackage gamePackage)
    {
        _gamePackage = gamePackage ?? throw new ArgumentNullException(nameof(gamePackage));
    }

    /// <summary>
    /// Sets the coroutine runner.
    /// </summary>
    public void SetBehaviour(MonoBehaviour behaviour)
    {
        _behaviour = behaviour ?? throw new ArgumentNullException(nameof(behaviour));
    }

    private GameManager()
    {
        // Register event listeners.
        _eventGroup.AddListener<SceneChangeToHomeEvent>(OnHandleEventMessage);
        _eventGroup.AddListener<SceneChangeToBattleEvent>(OnHandleEventMessage);
    }

    /// <summary>
    /// Starts a coroutine.
    /// </summary>
    public void StartCoroutine(IEnumerator enumerator)
    {
        if (enumerator == null)
            throw new ArgumentNullException(nameof(enumerator));
        if (_behaviour == null)
            throw new InvalidOperationException("Coroutine runner has not been set. Call SetBehaviour before starting coroutines.");

        _behaviour.StartCoroutine(enumerator);
    }

    /// <summary>
    /// Handles event messages.
    /// </summary>
    private void OnHandleEventMessage(IEventMessage message)
    {
        if (message is SceneChangeToHomeEvent)
        {

            StartGame();
            GamePackage.LoadSceneAsync("scene_home");
        }
        else if (message is SceneChangeToBattleEvent)
        {
            GamePackage.LoadSceneAsync("scene_battle");
        }
    }
   #region 补充元数据

    //补充元数据dll的列表
    //通过RuntimeApi.LoadMetadataForAOTAssembly()函数来补充AOT泛型的原始元数据
    private static List<string> AOTMetaAssemblyFiles { get; } = new() { "mscorlib.dll", "System.dll", "System.Core.dll", };
    private static Dictionary<string, TextAsset> s_assetDatas = new Dictionary<string, TextAsset>();
    private static Assembly _hotUpdateAss;
    
    public void SetAssetData(Dictionary<string, TextAsset> assetDatas)
    {
        Debug.Log("SetAssetData");
        s_assetDatas = assetDatas;
    }
    
        // var package = YooAssets.GetPackage("DefaultPackage");
        // //判断是否下载成功
        // var assets = new List<string> { "HotUpdate.dll" }.Concat(AOTMetaAssemblyFiles);
        // foreach (var asset in assets)
        // {
        //     var handle = package.LoadAssetAsync<TextAsset>(asset);
        //     handle.Completed += (obj) =>
        //     {
        //         var assetObj = obj.AssetObject as TextAsset;
        //         s_assetDatas[asset] = assetObj;
        //         Debug.Log($"dll:{asset}   {assetObj == null}");
        //     };
        // }
        
    


    public static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        if (s_assetDatas.ContainsKey(dllName))
        {
            return s_assetDatas[dllName].bytes;
        }

        return Array.Empty<byte>();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    #endregion

    void StartGame()
    {
        Debug.Log("Starting SpaceShooter with bundled C# logic.");
    }

    private void StartHybridClrHotUpdateGame()
    {
        // 加载AOT dll的元数据
        LoadMetadataForAOTAssemblies();
        //加载热更dll
#if !UNITY_EDITOR
        _hotUpdateAss = Assembly.Load(ReadBytesFromStreamingAssets("HotUpdate.dll"));
#else
        _hotUpdateAss = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate");
#endif
        Debug.Log("运行热更代码");
        // StartCoroutine(Run_InstantiateComponentByAsset());
    }

    IEnumerator Run_InstantiateComponentByAsset()
    {
        // 通过实例化assetbundle中的资源，还原资源上的热更新脚本
        var package = YooAssets.GetPackage("DefaultPackage");
        var handle = package.LoadAssetAsync<GameObject>("Cube");
        yield return handle;
        handle.Completed += Handle_Completed;
    }

    private void Handle_Completed(AssetHandle obj)
    {
        Debug.Log("准备实例化");
        GameObject go = obj.InstantiateSync();
        Debug.Log($"Prefab name is {go.name}");
    }

}
