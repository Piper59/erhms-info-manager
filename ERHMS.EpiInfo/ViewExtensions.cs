using Epi;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static void LoadFields(this View @this)
        {
            @this.MustRefreshFieldCollection = true;
            _ = @this.Fields;
        }

        public static void CreateDataTables(this View @this)
        {
            if (!@this.Project.CollectedData.TableExists(@this.TableName))
            {
                @this.Project.CollectedData.CreateDataTableForView(@this, 1);
            }
        }

        public static void CreateDataTableTree(this View @this)
        {
            CreateDataTables(@this);
            foreach (View view in @this.GetDescendantViews())
            {
                CreateDataTables(view);
            }
        }

        public static void DeleteDataTableTree(this View @this)
        {
            @this.DeleteDataTables();
            foreach (View view in @this.GetDescendantViews())
            {
                view.DeleteDataTables();
            }
        }
    }
}
