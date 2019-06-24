using Crumbs.Core.Snapshot;

namespace Crumbs.Core.Aggregate
{
    public abstract class SnapshottableAggregateRoot<TState> : AggregateRoot, ISnapshottable<TState>
        where TState : AggrageteState
    {
        public SnapshottableAggregateRoot()
        {
            State = InitializeState();
        }

        protected TState State { get; private set; }

        protected abstract TState InitializeState();

        public TState CreateSnapshot()
        {
            return State;
        }

        public void RestoreFromSnapshot(TState snapshot)
        {
            State = snapshot;
        }
    }
}
