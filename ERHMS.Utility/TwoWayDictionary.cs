using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public class TwoWayDictionary<T1, T2> : ICollection<Tuple<T1, T2>>
    {
        private static Tuple<T1, T2> ToTuple(T1 item1, T2 item2)
        {
            return Tuple.Create(item1, item2);
        }

        private static Tuple<T1, T2> ToTuple(KeyValuePair<T1, T2> pair)
        {
            return ToTuple(pair.Key, pair.Value);
        }

        private static KeyValuePair<T1, T2> ToForwardPair(Tuple<T1, T2> tuple)
        {
            return new KeyValuePair<T1, T2>(tuple.Item1, tuple.Item2);
        }

        private static KeyValuePair<T2, T1> ToReversePair(Tuple<T1, T2> tuple)
        {
            return new KeyValuePair<T2, T1>(tuple.Item2, tuple.Item1);
        }

        private IDictionary<T1, T2> forward;
        private IDictionary<T2, T1> reverse;

        public int Count
        {
            get { return forward.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public TwoWayDictionary()
        {
            forward = new Dictionary<T1, T2>();
            reverse = new Dictionary<T2, T1>();
        }

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

        public void Add(Tuple<T1, T2> tuple)
        {
            Add(tuple.Item1, tuple.Item2);
        }

        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }

        public bool Contains(Tuple<T1, T2> tuple)
        {
            return forward.Contains(ToForwardPair(tuple));
        }

        public void CopyTo(Tuple<T1, T2>[] array, int arrayIndex)
        {
            foreach (Iterator<KeyValuePair<T1, T2>> pair in forward.Iterate())
            {
                array[arrayIndex + pair.Index] = ToTuple(pair.Value);
            }
        }

        public bool Remove(Tuple<T1, T2> tuple)
        {
            if (forward.Remove(ToForwardPair(tuple)))
            {
                reverse.Remove(ToReversePair(tuple));
                return true;
            }
            else
            {
                return false;
            }
        }

        public T2 Forward(T1 item1)
        {
            return forward[item1];
        }

        public T1 Reverse(T2 item2)
        {
            return reverse[item2];
        }

        public IEnumerator<Tuple<T1, T2>> GetEnumerator()
        {
            return forward.Select(ToTuple).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
