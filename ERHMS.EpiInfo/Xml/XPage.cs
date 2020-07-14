using Epi;
using ERHMS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace ERHMS.EpiInfo.Xml
{
    public class XPage : XElement
    {
        private const double GroupTabIndexDifference = 0.1;

        private static IEnumerable<DataRow> GetFields(Page page)
        {
            DataTable table = page.GetMetadata().GetFieldsOnPageAsDataTable(page.Id);
            table.SetColumnDataType(ColumnNames.TAB_INDEX, typeof(double));
            IDictionary<string, DataRow> fields = table.Rows.Cast<DataRow>()
                .ToDictionary(field => field.Field<string>(ColumnNames.NAME), StringComparer.OrdinalIgnoreCase);
            IEnumerable<DataRow> groups = fields.Values
                .Where(field => (MetaFieldType)field.Field<int>(ColumnNames.FIELD_TYPE_ID) == MetaFieldType.Group);
            foreach (DataRow group in groups)
            {
                IEnumerable<DataRow> members = group.Field<string>(ColumnNames.LIST)
                    .Split(Constants.LIST_SEPARATOR)
                    .Select(fieldName => fields.TryGetValue(fieldName, out DataRow field) ? field : null)
                    .Where(field => field != null);
                double? tabIndexMin = members.Min(member => member.Field<double?>(ColumnNames.TAB_INDEX));
                if (tabIndexMin == null)
                {
                    continue;
                }
                group.SetField(ColumnNames.TAB_INDEX, tabIndexMin.Value - GroupTabIndexDifference);
            }
            return fields.Values.OrderBy(field => field.Field<double?>(ColumnNames.TAB_INDEX));
        }

        public int PageId
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public new string Name
        {
            get { return (string)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public int Position
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public int BackgroundId
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        public int ViewId
        {
            get { return (int)this.GetAttribute(); }
            private set { this.SetAttributeValue(value); }
        }

        private XPage()
            : base(ElementNames.Page) { }

        public XPage(Page page)
            : this()
        {
            Log.Default.Debug($"Adding page: {page.Name}");
            PageId = page.Id;
            Name = page.Name;
            Position = page.Position;
            BackgroundId = page.BackgroundId;
            ViewId = page.GetView().Id;
            foreach (DataRow field in GetFields(page))
            {
                Add(new XField(field));
            }
        }
    }
}
