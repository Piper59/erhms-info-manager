using ERHMS.EpiInfo.Data.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo.Data.Collections
{
    public class GridEntityCollection : ICollection<GridEntity>, ICloneable
    {
        private ICollection<GridEntity> @base;

        public int Count
        {
            get { return @base.Count; }
        }

        public bool IsReadOnly
        {
            get { return @base.IsReadOnly; }
        }

        public GridEntityCollection()
        {
            @base = new List<GridEntity>();
        }

        public GridEntityCollection(DataTable table) : this()
        {
            foreach (DataRow row in table.Rows)
            {
                Add(new GridEntity(row));
            }
        }

        public void Add(GridEntity item)
        {
            @base.Add(item);
        }

        public void Clear()
        {
            @base.Clear();
        }

        public bool Contains(GridEntity item)
        {
            return @base.Contains(item);
        }

        public void CopyTo(GridEntity[] array, int arrayIndex)
        {
            @base.CopyTo(array, arrayIndex);
        }

        public bool Remove(GridEntity item)
        {
            return @base.Remove(item);
        }

        public IEnumerator<GridEntity> GetEnumerator()
        {
            return @base.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public object Clone()
        {
            GridEntityCollection clone = new GridEntityCollection();
            foreach (GridEntity entity in this)
            {
                clone.Add((GridEntity)entity.Clone());
            }
            return clone;
        }
    }
}
