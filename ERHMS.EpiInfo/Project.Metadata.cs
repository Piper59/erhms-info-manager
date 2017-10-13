using Epi;
using Epi.Data;
using ERHMS.Utility;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ERHMS.EpiInfo
{
    public partial class Project
    {
        public DataTable GetFieldsAsDataTable()
        {
            string sql = "SELECT * FROM [metaFields]";
            return Driver.Select(Driver.CreateQuery(sql));
        }

        public IEnumerable<int> GetSortedFieldIds(int viewId)
        {
            string sql = @"
                SELECT F.[FieldId], F.[TabIndex], P.[Position]
                FROM [metaFields] AS F
                LEFT OUTER JOIN [metaPages] AS P ON F.[PageId] = P.[PageId]
                WHERE F.[ViewId] = @ViewId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@ViewId", DbType.Int32, viewId));
            short prepend = -1;
            return Driver.Select(query).AsEnumerable()
                .OrderBy(row => row.IsNull("Position") ? prepend : row["Position"])
                .ThenBy(row => row.IsNull("TabIndex") ? prepend : row["TabIndex"])
                .ThenBy(row => row["FieldId"])
                .Select(row => row.Field<int>("FieldId"));
        }

        public IEnumerable<string> GetCodes(string tableName, string columnName, bool sorted)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT {0} FROM {1}", Driver.InsertInEscape(columnName), Driver.InsertInEscape(tableName));
            if (sorted)
            {
                sql.AppendFormat(" ORDER BY {0}", Driver.InsertInEscape(columnName));
            }
            Query query = Driver.CreateQuery(sql.ToString());
            return Driver.Select(query).AsEnumerable().Select(row => row.Field<string>(columnName));
        }

        public IEnumerable<View> GetViews()
        {
            LoadViews();
            return Views.Cast<View>();
        }

        public new View GetViewById(int viewId)
        {
            return Metadata.GetViewById(viewId);
        }

        public new View GetViewByName(string name)
        {
            return Metadata.GetViewByFullName(name);
        }

        public new IEnumerable<Pgm> GetPgms()
        {
            return Metadata.GetPgms().AsEnumerable().Select(row => new Pgm(row));
        }

        public Pgm GetPgmById(int pgmId)
        {
            string sql = "SELECT * FROM [metaPrograms] WHERE [ProgramId] = @PgmId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@PgmId", DbType.Int32, pgmId));
            return new Pgm(Driver.Select(query).AsEnumerable().SingleOrDefault());
        }

        public IEnumerable<Canvas> GetCanvases()
        {
            string sql = "SELECT * FROM [metaCanvases]";
            return Driver.Select(Driver.CreateQuery(sql)).AsEnumerable().Select(row => new Canvas(row));
        }

        public Canvas GetCanvasById(int canvasId)
        {
            string sql = "SELECT * FROM [metaCanvases] WHERE [CanvasId] = @CanvasId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@CanvasId", DbType.Int32, canvasId));
            return new Canvas(Driver.Select(query).AsEnumerable().SingleOrDefault());
        }

        public void InsertPgm(Pgm pgm)
        {
            Log.Logger.DebugFormat("Inserting PGM: {0}", pgm.Name);
            Metadata.InsertPgm(pgm.Name, pgm.Content, pgm.Comment, pgm.Author);
            string sql = "SELECT MAX([ProgramId]) FROM [metaPrograms]";
            pgm.PgmId = (int)Driver.ExecuteScalar(Driver.CreateQuery(sql));
        }

        public void InsertCanvas(Canvas canvas)
        {
            Log.Logger.DebugFormat("Inserting canvas: {0}", canvas.Name);
            {
                string sql = "INSERT INTO [metaCanvases] ([Name], [Content]) VALUES (@Name, @Content)";
                Query query = Driver.CreateQuery(sql);
                query.Parameters.Add(new QueryParameter("@Name", DbType.String, canvas.Name));
                query.Parameters.Add(new QueryParameter("@Content", DbType.String, canvas.Content));
                Driver.ExecuteNonQuery(query);
            }
            {
                string sql = "SELECT MAX([CanvasId]) FROM [metaCanvases]";
                canvas.CanvasId = (int)Driver.ExecuteScalar(Driver.CreateQuery(sql));
            }
        }

        public void UpdatePgm(Pgm pgm)
        {
            Log.Logger.DebugFormat("Updating PGM: {0}", pgm.Name);
            Metadata.UpdatePgm(pgm.PgmId, pgm.Name, pgm.Content, pgm.Comment, pgm.Author);
        }

        public void UpdateCanvas(Canvas canvas)
        {
            Log.Logger.DebugFormat("Updating canvas: {0}", canvas.Name);
            string sql = "UPDATE [metaCanvases] SET [Name] = @Name, [Content] = @Content WHERE [CanvasId] = @CanvasId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@Name", DbType.String, canvas.Name));
            query.Parameters.Add(new QueryParameter("@Content", DbType.String, canvas.Content));
            query.Parameters.Add(new QueryParameter("@CanvasId", DbType.Int32, canvas.CanvasId));
            Driver.ExecuteNonQuery(query);
        }

        public void DeleteView(int viewId)
        {
            Log.Logger.DebugFormat("Deleting view: {0}", viewId);
            ViewDeleter deleter = new ViewDeleter(this);
            deleter.DeleteViewAndDescendants(viewId);
            LoadViews();
        }

        public void DeletePgm(int pgmId)
        {
            Log.Logger.DebugFormat("Deleting PGM: {0}", pgmId);
            Metadata.DeletePgm(pgmId);
        }

        public void DeleteCanvas(int canvasId)
        {
            Log.Logger.DebugFormat("Deleting canvas: {0}", canvasId);
            string sql = "DELETE FROM [metaCanvases] WHERE [CanvasId] = @CanvasId";
            Query query = Driver.CreateQuery(sql);
            query.Parameters.Add(new QueryParameter("@CanvasId", DbType.Int32, canvasId));
            Driver.ExecuteNonQuery(query);
        }
    }
}
