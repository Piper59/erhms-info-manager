using Epi.Fields;
using ERHMS.Domain;
using ERHMS.Presentation.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Project = ERHMS.EpiInfo.Project;
using View = Epi.View;

namespace ERHMS.Presentation
{
    public static class DataGridExtensions
    {
        private static IEnumerable<DataGridColumn> GetColumns(Project project, View view, IEnumerable<Field> fields, bool linked)
        {
            List<int> fieldIds = project.GetSortedFieldIds(view.Id).ToList();
            foreach (Field field in fields.OrderBy(field => field is RecStatusField ? int.MaxValue : fieldIds.IndexOf(field.Id)))
            {
                if (linked && field.Name.Equals(FieldNames.ResponderId, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new DataGridTextColumn
                    {
                        Header = "Responder",
                        Binding = new Binding("Responder")
                        {
                            Converter = new NullSafeConverter(new ResponderToFullNameAndEmailAddressConverter())
                        }
                    };
                }
                yield return new DataGridTextColumn
                {
                    Header = field.Name,
                    Binding = new Binding(field.Name)
                };
            }
        }

        public static IEnumerable<DataGridColumn> GetDataColumns(Project project, View view, bool linked)
        {
            return GetColumns(project, view, view.Fields.DataFields.Cast<Field>(), linked);
        }

        public static IEnumerable<DataGridColumn> GetInputColumns(Project project, View view, bool linked)
        {
            return GetColumns(project, view, view.Fields.InputFields.Cast<Field>(), linked);
        }
    }
}
