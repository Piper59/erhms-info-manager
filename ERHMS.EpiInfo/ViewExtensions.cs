using Epi;
using Epi.Data.Services;
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
        public static Page GetPageByName(this View @this, string pageName)
        {
            return @this.Pages.SingleOrDefault(page => NameComparer.Default.Equals(page.Name, pageName));
        }

        public static bool ImportFromPackage(this View @this)
        {
            ImportEncryptedDataPackageDialog dialog = new ImportEncryptedDataPackageDialog(@this)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            return dialog.ShowDialog() == DialogResult.OK;
        }

        public static void ExportToPackage(this View @this)
        {
            PackageForTransportDialog dialog = new PackageForTransportDialog(@this.Project.FilePath, @this)
            {
                StartPosition = FormStartPosition.CenterParent
            };
            dialog.ShowDialog();
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
    }
}
