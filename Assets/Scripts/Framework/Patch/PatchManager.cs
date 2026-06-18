using System;
using UniFramework.Machine;
using UniFramework.Event;
using YooAsset;

namespace Framework.Patch
{
    public static class PatchManager
    {
        private static readonly EventGroup _eventGroup = new EventGroup();
        private static StateMachine _machine;

        public static void Create(string packageName, EPlayMode playMode)
        {
            if (string.IsNullOrWhiteSpace(packageName))
                throw new ArgumentException("Package name cannot be null or empty.", nameof(packageName));

            if (!IsValidPlayMode(playMode))
                throw new ArgumentException($"Invalid play mode: {playMode}.", nameof(playMode));

            _eventGroup.AddListener<UserTryInitializePackageEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<UserBeginDownloadWebFilesEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<UserTryRequestPackageVersionEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<UserTryUpdatePackageManifestEvent>(OnHandleEventMessage);
            _eventGroup.AddListener<UserTryDownloadWebFilesEvent>(OnHandleEventMessage);

            _machine = new StateMachine(null);
            _machine.AddNode<FsmInitializePackage>();
            _machine.AddNode<FsmRequestPackageVersion>();
            _machine.AddNode<FsmUpdatePackageManifest>();
            _machine.AddNode<FsmCreateDownloader>();
            _machine.AddNode<FsmDownloadPackageFiles>();
            _machine.AddNode<FsmDownloadPackageOver>();
            _machine.AddNode<FsmClearCacheBundle>();
            _machine.AddNode<FsmStartGame>();

            _machine.SetBlackboardValue("PackageName", packageName);
            _machine.SetBlackboardValue("PlayMode", playMode);
        }

        public static void Start()
        {
            if (_machine == null)
                throw new InvalidOperationException("Patch manager has not been created. Call Create before Start.");

            _machine.Run<FsmInitializePackage>();
        }

        public static void Update()
        {
            if (_machine == null)
                return;

            _machine.Update();
        }

        private static void OnHandleEventMessage(IEventMessage message)
        {
            if (_machine == null)
                return;

            switch (message)
            {
                case UserTryInitializePackageEvent:
                    _machine.ChangeState<FsmInitializePackage>();
                    break;
                case UserBeginDownloadWebFilesEvent:
                    _machine.ChangeState<FsmDownloadPackageFiles>();
                    break;
                case UserTryRequestPackageVersionEvent:
                    _machine.ChangeState<FsmRequestPackageVersion>();
                    break;
                case UserTryUpdatePackageManifestEvent:
                    _machine.ChangeState<FsmUpdatePackageManifest>();
                    break;
                case UserTryDownloadWebFilesEvent:
                    _machine.ChangeState<FsmCreateDownloader>();
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported patch event message type: {message.GetType().FullName}.");
            }
        }

        private static bool IsValidPlayMode(EPlayMode playMode)
        {
            return playMode == EPlayMode.EditorSimulateMode
                || playMode == EPlayMode.OfflinePlayMode
                || playMode == EPlayMode.HostPlayMode
                || playMode == EPlayMode.WebPlayMode;
        }
    }
}
