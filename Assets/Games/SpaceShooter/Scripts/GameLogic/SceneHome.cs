using UnityEngine;
using YooAsset;

public class SceneHome : MonoBehaviour
{
    public GameObject CanvasDesktop;

    private AssetHandle _windowHandle;

    void Start()
    {
        AsyncLoad();
    }

    private async void AsyncLoad()
    {
        // Load home window.
        _windowHandle = GameManager.Instance.GamePackage.LoadAssetAsync<GameObject>("UIHome");
        await _windowHandle;
        _windowHandle.InstantiateSync(new InstantiateOptions(true, CanvasDesktop.transform, false));




        _windowHandle =GameManager.Instance.GamePackage.LoadAssetAsync<GameObject>("Cube");
        await _windowHandle;
        _windowHandle.InstantiateSync(new InstantiateOptions(true, CanvasDesktop.transform, false));
        Debug.Log("准备实例化 Cube");

        // Debug.Log($"Prefab name is {go.name}");



        // Release unused assets after changing scenes.
        var package = YooAssets.GetPackage("DefaultPackage");
        var operation = package.UnloadUnusedAssetsAsync();
        await operation;
    }

    private void OnDestroy()
    {
        // Release asset handle.
        if (_windowHandle != null)
        {
            _windowHandle.Release();
            _windowHandle = null;
        }
    }
}