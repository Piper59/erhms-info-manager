using Epi.Fields;
using Epi.Windows.Analysis.Dialogs;
using Epi.Windows.Analysis.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using View = Epi.View;

namespace ERHMS.EpiInfo.Analysis
{
    internal partial class MappingDialog : DialogBase
    {
        public const string EmptyTarget = "";

        public DataTable Input { get; private set; }
        public View Output { get; private set; }

        public MappingDialog(AnalysisMainForm form, DataTable input, View output)
            : base(form)
        {
            InitializeComponent();
            Input = input;
            Output = output;
            BindMappings();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void BindMappings()
        {
            ICollection<string> sources = Input.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToList();
            ICollection<string> targets = Output.Fields.TableColumnFields
                .Cast<Field>()
                .Select(field => field.Name)
                .ToList();
            colTarget.Items.Clear();
            colTarget.Items.Add(EmptyTarget);
            foreach (string target in targets)
            {
                colTarget.Items.Add(target);
            }
            foreach (string source in sources)
            {
                string target = targets.FirstOrDefault(_target => _target.Equals(source, StringComparison.OrdinalIgnoreCase));
                if (target == null)
                {
                    target = EmptyTarget;
                }
                dgvMappings.Rows.Add(source, target);
            }
        }

        public MappingCollection GetMappings()
        {
            MappingCollection mappings = new MappingCollection();
            foreach (DataGridViewRow row in dgvMappings.Rows)
            {
                string source = (string)row.Cells["colSource"].Value;
                string target = (string)row.Cells["colTarget"].Value;
                if (target.Equals(EmptyTarget, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                mappings.Add(source, target);
            }
            return mappings;
        }
    }
}
