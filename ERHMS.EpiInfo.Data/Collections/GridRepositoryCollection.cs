using Epi;
using Epi.Fields;
using ERHMS.EpiInfo.Data.Repositories;
using System.Collections;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Data.Collections
{
    public class GridRepositoryCollection : IEnumerable<GridRepository>
    {
        private IDictionary<string, GridRepository> @base;

        public GridRepositoryCollection(View view)
        {
            @base = new Dictionary<string, GridRepository>();
            foreach (GridField field in view.Fields.GridFields)
            {
                @base[field.Name] = new GridRepository(field);
            }
        }

        public GridRepository this[string fieldName]
        {
            get { return @base[fieldName]; }
        }

        public IEnumerator<GridRepository> GetEnumerator()
        {
            return @base.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
