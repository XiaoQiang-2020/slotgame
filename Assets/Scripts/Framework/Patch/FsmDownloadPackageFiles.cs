using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;

namespace Framework.Patch
{
    internal class FsmDownloadPackageFiles : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter()
        {
            PatchStepChangedEvent.SendEventMessage("Downloading resource files.");
            PatchBoot.Instance.RunCoroutine(StartDownload());
        }

        void IStateNode.OnUpdate() { }

        void IStateNode.OnExit() { }

        private IEnumerator StartDownload()
        {
            var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
            downloader.DownloadError += PatchWebFileDownloadFailedEvent.SendEventMessage;
            downloader.DownloadProgressChanged += PatchDownloadUpdatedEvent.SendEventMessage;
            downloader.StartDownload();
            yield return downloader;

            if (downloader.Status != EOperationStatus.Succeeded)
                yield break;

            _machine.ChangeState<FsmDownloadPackageOver>();
        }
    }
}
