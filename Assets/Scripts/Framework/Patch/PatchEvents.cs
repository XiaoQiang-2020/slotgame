using UniFramework.Event;

namespace Framework.Patch
{
    /// <summary>
    /// Patch process completed event.
    /// </summary>
    public sealed class PatchCompletedEvent : IEventMessage
    {
        public static void SendEventMessage()
        {
            UniEvent.SendMessage(new PatchCompletedEvent());
        }
    }
}
