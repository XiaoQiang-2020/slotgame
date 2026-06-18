using YooAsset;

namespace Framework.Patch
{
    public enum PatchStatus
    {
        Idle,
        Initializing,
        CheckingVersion,
        UpdatingManifest,
        Updating,
        Downloading,
        Completed,
        Error
    }

    public class PatchModel
    {
        public string PackageName { get; }
        public EPlayMode PlayMode { get; }
        public PatchStatus Status { get; set; } = PatchStatus.Idle;
        public string Tips { get; set; }
        public float Progress { get; set; }
        public string ErrorMessage { get; set; }

        public PatchModel(string packageName, EPlayMode playMode)
        {
            PackageName = packageName;
            PlayMode = playMode;
            Status = PatchStatus.Idle;
            Tips = "Waiting for patch initialization.";
            Progress = 0f;
            ErrorMessage = string.Empty;
        }
    }
}
