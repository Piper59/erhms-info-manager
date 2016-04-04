using System.Collections.Generic;
using View = Epi.View;

namespace ERHMS.EpiInfo.Enter
{
    public class Enter
    {
        public static MainForm Execute()
        {
            MainForm form = new MainForm();
            form.Show();
            return form;
        }

        public static MainForm OpenView(View view, IDictionary<string, object> values = null)
        {
            MainForm form = new MainForm(view);
            form.Show();
            form.FireOpenViewEvent(view, "+");
            if (values != null)
            {
                foreach (KeyValuePair<string, object> value in values)
                {
                    form.SetValue(value.Key, value.Value);
                }
                form.Refresh();
            }
            return form;
        }

        public static MainForm OpenRecord(View view, int uniqueKey)
        {
            MainForm form = new MainForm(view);
            form.Show();
            form.LoadRecord(uniqueKey);
            return form;
        }
    }
}
