using Epi;
using Epi.Data;
using System.Collections.Generic;
using System.Data;

namespace ERHMS.EpiInfo
{
    internal class ViewDeleter
    {
        private DataTable fields;
        private DataTable views;

        public Project Project { get; private set; }

        private IDbDriver Driver
        {
            get { return Project.Driver; }
        }

        public ViewDeleter(Project project)
        {
            Project = project;
            fields = project.GetFieldsAsDataTable();
            views = project.GetViewsAsDataTable();
        }

        private IEnumerable<int> GetChildViewIds(int viewId)
        {
            string filter = string.Format("FieldTypeId = {0:d} AND ViewId = {1}", MetaFieldType.Relate, viewId);
            foreach (DataRow childField in fields.Select(filter))
            {
                int? childViewId = childField.Field<int?>("RelatedViewId");
                if (childViewId.HasValue)
                {
                    yield return childViewId.Value;
                }
            }
        }

        private IEnumerable<int> GetParentFieldIds(int viewId)
        {
            string filter = string.Format("FieldTypeId = {0:d} AND RelatedViewId = {1}", MetaFieldType.Relate, viewId);
            foreach (DataRow parentField in fields.Select(filter))
            {
                yield return parentField.Field<int>("FieldId");
            }
        }

        private void DeleteField(int fieldId)
        {
            string sql = "DELETE FROM metaFields WHERE FieldId = @FieldId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@FieldId", DbType.Int32, fieldId));
            Driver.ExecuteNonQuery(query);
        }

        private void DeleteFieldsByView(int viewId)
        {
            string sql = "DELETE FROM metaFields WHERE ViewId = @ViewId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            Driver.ExecuteNonQuery(query);
        }

        private void DeletePagesByView(int viewId)
        {
            DeleteFieldsByView(viewId);
            string sql = "DELETE FROM metaPages WHERE ViewId = @ViewId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            Driver.ExecuteNonQuery(query);
        }

        private void DeleteView(int viewId)
        {
            DeletePagesByView(viewId);
            string sql = "DELETE FROM metaViews WHERE ViewId = @ViewId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            Driver.ExecuteNonQuery(query);
        }

        public void DeleteViewAndDescendants(int viewId)
        {
            Log.Current.DebugFormat("Deleting view and descendants: {0}", viewId);
            foreach (int childViewId in GetChildViewIds(viewId))
            {
                Log.Current.DebugFormat("Deleting child view: {0}", childViewId);
                DeleteViewAndDescendants(childViewId);
            }
            foreach (int parentFieldId in GetParentFieldIds(viewId))
            {
                Log.Current.DebugFormat("Deleting parent field: {0}", parentFieldId);
                DeleteField(parentFieldId);
            }
            Log.Current.DebugFormat("Deleting view: {0}", viewId);
            DeleteView(viewId);
        }
    }
}
