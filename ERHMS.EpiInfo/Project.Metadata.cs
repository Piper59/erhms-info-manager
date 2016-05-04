using Epi;
using Epi.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ERHMS.EpiInfo
{
    public partial class Project
    {
        public DataTable GetFieldsAsDataTable()
        {
            string sql = "SELECT * FROM metaFields";
            return Driver.Select(Driver.CreateQuery(sql));
        }

        public IEnumerable<int> GetSortedFieldIds(int viewId)
        {
            string sql = @"
                SELECT F.FieldId, F.TabIndex, F.PageId, P.[Position]
                FROM metaFields AS F
                LEFT OUTER JOIN metaPages AS P ON F.PageId = P.PageId
                WHERE F.ViewId = @ViewId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            DataTable fields = Driver.Select(query);
            return fields.AsEnumerable()
                .OrderBy(field => field.IsNull("PageId") ? 1 : 2)
                .ThenBy(field => field.Field<short?>("Position") ?? field.Field<int>("FieldId"))
                .ThenBy(field => field.Field<double?>("TabIndex") ?? field.Field<int>("FieldId"))
                .Select(field => field.Field<int>("FieldId"));
        }

        public IEnumerable<View> GetViews()
        {
            return Metadata.GetViews().Cast<View>();
        }

        public new View GetViewByName(string viewName)
        {
            return GetViews().SingleOrDefault(view => view.Name == viewName);
        }

        public new IEnumerable<Pgm> GetPgms()
        {
            foreach (DataRow row in Metadata.GetPgms().Rows)
            {
                yield return new Pgm
                {
                    PgmId = row.Field<int>("ProgramId"),
                    Name = row.Field<string>("Name"),
                    Content = row.Field<string>("Content"),
                    Comment = row.Field<string>("Comment"),
                    Author = row.Field<string>("Author")
                };
            }
        }

        public Pgm GetPgmById(int pgmId)
        {
            return GetPgms().SingleOrDefault(pgm => pgm.PgmId == pgmId);
        }

        public IEnumerable<Canvas> GetCanvases()
        {
            string sql = "SELECT * FROM metaCanvases";
            foreach (DataRow row in Driver.Select(Driver.CreateQuery(sql)).Rows)
            {
                yield return new Canvas
                {
                    CanvasId = row.Field<int>("CanvasId"),
                    Name = row.Field<string>("Name"),
                    Content = row.Field<string>("Content")
                };
            }
        }

        public Canvas GetCanvasById(int canvasId)
        {
            return GetCanvases().SingleOrDefault(canvas => canvas.CanvasId == canvasId);
        }

        public void InsertPgm(Pgm pgm)
        {
            Metadata.InsertPgm(pgm.Name, pgm.Content ?? "", pgm.Comment ?? "", pgm.Author ?? "");
            string sql = "SELECT MAX(ProgramId) FROM metaPrograms";
            pgm.PgmId = (int)Driver.ExecuteScalar(Driver.CreateQuery(sql));
        }

        public void InsertCanvas(Canvas canvas)
        {
            {
                string sql = "INSERT INTO metaCanvases (Name, Content) VALUES (@Name, @Content)";
                Query query = Driver.CreateQuery(sql);
                query.Parameters.Add(new QueryParameter("@Name", DbType.String, canvas.Name));
                query.Parameters.Add(new QueryParameter("@Content", DbType.String, canvas.Content ?? ""));
                Driver.ExecuteNonQuery(query);
            }
            {
                string sql = "SELECT MAX(CanvasId) FROM metaCanvases";
                canvas.CanvasId = (int)Driver.ExecuteScalar(Driver.CreateQuery(sql));
            }
        }

        public void UpdatePgm(Pgm pgm)
        {
            Metadata.UpdatePgm(pgm.PgmId, pgm.Name, pgm.Content, pgm.Comment, pgm.Author);
        }

        public void UpdateCanvas(Canvas canvas)
        {
            string sql = "UPDATE metaCanvases SET Name = @Name, Content = @Content WHERE CanvasId = @CanvasId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@Name", DbType.String, canvas.Name));
            query.Parameters.Add(new QueryParameter("@Content", DbType.String, canvas.Content));
            query.Parameters.Add(new QueryParameter("@CanvasId", DbType.Int32, canvas.CanvasId));
            Driver.ExecuteNonQuery(query);
        }

        public void DeleteView(int viewId)
        {
            ViewDeleter deleter = new ViewDeleter(this);
            deleter.DeleteViewAndDescendants(viewId);
        }

        public void DeletePgm(int pgmId)
        {
            Metadata.DeletePgm(pgmId);
        }

        public void DeleteCanvas(int canvasId)
        {
            string sql = "DELETE FROM metaCanvases WHERE CanvasId = @CanvasId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@CanvasId", DbType.Int32, canvasId));
            Driver.ExecuteNonQuery(query);
        }
    }
}
