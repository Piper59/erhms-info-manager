using System.Collections.Generic;

namespace ERHMS.Common.Collections
{
    public interface IReadOnlyTwoWayMap<T1, T2>
    {
        IReadOnlyDictionary<T1, T2> Forward { get; }
        IReadOnlyDictionary<T2, T1> Reverse { get; }
        int Count { get; }
    }
}
