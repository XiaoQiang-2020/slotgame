using System.Collections;
using UniFramework.Machine;
using UnityEngine;
using YooAsset;

namespace Framework.Patch
{
    internal class FsmRequestPackageVersion : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter()
        {
            PatchStepChangedEvent.SendEventMessage("Requesting package version.");
            PatchBoot.Instance.RunCoroutine(UpdatePackageVersion());
        }

        void IStateNode.OnUpdate() { }

        void IStateNode.OnExit() { }

        private IEnumerator UpdatePackageVersion()
        {
            var packageName = (string)_machine.GetBlackboardValue("PackageName");
            var package = YooAssets.GetPackage(packageName);
            var operation = package.RequestPackageVersionAsync();
            yield return operation;

            if (operation.Status != EOperationStatus.Succeeded)
            {
                Debug.LogWarning(operation.Error);
                PatchPackageVersionRequestFailedEvent.SendEventMessage();
            }
            else
            {
                Debug.Log($"Package version requested: '{operation.PackageVersion}'.");
                _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
                _machine.ChangeState<FsmUpdatePackageManifest>();
            }
        }
    }
}
