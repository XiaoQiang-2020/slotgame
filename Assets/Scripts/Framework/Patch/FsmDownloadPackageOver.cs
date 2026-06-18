using UniFramework.Machine;

namespace Framework.Patch
{
    internal class FsmDownloadPackageOver : IStateNode
    {
        private StateMachine _machine;

        void IStateNode.OnCreate(StateMachine machine)
        {
            _machine = machine;
        }

        void IStateNode.OnEnter()
        {
            PatchStepChangedEvent.SendEventMessage("Resource files download completed.");
            _machine.ChangeState<FsmClearCacheBundle>();
        }

        void IStateNode.OnUpdate() { }

        void IStateNode.OnExit() { }
    }
}
