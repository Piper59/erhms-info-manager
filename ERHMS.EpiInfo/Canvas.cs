using Epi;
using ERHMS.EpiInfo.Templates;
using ERHMS.Utility;

namespace ERHMS.EpiInfo
{
    public class Canvas
    {
        public const string FileExtension = ".cvs7";

        public static string GetContentForView(View view)
        {
            return new ViewCanvas(view).TransformText();
        }

        public static string GetContentForTable(string connectionString, string tableName)
        {
            return new TableCanvas(connectionString, tableName).TransformText();
        }

        public int CanvasId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }

        public override bool Equals(object obj)
        {
            Canvas canvas = obj as Canvas;
            return canvas != null && canvas.CanvasId == CanvasId && canvas.Name == Name && canvas.Content == Content;
        }

        public override int GetHashCode()
        {
            return ObjectExtensions.GetHashCode(CanvasId, Name, Content);
        }
    }
}
