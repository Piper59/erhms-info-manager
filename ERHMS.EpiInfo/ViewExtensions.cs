using Epi;
using Epi.Data.Services;
using Epi.Enter.Forms;
using Epi.Fields;
using Epi.Windows.ImportExport.Dialogs;
using ERHMS.Common.Logging;
using ERHMS.EpiInfo.Naming;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo
{
    public static class ViewExtensions
    {
        public static void LoadFields(this View @this)
        {
            @this.MustRefreshFieldCollection = true;
            _ = @this.Fields;
        }

        public static Page GetPageByName(this View @this, string pageName)
        {
            return @this.Pages.SingleOrDefault(page => NameComparer.Default.Equals(page.Name, pageName));
        }

        public static void Unrelate(this View @this)
        {
            Log.Instance.Debug($"Unrelating view: {@this.DisplayName}");
            IMetadataProvider metadata = @this.GetMetadata();
            if (@this.ParentView != null)
            {
                IEnumerable<Field> relateFields = @this.ParentView.Fields.RelatedFields
                    .Cast<RelatedViewField>()
                    .Where(field => field.RelatedViewID == @this.Id)
                    .ToList();
                foreach (Field relateField in relateFields)
                {
                    metadata.DeleteField(relateField);
                    @this.ParentView.Fields.Remove(relateField);
                }
            }
            @this.IsRelatedView = false;
            metadata.UpdateView(@this);
        }

        private static void ShowDialog(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
        }

        public static void ImportFromPackage(this View @this)
        {
            using (Form form = new ImportEncryptedDataPackageDialog(@this))
            {
                ShowDialog(form);
            }
        }

        public static void ImportFromProject(this View @this)
        {
            using (Form form = new ImportDataForm(@this))
            {
                ShowDialog(form);
            }
        }

        public static void ExportToPackage(this View @this)
        {
            using (Form form = new PackageForTransportDialog(@this.Project.FilePath, @this))
            {
                ShowDialog(form);
            }
        }
    }
}
