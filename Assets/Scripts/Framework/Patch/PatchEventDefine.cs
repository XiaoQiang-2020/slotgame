using UniFramework.Event;
using YooAsset;

namespace Framework.Patch
{
    public sealed class PatchInitializeFailedEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new PatchInitializeFailedEvent());
        }
    }

    public sealed class PatchStepChangedEvent : IEventMessage
    {
        public string Tips { get; }

        private PatchStepChangedEvent(string tips)
        {
            Tips = tips;
        }

        public static void SendEventMessage(string tips)
        {
            UniEvent.SendMessage(new PatchStepChangedEvent(tips));
        }
    }

    public sealed class PatchFoundUpdateFilesEvent : IEventMessage
    {
        public int TotalCount { get; }
        public long TotalSizeBytes { get; }

        private PatchFoundUpdateFilesEvent(int totalCount, long totalSizeBytes)
        {
            TotalCount = totalCount;
            TotalSizeBytes = totalSizeBytes;
        }

        public static void SendEventMessage(int totalCount, long totalSizeBytes)
        {
            UniEvent.SendMessage(new PatchFoundUpdateFilesEvent(totalCount, totalSizeBytes));
        }
    }

    public sealed class PatchDownloadUpdatedEvent : IEventMessage
    {
        public int TotalDownloadCount { get; }
        public int CurrentDownloadCount { get; }
        public long TotalDownloadSizeBytes { get; }
        public long CurrentDownloadSizeBytes { get; }

        private PatchDownloadUpdatedEvent(DownloadProgressChangedEventArgs eventArgs)
        {
            TotalDownloadCount = eventArgs.TotalDownloadCount;
            CurrentDownloadCount = eventArgs.CurrentDownloadCount;
            TotalDownloadSizeBytes = eventArgs.TotalDownloadBytes;
            CurrentDownloadSizeBytes = eventArgs.CurrentDownloadBytes;
        }

        public static void SendEventMessage(DownloadProgressChangedEventArgs eventArgs)
        {
            UniEvent.SendMessage(new PatchDownloadUpdatedEvent(eventArgs));
        }
    }

    public sealed class PatchPackageVersionRequestFailedEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new PatchPackageVersionRequestFailedEvent());
        }
    }

    public sealed class PatchPackageManifestUpdateFailedEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new PatchPackageManifestUpdateFailedEvent());
        }
    }

    public sealed class PatchWebFileDownloadFailedEvent : IEventMessage
    {
        public string FileName { get; }
        public string Error { get; }

        private PatchWebFileDownloadFailedEvent(DownloadErrorEventArgs errorData)
        {
            FileName = errorData.FileName;
            Error = errorData.ErrorInfo;
        }

        public static void SendEventMessage(DownloadErrorEventArgs errorData)
        {
            UniEvent.SendMessage(new PatchWebFileDownloadFailedEvent(errorData));
        }
    }

    public sealed class UserTryInitializePackageEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new UserTryInitializePackageEvent());
        }
    }

    public sealed class UserBeginDownloadWebFilesEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new UserBeginDownloadWebFilesEvent());
        }
    }

    public sealed class UserTryRequestPackageVersionEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new UserTryRequestPackageVersionEvent());
        }
    }

    public sealed class UserTryUpdatePackageManifestEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new UserTryUpdatePackageManifestEvent());
        }
    }

    public sealed class UserTryDownloadWebFilesEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new UserTryDownloadWebFilesEvent());
        }
    }
}
