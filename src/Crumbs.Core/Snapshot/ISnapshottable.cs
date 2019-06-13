namespace Crumbs.Core.Snapshot
{
    public interface ISnapshottable<T> where T : Snapshot
    {
        T CreateSnapshot();
        void RestoreFromSnapshot(T snapshot);
    }
}