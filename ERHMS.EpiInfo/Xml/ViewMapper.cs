using Epi;
using System.Collections.Generic;

namespace ERHMS.EpiInfo.Xml
{
    public class ViewMapper : Mapper<View>
    {
        protected override string ElementName => ElementNames.View;

        public ViewMapper()
        {
            Mappings = new List<Mapping>
            {
                Mapping.FromExpr(v => v.Id, attributeName: ColumnNames.VIEW_ID),
                Mapping.FromExpr(v => v.Name),
                Mapping.FromExpr(v => v.IsRelatedView),
                Mapping.FromExpr(v => v.CheckCode),
                Mapping.FromExpr(v => v.PageWidth, attributeName: ColumnNames.PAGE_WIDTH),
                Mapping.FromExpr(v => v.PageHeight, attributeName: ColumnNames.PAGE_HEIGHT),
                Mapping.FromExpr(v => v.PageOrientation, attributeName: ColumnNames.PAGE_ORIENTATION),
                Mapping.FromExpr(v => v.PageLabelAlign, attributeName: ColumnNames.PAGE_LABEL_ALIGN)
            };
            if (ConfigurationExtensions.CompatibilityMode)
            {
                Mappings.Add(Mapping.FromExpr(v => v.WebSurveyId, Mapping.Ignored, "SurveyId"));
            }
        }
    }
}
