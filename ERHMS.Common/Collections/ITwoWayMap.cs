namespace ERHMS.Common.Collections
{
    public interface ITwoWayMap<T1, T2> : IReadOnlyTwoWayMap<T1, T2>
    {
        void Add(T1 item1, T2 item2);
        bool Remove(T1 item1, T2 item2);
        void Clear();
    }
}
