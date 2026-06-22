using UniFramework.Machine;

namespace Framework.Patch
{
    internal class FsmStartGame : IStateNode
    {
        void IStateNode.OnCreate(StateMachine machine) { }

        void IStateNode.OnEnter()
        {
            PatchStepChangedEvent.SendEventMessage("Starting game.");

            PatchCompletedEvent.SendEventMessage();
        }

        void IStateNode.OnUpdate() { }

        void IStateNode.OnExit() { }
    }
}
