using System.Collections;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Data
{
    public class RecordCollection : ICollection<Record>
    {
        private readonly IDictionary<string, Record> itemsByGlobalRecordId =
            new Dictionary<string, Record>(Record.GlobalRecordIdComparer);

        public int Count => itemsByGlobalRecordId.Count;
        public bool IsReadOnly => false;

        public void Add(Record item)
        {
            itemsByGlobalRecordId.Add(item.GlobalRecordId, item);
        }

        public void Clear()
        {
            itemsByGlobalRecordId.Clear();
        }

        public bool Contains(Record item)
        {
            return itemsByGlobalRecordId.ContainsKey(item.GlobalRecordId);
        }

        public void CopyTo(Record[] array, int index)
        {
            itemsByGlobalRecordId.Values.CopyTo(array, index);
        }

        public bool Remove(Record item)
        {
            return itemsByGlobalRecordId.Remove(item.GlobalRecordId);
        }

        public void Update(RecordMapper mapper)
        {
            if (itemsByGlobalRecordId.TryGetValue(mapper.GlobalRecordId, out Record record))
            {
                mapper.Update(record);
            }
            else
            {
                itemsByGlobalRecordId[mapper.GlobalRecordId] = mapper.Create();
            }
        }

        public IEnumerator<Record> GetEnumerator()
        {
            return itemsByGlobalRecordId.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
