using Epi;
using Epi.Data;
using ERHMS.Utility;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public class Project : Epi.Project
    {
        public new DirectoryInfo Location { get; private set; }

        public IDbDriver Driver
        {
            get { return CollectedData.GetDbDriver(); }
        }

        public Project(string name, DirectoryInfo location, string driver, DbConnectionStringBuilder builder)
        {
            Log.Current.DebugFormat("Opening project: {0}, {1}, {2}, {3}", name, location.FullName, driver, builder.ToSafeString());
            Name = name;
            base.Location = location.FullName;
            CollectedDataDriver = driver;
            CollectedDataConnectionString = builder.ConnectionString;
            CollectedDataDbInfo.DBCnnStringBuilder.ConnectionString = builder.ConnectionString;
            CollectedData.Initialize(CollectedDataDbInfo, driver, false);
            MetadataSource = MetadataSource.SameDb;
            Metadata.AttachDbDriver(CollectedData.GetDbDriver());
        }

        public DataTable GetFieldsAsDataTable()
        {
            string sql = "SELECT * FROM metaFields";
            return Driver.Select(Driver.CreateQuery(sql));
        }

        private void DeleteField(int fieldId)
        {
            string sql = "DELETE FROM metaFields WHERE FieldId = @FieldId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, fieldId));
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

        private void DeleteViewAndDescendants(int viewId, DataTable views, DataTable fields)
        {
            DataRow[] relatedFields = fields.Select(string.Format("RelatedViewId = {0:d}", (int)MetaFieldType.Relate));
            foreach (DataRow field in relatedFields.Where(field => field.Field<int>("ViewId") == viewId))
            {
                int? relatedViewId = field.Field<int?>("RelatedViewId");
                if (relatedViewId.HasValue)
                {
                    DeleteViewAndDescendants(relatedViewId.Value, views, fields);
                }
            }
            foreach (DataRow field in relatedFields.Where(field => field.Field<int?>("RelatedViewId") == viewId))
            {
                DeleteField(field.Field<int>("FieldId"));
            }
            DeleteView(viewId);
        }

        public void DeleteViewAndDescendants(int viewId)
        {
            DeleteViewAndDescendants(viewId, GetViewsAsDataTable(), GetFieldsAsDataTable());
        }

        public void DeletePgm(int pgmId)
        {
            Metadata.DeletePgm(pgmId);
        }
    }
}
