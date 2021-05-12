using System.Collections;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Data
{
    public class RecordCollection<TRecord> : ICollection<TRecord>
        where TRecord : Record, new()
    {
        private readonly IDictionary<string, TRecord> itemsByGlobalRecordId =
            new Dictionary<string, TRecord>(Record.GlobalRecordIdComparer);

        public int Count => itemsByGlobalRecordId.Count;
        public bool IsReadOnly => false;

        public void Add(TRecord item)
        {
            itemsByGlobalRecordId.Add(item.GlobalRecordId, item);
        }

        public void Clear()
        {
            itemsByGlobalRecordId.Clear();
        }

        public bool Contains(TRecord item)
        {
            return itemsByGlobalRecordId.ContainsKey(item.GlobalRecordId);
        }

        public void CopyTo(TRecord[] array, int index)
        {
            itemsByGlobalRecordId.Values.CopyTo(array, index);
        }

        public void Map(RecordMapper<TRecord> mapper)
        {
            if (itemsByGlobalRecordId.TryGetValue(mapper.GlobalRecordId, out TRecord record))
            {
                mapper.Update(record);
            }
            else
            {
                Add(mapper.Create());
            }
        }

        public bool Remove(TRecord item)
        {
            return itemsByGlobalRecordId.Remove(item.GlobalRecordId);
        }

        public IEnumerator<TRecord> GetEnumerator()
        {
            return itemsByGlobalRecordId.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
