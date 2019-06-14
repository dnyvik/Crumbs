namespace Crumbs.Core.Event.EventualConsistency
{
    public enum StatefulHandlerStatus
    {
        Stopped,
        SpoolingHistory,
        Running,
        Faulted,
    }
}