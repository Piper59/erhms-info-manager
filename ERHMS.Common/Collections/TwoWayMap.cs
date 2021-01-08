using System.Collections;
using System.Collections.Generic;

namespace ERHMS.Common.Collections
{
    public class TwoWayMap<T1, T2> : ITwoWayMap<T1, T2>, IEnumerable<KeyValuePair<T1, T2>>
    {
        private readonly Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
        public IReadOnlyDictionary<T1, T2> Forward => forward;

        private readonly Dictionary<T2, T1> reverse = new Dictionary<T2, T1>();
        public IReadOnlyDictionary<T2, T1> Reverse => reverse;

        public int Count => forward.Count;

        public void Add(T1 item1, T2 item2)
        {
            forward.Add(item1, item2);
            try
            {
                reverse.Add(item2, item1);
            }
            catch
            {
                forward.Remove(item1);
                throw;
            }
        }

        public bool Remove(T1 item1, T2 item2)
        {
            if (forward.TryGetValue(item1, out T2 value) && EqualityComparer<T2>.Default.Equals(item2, value))
            {
                forward.Remove(item1);
                reverse.Remove(item2);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return forward.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
