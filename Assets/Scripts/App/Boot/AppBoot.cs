using App.Scene;
using Framework.Patch;
using UniFramework.Event;
using UnityEngine;
using YooAsset;

namespace App.Boot
{
    public sealed class AppBoot : MonoBehaviour
    {
        public static AppBoot Instance { get; private set; }

        [SerializeField]
        private string _packageName = "DefaultPackage";

        [SerializeField]
        private EPlayMode _playMode = EPlayMode.EditorSimulateMode;

        private readonly EventGroup _eventGroup = new EventGroup();

        public string PackageName => _packageName;
        public EPlayMode PlayMode => _playMode;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            Application.targetFrameRate = 60;
            Application.runInBackground = true;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            UniEvent.Initalize();
            YooAssets.Initialize();
            CreatePatchWindow();

            _eventGroup.AddListener<PatchCompletedEvent>(OnPatchCompleted);
            PatchFacade.Create(_packageName, _playMode);
            PatchFacade.Start();
        }

        private void Update()
        {
            PatchFacade.Update();
        }

        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();

            if (Instance == this)
                Instance = null;
        }

        private static void CreatePatchWindow()
        {
            var prefab = Resources.Load<GameObject>("PatchWindow");
            if (prefab == null)
            {
                Debug.LogError("PatchWindow prefab not found at Assets/Resources/PatchWindow.prefab.");
                return;
            }

            Instantiate(prefab);
        }

        private static void OnPatchCompleted(IEventMessage message)
        {
            SceneNavigator.LoadLoginScene();
        }
    }
}
