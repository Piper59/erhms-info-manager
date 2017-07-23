using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Utility
{
    public class TwoWayDictionary<T1, T2> : ICollection<Tuple<T1, T2>>
    {
        private static Tuple<T1, T2> ToItem(KeyValuePair<T1, T2> pair)
        {
            return Tuple.Create(pair.Key, pair.Value);
        }

        private static KeyValuePair<T1, T2> ToForwardPair(Tuple<T1, T2> item)
        {
            return new KeyValuePair<T1, T2>(item.Item1, item.Item2);
        }

        private static KeyValuePair<T2, T1> ToReversePair(Tuple<T1, T2> item)
        {
            return new KeyValuePair<T2, T1>(item.Item2, item.Item1);
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

        public void Add(Tuple<T1, T2> item)
        {
            Add(item.Item1, item.Item2);
        }

        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }

        public bool Contains(Tuple<T1, T2> item)
        {
            return forward.Contains(ToForwardPair(item));
        }

        public void CopyTo(Tuple<T1, T2>[] array, int arrayIndex)
        {
            int index = arrayIndex;
            foreach (KeyValuePair<T1, T2> pair in forward)
            {
                array[index++] = ToItem(pair);
            }
        }

        public bool Remove(Tuple<T1, T2> item)
        {
            if (forward.Remove(ToForwardPair(item)))
            {
                try
                {
                    reverse.Remove(ToReversePair(item));
                }
                catch
                {
                    forward.Add(item.Item1, item.Item2);
                    throw;
                }
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
            return forward.Select(ToItem).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
